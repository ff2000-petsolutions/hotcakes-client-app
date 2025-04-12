using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Windows.Forms;

namespace Kliens_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static string apiKey;
        private static string apiUrl;

        private void Form1_Load(object sender, EventArgs e)
        {
            apiKey = ConfigurationManager.AppSettings["apikulcs"];
            apiUrl = ConfigurationManager.AppSettings["apiurl"];

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
                var products = await GetProducts(apiUrl, apiKey);

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("ProductName", "Termék neve");
                dataGridView1.Columns.Add("Sku", "SKU");
                dataGridView1.Columns.Add("Bvin", "Bvin");

                foreach (var product in products)
                {
                    dataGridView1.Rows.Add(product.ProductName, product.Sku, product.Bvin);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt: " + ex.Message);
            }
        }

        private async Task<List<Product>> GetProducts(string apiUrl, string apiKey)
        {
            string endpoint = $"DesktopModules/Hotcakes/API/rest/v1/products?key={apiKey}";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(apiUrl + endpoint);
                var json = JObject.Parse(response);
                var products = new List<Product>();

                foreach (var item in json["Content"]["Products"])
                {
                    products.Add(new Product
                    {
                        ProductName = item["ProductName"]?.ToString(),
                        Sku = item["Sku"]?.ToString(),
                        Bvin = item["Bvin"]?.ToString()
                    });
                }

                return products;
            }
        }

        public class Product
        {
            public string ProductName { get; set; }
            public string Sku { get; set; }
            public string Bvin { get; set; }
        }
    }
}
