using System.Configuration;
namespace Kliens_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string apiKey = ConfigurationManager.AppSettings["apikulcs"];
            MessageBox.Show(apiKey);
        }
    }
}
