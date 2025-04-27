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
using System.Drawing;

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
        private readonly Dictionary<string, string> propertyTranslations;
        private List<string> displayedColumns;
        private string dynamicColumn;
        private readonly ValueConverter converter = new ValueConverter();

        public Form1()
        {

            InitializeComponent();
            apiKey = ConfigurationManager.AppSettings["apikulcs"];
            apiUrl = ConfigurationManager.AppSettings["apiurl"];
            proxy = new Api(apiUrl, apiKey);

            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(1150, 700);


            CustomizeButton(btnGetProducts);
            CustomizeButton(btnSave);
            CustomizeButton(btnAddColumn);
            CustomizeButton(btnRemoveColumn);

            CustomizeComboBox(comboBoxProperties);
            CustomizeTextBox(textBox1);
            CustomizeTextBox(this.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "textBox2"));


            CustomizeDataGridView(dataGridView1);

            propertyTranslations = new Dictionary<string, string>
{
    { "ProductType", "Terméktípus" },
    { "Category", "Kategória" },
    { "SitePrice", "Weboldal ár" },
    { "ListPrice", "Listaár" },
    { "ProductName", "Termék neve" },
    { "Sku", "SKU" },
    { "Bvin", "Azonosító" },
    { "CreationDateUtc", "Létrehozás dátuma" },
    { "LastUpdatedUtc", "Utolsó frissítés" },
    { "AllowReviews", "Vélemények engedélyezése" },
    { "MetaDescription", "Meta leírás" },
    { "MetaKeywords", "Meta kulcsszavak" },
    { "MetaTitle", "Meta cím" },
    { "MinimumQty", "Minimális mennyiség" },
    { "ShortDescription", "Rövid leírás" },
    { "LongDescription", "Hosszú leírás" },
    { "ManufacturerId", "Gyártó azonosító" },
    { "VendorId", "Szállító azonosító" },
    { "ImageFileSmall", "Kis képfájl" },
    { "ImageFileMedium", "Közepes képfájl" },
    { "TaxSchedule", "Adótábla azonosító" },
    { "ShippingMode", "Szállítási mód" },
    { "ShippingWeight", "Szállítási súly" },
    { "ShippingHeight", "Szállítási magasság" },
    { "ShippingWidth", "Szállítási szélesség" },
    { "ShippingLength", "Szállítási hossz" },
    { "Keywords", "Kulcsszavak" },
    { "PreContentColumnId", "Előnézeti oszlop azonosító" },
    { "PostContentColumnId", "Utónézeti oszlop azonosító" },
    { "SiteCost", "Weboldal költség" },
    { "MainImageUrl", "Fő kép URL" },
    { "UrlSlug", "URL slug" },
    { "GiftWrapAllowed", "Ajándékcsomagolás engedélyezése" },
    { "GiftWrapPrice", "Ajándékcsomagolás ára" },
    { "Status", "Állapot" },
    { "InventoryMode", "Készlet mód" },
    { "IsAvailableForSale", "Elérhető eladásra" },
    { "Featured", "Kiemelt" },
    { "HidePrice", "Ár elrejtése" },
    { "TaxExempt", "Adómentes" },
    { "IsNonShipping", "Nem szállítható" },
    { "ExtraShipFee", "Extra szállítási díj" },
    { "ShipSeparately", "Külön szállítandó" },
    { "ShippingSource", "Szállítási forrás" },
    { "ShippingSourceId", "Szállítási forrás azonosító" },
    { "SitePriceOverrideText", "Weboldal ár felülírási szöveg" },
    { "ImageFileSmallAlternateText", "Kis kép alternatív szöveg" },
    { "ImageFileMediumAlternateText", "Közepes kép alternatív szöveg" },
    { "IsSearchable", "Kereshető" },
    { "ShippingCharge", "Szállítási díj" },
    { "AllowUpcharge", "Felár engedélyezése" },
    { "UpchargeAmount", "Felár összege" },
    { "UpchargeUnit", "Felár egysége" },
    { "ShippingDetails", "Szállítási részletek" }
        };

            displayedColumns = new List<string> { "ProductName", "Sku", "Bvin" };
            dynamicColumn = "SitePrice";
            InitializeComboBox();
            InitializeProductTypeComboBox();
            InitializeTextBox2();
            SetupResponsiveLayout();
        }

        private void SetupResponsiveLayout()
        {
            dataGridView1.Location = new Point(10, 170);
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dataGridView1.Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 180);


            btnGetProducts.Location = new Point(10, 10);
            btnGetProducts.Width = 200;
            btnGetProducts.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            btnSave.Location = new Point(10, 50);
            btnSave.Width = 200;
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            btnAddColumn.Location = new Point(10, 90);
            btnAddColumn.Width = 200;
            btnAddColumn.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            btnRemoveColumn.Location = new Point(10, 130);
            btnRemoveColumn.Width = 200;
            btnRemoveColumn.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            label2.Location = new Point(230, 52);

            comboBoxProperties.Location = new Point(320, 50);
            comboBoxProperties.Width = this.ClientSize.Width - 330;
            comboBoxProperties.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            label1.Location = new Point(230, 92);

            textBox1.Location = new Point(320, 90);
            textBox1.Width = this.ClientSize.Width - 330;
            textBox1.Height = 70;
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            label3.Location = new Point(230, 12);

            var textBox2 = this.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Name == "textBox2");
            if (textBox2 != null)
            {
                textBox2.Location = new Point(320, 10);
                textBox2.Width = this.ClientSize.Width - 330;
                textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
        }
        private void CustomizeButton(Button button)
        {
            if (button == null) return;

            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = Color.FromArgb(0, 120, 215);
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 150, 255);
            button.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button.Height = 35;
        }

        private void CustomizeComboBox(ComboBox comboBox)
        {
            if (comboBox == null) return;

            comboBox.BackColor = Color.FromArgb(50, 50, 50);
            comboBox.ForeColor = Color.White;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Segoe UI", 10);
        }

        private void CustomizeTextBox(TextBox textBox)
        {
            if (textBox == null) return;

            textBox.BackColor = Color.FromArgb(50, 50, 50);
            textBox.ForeColor = Color.White;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 10);
        }

        private void CustomizeDataGridView(DataGridView dataGridView)
        {

            dataGridView.BackgroundColor = Color.FromArgb(30, 30, 30);
            dataGridView.ForeColor = Color.White;
            dataGridView.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dataGridView.DefaultCellStyle.ForeColor = Color.White;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;


            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView.ColumnHeadersHeight = 40;
            dataGridView.EnableHeadersVisualStyles = false;


            dataGridView.GridColor = Color.FromArgb(70, 70, 70);
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.RowHeadersVisible = false;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;


            dataGridView.RowTemplate.Height = 30;
        }
        private void InitializeComboBox()
        {
            var properties = typeof(ProductDTO).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.CanRead && p.Name != "Bvin" && p.Name != "StoreId" && p.Name != "ProductTypeId")
                .Select(p => p.Name)
                .OrderBy(name => propertyTranslations.ContainsKey(name) ? propertyTranslations[name] : name)
                .ToList();

            properties.Add("ProductType");
            comboBoxProperties.Items.Clear();
            comboBoxProperties.Items.AddRange(properties.Select(p => propertyTranslations.ContainsKey(p) ? propertyTranslations[p] : p).ToArray());
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
                    Location = new Point(320, 90),
                    Width = this.ClientSize.Width - 330,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Visible = false
                };
                this.Controls.Add(productTypeComboBox);
                CustomizeComboBox(productTypeComboBox);
            }
        }

        private void InitializeTextBox2()
        {
            if (this.Controls.OfType<TextBox>().All(tb => tb.Name != "textBox2"))
            {
                var textBox2 = new TextBox
                {
                    Name = "textBox2",
                    Location = new System.Drawing.Point(textBox1.Location.X, textBox1.Location.Y + textBox1.Height + 40),
                    Width = textBox1.Width
                };
                this.Controls.Add(textBox2);
                textBox2.Enter += textBox2_Enter;
                textBox2.TextChanged += textBox2_TextChanged;
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


            foreach (var column in displayedColumns)
            {
                dataGridView1.Columns.Add(column, propertyTranslations.ContainsKey(column) ? propertyTranslations[column] : column);
            }


            if (!string.IsNullOrEmpty(dynamicColumn))
            {
                dataGridView1.Columns.Add(dynamicColumn, propertyTranslations.ContainsKey(dynamicColumn) ? propertyTranslations[dynamicColumn] : dynamicColumn);
            }

            foreach (var product in products)
            {
                var row = new List<object>();

                foreach (var column in displayedColumns)
                {
                    if (column == "ProductType" && productTypes != null)
                    {
                        var type = productTypes.FirstOrDefault(t => t.Bvin == product.ProductTypeId);
                        row.Add(type?.ProductTypeName ?? product.ProductTypeId);
                    }
                    else if (column == "Bvin")
                    {
                        row.Add(product.Bvin);
                    }
                    else
                    {
                        var propertyInfo = typeof(ProductDTO).GetProperty(column);
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


                if (!string.IsNullOrEmpty(dynamicColumn))
                {
                    if (dynamicColumn == "ProductType" && productTypes != null)
                    {
                        var type = productTypes.FirstOrDefault(t => t.Bvin == product.ProductTypeId);
                        row.Add(type?.ProductTypeName ?? product.ProductTypeId);
                    }
                    else
                    {
                        var propertyInfo = typeof(ProductDTO).GetProperty(dynamicColumn);
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

        private async void textBox2_Enter(object sender, EventArgs e)
        {
            await LoadProducts();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (currentProducts == null || !this.Controls.OfType<TextBox>().Any(tb => tb.Name != "textBox2"))
                return;

            string searchText = this.Controls.OfType<TextBox>().First(tb => tb.Name == "textBox2").Text.Trim().ToLower();

            var filteredProducts = currentProducts.Where(p =>
            {
                bool matches = false;
                foreach (var column in displayedColumns.Concat(new[] { dynamicColumn }).Where(c => !string.IsNullOrEmpty(c)))
                {
                    if (column == "ProductType" && productTypes != null)
                    {
                        var type = productTypes.FirstOrDefault(t => t.Bvin == p.ProductTypeId);
                        matches |= type?.ProductTypeName?.ToLower().Contains(searchText) == true;
                    }
                    else if (column == "Bvin")
                    {
                        matches |= p.Bvin?.ToLower().Contains(searchText) == true;
                    }
                    else
                    {
                        var propertyInfo = typeof(ProductDTO).GetProperty(column);
                        var value = propertyInfo?.GetValue(p);
                        if (value is decimal decValue)
                        {
                            matches |= decValue.ToString("F0").Contains(searchText);
                        }
                        else
                        {
                            matches |= value?.ToString()?.ToLower().Contains(searchText) == true;
                        }
                    }
                }
                return matches;
            }).ToList();

            DisplayProducts(filteredProducts);
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Biztosan menti a változtatásokat?", "Mentés megerősítése", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;

            string selectedProperty = comboBoxProperties.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedProperty))
            {
                MessageBox.Show("Kérem válasszon ki egy tulajdonságot!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string newValueText = selectedProperty == propertyTranslations["ProductType"] ? productTypeComboBox.SelectedItem?.ToString() : textBox1.Text;
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

                    if (selectedProperty == propertyTranslations["ProductType"])
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
                        var propertyInfo = typeof(ProductDTO).GetProperty(propertyTranslations.FirstOrDefault(x => x.Value == selectedProperty).Key);
                        if (propertyInfo == null)
                        {
                            MessageBox.Show($"Érvénytelen tulajdonság: {selectedProperty}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        object newValue;
                        try
                        {
                            newValue = converter.Convert(newValueText, propertyInfo.PropertyType);
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

                    if (dataGridView1.Columns.Contains(propertyTranslations.FirstOrDefault(x => x.Value == selectedProperty).Key))
                    {
                        string selectedPropertyKey = propertyTranslations.FirstOrDefault(x => x.Value == selectedProperty).Key;
                        if (selectedProperty == propertyTranslations["ProductType"])
                        {
                            var type = productTypes?.FirstOrDefault(t => t.Bvin == product.ProductTypeId);
                            dataGridView1.Rows[rowIndex].Cells[selectedPropertyKey].Value = type?.ProductTypeName ?? product.ProductTypeId;
                        }
                        else if (product.GetType().GetProperty(selectedPropertyKey)?.GetValue(product) is decimal decValue)
                        {
                            dataGridView1.Rows[rowIndex].Cells[selectedPropertyKey].Value = decValue.ToString("F0");
                        }
                        else
                        {
                            dataGridView1.Rows[rowIndex].Cells[selectedPropertyKey].Value = product.GetType().GetProperty(selectedPropertyKey)?.GetValue(product)?.ToString();
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


        private void comboBoxProperties_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string selectedProperty = comboBoxProperties.SelectedItem?.ToString();
            textBox1.Visible = selectedProperty != propertyTranslations["ProductType"];

            if (productTypeComboBox != null)
            {
                productTypeComboBox.Visible = selectedProperty == propertyTranslations["ProductType"];

                if (selectedProperty == propertyTranslations["ProductType"] && productTypes != null)
                {
                    productTypeComboBox.Items.Clear();
                    productTypeComboBox.Items.AddRange(productTypes.Select(t => t.ProductTypeName).ToArray());
                    if (productTypeComboBox.Items.Count > 0)
                        productTypeComboBox.SelectedIndex = 0;
                }
            }

            if (currentProducts != null && selectedProperty != null)
            {
                dynamicColumn = propertyTranslations.FirstOrDefault(x => x.Value == selectedProperty).Key ?? selectedProperty;
                DisplayProducts(currentProducts);
            }
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            using (var form = new Form
            {
                Text = "Oszlop hozzáadása",
                Size = new System.Drawing.Size(300, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                var comboBox = new ComboBox
                {
                    Location = new System.Drawing.Point(20, 20),
                    Width = 240,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                var availableProperties = typeof(ProductDTO).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && !displayedColumns.Contains(p.Name))
                    .Select(p => p.Name)
                    .OrderBy(name => propertyTranslations.ContainsKey(name) ? propertyTranslations[name] : name)
                    .ToList();

                availableProperties.Add("ProductType");
                comboBox.Items.AddRange(availableProperties.Select(p => propertyTranslations.ContainsKey(p) ? propertyTranslations[p] : p).ToArray());

                if (comboBox.Items.Count == 0)
                {
                    MessageBox.Show("Nincs több hozzáadható oszlop!", "Információ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                comboBox.SelectedIndex = 0;

                var btnOk = new Button
                {
                    Text = "Hozzáadás",
                    Location = new System.Drawing.Point(20, 60),
                    DialogResult = DialogResult.OK
                };

                var btnCancel = new Button
                {
                    Text = "Mégse",
                    Location = new System.Drawing.Point(100, 60),
                    DialogResult = DialogResult.Cancel
                };

                form.Controls.Add(comboBox);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string selectedProperty = comboBox.SelectedItem.ToString();
                    string propertyKey = propertyTranslations.FirstOrDefault(x => x.Value == selectedProperty).Key ?? selectedProperty;
                    if (!displayedColumns.Contains(propertyKey))
                    {
                        displayedColumns.Add(propertyKey);
                        DisplayProducts(currentProducts);
                    }
                }
            }
        }

        private void btnRemoveColumn_Click(object sender, EventArgs e)
        {
            using (var form = new Form
            {
                Text = "Oszlop eltávolítása",
                Size = new System.Drawing.Size(300, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                var comboBox = new ComboBox
                {
                    Location = new System.Drawing.Point(20, 20),
                    Width = 240,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                var removableColumns = displayedColumns
                    .Where(c => c != "ProductName" && c != "Sku" && c != "Bvin")
                    .OrderBy(c => propertyTranslations.ContainsKey(c) ? propertyTranslations[c] : c)
                    .ToList();

                comboBox.Items.AddRange(removableColumns.Select(c => propertyTranslations.ContainsKey(c) ? propertyTranslations[c] : c).ToArray());

                if (comboBox.Items.Count == 0)
                {
                    MessageBox.Show("Nincs eltávolítható oszlop!", "Információ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                comboBox.SelectedIndex = 0;

                var btnOk = new Button
                {
                    Text = "Eltávolítás",
                    Location = new System.Drawing.Point(20, 60),
                    DialogResult = DialogResult.OK
                };

                var btnCancel = new Button
                {
                    Text = "Mégse",
                    Location = new System.Drawing.Point(100, 60),
                    DialogResult = DialogResult.Cancel
                };

                form.Controls.Add(comboBox);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string selectedProperty = comboBox.SelectedItem.ToString();
                    string propertyKey = propertyTranslations.FirstOrDefault(x => x.Value == selectedProperty).Key ?? selectedProperty;
                    if (displayedColumns.Contains(propertyKey))
                    {
                        displayedColumns.Remove(propertyKey);
                        DisplayProducts(currentProducts);
                    }
                }
            }
        }
    }
}