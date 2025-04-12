using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Catalog;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;

namespace KliensApp
{
    public partial class Form1 : Form
    {
        private readonly string apiKey;
        private readonly string apiUrl;
        private readonly Api proxy;
        private List<ProductDTO> currentProducts;
        private List<ProductTypeDTO> productTypes;
        private ComboBox productTypeComboBox;

        public Form1()
        {
            InitializeComponent();
            apiKey = ConfigurationManager.AppSettings["apikulcs"];
            apiUrl = ConfigurationManager.AppSettings["apiurl"];
            proxy = new Api(apiUrl, apiKey);
            InitializeComboBox();
            InitializeProductTypeComboBox();
        }

        private void InitializeComboBox()
        {
            var properties = typeof(ProductDTO).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.CanRead && p.Name != "Bvin" && p.Name != "StoreId" && p.Name != "ProductTypeId")
                .Select(p => p.Name)
                .OrderBy(name => name)
                .ToList();

            properties.Add("ProductType");
            comboBoxProperties.Items.Clear();
            comboBoxProperties.Items.AddRange(properties.ToArray());
            if (properties.Contains("SitePrice"))
                comboBoxProperties.SelectedIndex = properties.IndexOf("SitePrice");
            else if (properties.Count > 0)
                comboBoxProperties.SelectedIndex = 0;
        }

        private void InitializeProductTypeComboBox()
        {
            if (productTypeComboBox == null)
            {
                productTypeComboBox = new ComboBox
                {
                    Location = new System.Drawing.Point(textBox1.Location.X, textBox1.Location.Y),
                    Width = textBox1.Width,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Visible = false
                };
                this.Controls.Add(productTypeComboBox);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProducts();
            LoadProductTypes();
        }

        private async Task LoadProducts()
        {
            try
            {
                var products = await Task.Run(() => proxy.ProductsFindAll());
                if (products.Errors.Any())
                {
                    MessageBox.Show($"API hiba: {string.Join(", ", products.Errors.Select(e => e.Description))}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                currentProducts = products.Content;
                DisplayProducts(currentProducts);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProductTypes()
        {
            try
            {
                var types = await Task.Run(() => proxy.ProductTypesFindAll());
                if (types.Errors.Any())
                {
                    MessageBox.Show($"API hiba a terméktípusok lekérésekor: {string.Join(", ", types.Errors.Select(e => e.Description))}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                productTypes = types.Content;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a terméktípusok lekérésekor: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayProducts(List<ProductDTO> products)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("ProductName", "Termék neve");
            dataGridView1.Columns.Add("Sku", "SKU");
            dataGridView1.Columns.Add("Bvin", "ID");

            string selectedProperty = comboBoxProperties.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedProperty))
            {
                dataGridView1.Columns.Add(selectedProperty, selectedProperty == "ProductType" ? "Terméktípus" : selectedProperty);
            }

            foreach (var product in products)
            {
                var row = new List<object> { product.ProductName, product.Sku, product.Bvin };
                if (!string.IsNullOrEmpty(selectedProperty))
                {
                    if (selectedProperty == "ProductType" && productTypes != null)
                    {
                        var type = productTypes.FirstOrDefault(t => t.Bvin == product.ProductTypeId);
                        row.Add(type?.ProductTypeName ?? product.ProductTypeId);
                    }
                    else
                    {
                        var propertyInfo = typeof(ProductDTO).GetProperty(selectedProperty);
                        var value = propertyInfo?.GetValue(product);
                        if (value is decimal decValue)
                        {
                            row.Add(decValue.ToString("F0"));
                        }
                        else
                        {
                            row.Add(value?.ToString() ?? "");
                        }
                    }
                }
                dataGridView1.Rows.Add(row.ToArray());
            }

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private async void btnGetProducts_Click(object sender, EventArgs e)
        {
            await LoadProducts();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            string selectedProperty = comboBoxProperties.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedProperty))
            {
                MessageBox.Show("Kérem válasszon ki egy tulajdonságot!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string newValueText = selectedProperty == "ProductType" ? productTypeComboBox.SelectedItem?.ToString() : textBox1.Text;
            if (string.IsNullOrWhiteSpace(newValueText))
            {
                MessageBox.Show("Kérem adjon meg vagy válasszon ki egy értéket!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("Kérem jelöljön ki legalább egy cellát!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int updatedCount = 0;

                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    int rowIndex = cell.RowIndex;
                    string bvin = dataGridView1.Rows[rowIndex].Cells["Bvin"].Value?.ToString();
                    if (string.IsNullOrEmpty(bvin))
                    {
                        MessageBox.Show("Érvénytelen termék ID a kijelölt sorban!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    var productResponse = await Task.Run(() => proxy.ProductsFind(bvin));
                    if (productResponse == null || productResponse.Content == null)
                    {
                        MessageBox.Show($"Nem található a termék (ID: {bvin})!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    if (productResponse.Errors.Any())
                    {
                        MessageBox.Show($"Hiba a termék lekérésekor (ID: {bvin}): {string.Join(", ", productResponse.Errors.Select(x => x.Description))}",
                            "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    var product = productResponse.Content;

                    if (selectedProperty == "ProductType")
                    {
                        var selectedType = productTypes?.FirstOrDefault(t => t.ProductTypeName == newValueText);
                        if (selectedType == null)
                        {
                            MessageBox.Show("Érvénytelen terméktípus!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }
                        product.ProductTypeId = selectedType.Bvin;
                    }
                    else
                    {
                        var propertyInfo = typeof(ProductDTO).GetProperty(selectedProperty);
                        if (propertyInfo == null)
                        {
                            MessageBox.Show($"Érvénytelen tulajdonság: {selectedProperty}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        object newValue;
                        try
                        {
                            newValue = ConvertValue(newValueText, propertyInfo.PropertyType);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Érvénytelen érték a(z) {selectedProperty} tulajdonsághoz: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        propertyInfo.SetValue(product, newValue);
                    }

                    var updateResponse = await Task.Run(() => proxy.ProductsUpdate(product));
                    if (updateResponse == null || updateResponse.Content == null)
                    {
                        MessageBox.Show($"A termék frissítése nem sikerült (ID: {bvin})!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    if (updateResponse.Errors.Any())
                    {
                        MessageBox.Show($"Hiba a termék frissítésekor (ID: {bvin}): {string.Join(", ", updateResponse.Errors.Select(x => x.Description))}",
                            "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    if (dataGridView1.Columns.Contains(selectedProperty))
                    {
                        if (selectedProperty == "ProductType")
                        {
                            var type = productTypes?.FirstOrDefault(t => t.Bvin == product.ProductTypeId);
                            dataGridView1.Rows[rowIndex].Cells[selectedProperty].Value = type?.ProductTypeName ?? product.ProductTypeId;
                        }
                        else if (product.GetType().GetProperty(selectedProperty)?.GetValue(product) is decimal decValue)
                        {
                            dataGridView1.Rows[rowIndex].Cells[selectedProperty].Value = decValue.ToString("F0");
                        }
                        else
                        {
                            dataGridView1.Rows[rowIndex].Cells[selectedProperty].Value = product.GetType().GetProperty(selectedProperty)?.GetValue(product)?.ToString();
                        }
                    }
                    updatedCount++;
                }

                if (updatedCount > 0)
                {
                    MessageBox.Show($"{updatedCount} termék tulajdonsága sikeresen frissítve!", "Siker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Egy termék tulajdonsága sem lett frissítve!", "Figyelmeztetés", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Váratlan hiba történt: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private object ConvertValue(string text, Type targetType)
        {
            if (targetType == typeof(string))
                return text;
            if (targetType == typeof(decimal))
                return decimal.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
            if (targetType == typeof(int))
                return int.Parse(text);
            if (targetType == typeof(bool))
                return bool.Parse(text);
            if (targetType == typeof(DateTime))
                return DateTime.Parse(text);
            throw new ArgumentException($"Nem támogatott típus: {targetType.Name}");
        }

        private void comboBoxProperties_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string selectedProperty = comboBoxProperties.SelectedItem?.ToString();
            textBox1.Visible = selectedProperty != "ProductType";

            if (productTypeComboBox != null)
            {
                productTypeComboBox.Visible = selectedProperty == "ProductType";

                if (selectedProperty == "ProductType" && productTypes != null)
                {
                    productTypeComboBox.Items.Clear();
                    productTypeComboBox.Items.AddRange(productTypes.Select(t => t.ProductTypeName).ToArray());
                    if (productTypeComboBox.Items.Count > 0)
                        productTypeComboBox.SelectedIndex = 0;
                }
            }

            if (currentProducts != null && comboBoxProperties.SelectedItem != null)
            {
                DisplayProducts(currentProducts);
            }
        }
    }
}