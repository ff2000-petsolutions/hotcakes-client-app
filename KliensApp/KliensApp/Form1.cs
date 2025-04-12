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

        public Form1()
        {
            InitializeComponent();
            apiKey = ConfigurationManager.AppSettings["apikulcs"];
            apiUrl = ConfigurationManager.AppSettings["apiurl"];
            proxy = new Api(apiUrl, apiKey);
            InitializeComboBox();
        }

        private void InitializeComboBox()
        {
            var properties = typeof(ProductDTO).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.CanRead && p.Name != "Bvin" && p.Name != "StoreId")
                .Select(p => p.Name)
                .OrderBy(name => name)
                .ToList();

            comboBoxProperties.Items.Clear();
            comboBoxProperties.Items.AddRange(properties.ToArray());
            if (properties.Contains("SitePrice"))
                comboBoxProperties.SelectedIndex = properties.IndexOf("SitePrice");
            else if (properties.Count > 0)
                comboBoxProperties.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProducts();
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
                dataGridView1.Columns.Add(selectedProperty, selectedProperty);
            }

            foreach (var product in products)
            {
                var row = new List<object> { product.ProductName, product.Sku, product.Bvin };
                if (!string.IsNullOrEmpty(selectedProperty))
                {
                    var propertyInfo = typeof(ProductDTO).GetProperty(selectedProperty);
                    var value = propertyInfo?.GetValue(product)?.ToString() ?? "";
                    row.Add(value);
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

            string newValueText = textBox1.Text;
            if (string.IsNullOrWhiteSpace(newValueText))
            {
                MessageBox.Show("Kérem adjon meg egy értéket!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                var propertyInfo = typeof(ProductDTO).GetProperty(selectedProperty);
                if (propertyInfo == null)
                {
                    MessageBox.Show($"Érvénytelen tulajdonság: {selectedProperty}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                object newValue;
                try
                {
                    newValue = ConvertValue(newValueText, propertyInfo.PropertyType);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Érvénytelen érték a(z) {selectedProperty} tulajdonsághoz: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var processedRows = new HashSet<int>();
                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    int rowIndex = cell.RowIndex;
                    if (processedRows.Contains(rowIndex))
                        continue;

                    processedRows.Add(rowIndex);
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
                    propertyInfo.SetValue(product, newValue);

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
                        dataGridView1.Rows[rowIndex].Cells[selectedProperty].Value = newValue?.ToString();
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
            if (currentProducts != null && comboBoxProperties.SelectedItem != null)
            {
                DisplayProducts(currentProducts);
            }
        }
    }
}