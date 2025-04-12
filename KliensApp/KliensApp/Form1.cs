using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Catalog;
using System.Threading.Tasks;
using System.Linq;

namespace KliensApp
{
    public partial class Form1 : Form
    {
        private readonly string apiKey;
        private readonly string apiUrl;
        private readonly Api proxy;

        public Form1()
        {
            InitializeComponent();
            apiKey = ConfigurationManager.AppSettings["apikulcs"];
            apiUrl = ConfigurationManager.AppSettings["apiurl"];
            proxy = new Api(apiUrl, apiKey);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private async void btnGetProducts_Click(object sender, EventArgs e)
        {
            await LoadProducts();
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
                DisplayProducts(products.Content);
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
            dataGridView1.Columns.Add("ListPrice", "Ár");

            foreach (var product in products)
            {
                dataGridView1.Rows.Add(product.ProductName, product.Sku, product.Bvin, product.ListPrice);
            }

            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
    }
}