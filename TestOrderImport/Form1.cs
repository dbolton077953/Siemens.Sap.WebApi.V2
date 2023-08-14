using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.ApplicationServices;
using System.Data;
using System.Net;

namespace TestOrderImport
{
    public partial class Form1 : Form
    {

        private ServiceProvider serviceProvider;
        private string _webAPIEndPoint;

        public Form1()
        {
            InitializeComponent();
            EndPointDDL.SelectedIndex = 0;
        }

        private void InitHttpClientFactory()
        {
            if (serviceProvider == null)
            {

                // Create credentials cache of potential end points
                var credentialsCache = new CredentialCache();

                var uri = new Uri(_webAPIEndPoint);
                credentialsCache.Add(uri, "NTLM", CredentialCache.DefaultNetworkCredentials);

                var service = new ServiceCollection().AddHttpClient(_webAPIEndPoint, c =>
                {
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-MessageDispatcherNsc");
                    c.DefaultRequestHeaders.Add("Connection", "keep-alive");



                }).ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler()
                    {
                        //UseDefaultCredentials = true,
                        Credentials = credentialsCache,
                        PreAuthenticate = true,
                        AllowAutoRedirect = true,
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }

                    };
                });

                serviceProvider = service.Services.BuildServiceProvider();
            }




        }

        private void GetBtn_Click(object sender, EventArgs e)
        {
            InitHttpClientFactory();
            GetOrderOperations();
        }

        private void GetOrderOperations()
        {
            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            App app = new App(_httpClientFactory, _webAPIEndPoint);

            DataTable[] tbls = app.GetOrders(OrderNumberTextBox.Text.Trim());

            if (tbls != null && tbls.Length > 0)
            {

                this.OrdersGridView.DataSource = tbls[1].DefaultView;
                this.OrdersGridView.Refresh();

                this.OperationsGridView.DataSource = tbls[0].DefaultView;
                this.OperationsGridView.Refresh();
            }
            else
            {
                MessageBox.Show("Order does mot exist or contains no data.", "Warning");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void EndPointDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            _webAPIEndPoint = EndPointDDL.SelectedItem.ToString();
        }
    }
}