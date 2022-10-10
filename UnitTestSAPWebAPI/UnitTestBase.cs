using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestSAPWebAPI
{
    internal class UnitTestBase
    {

        protected ServiceProvider serviceProvider;
        protected string _webAPIEndPoint = "https://localhost:7145/api/v1/Rfc/CallRFC";
        //protected string _webAPIEndPoint = "https://stage-core.con.siemens.co.uk/Siemens.Sap.WebAPI/api/v1/Rfc/CallRFC";

        protected void InitHttpClientFactory()
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



        protected DataTable[] GetTables(string  t)
        {
            DataTable[] tbls = null;

            var tables = t.Replace("\\", "");

            string[] arrayTables = tables.Split("@@");

            tbls = new DataTable[arrayTables.Length];
            int idx = 0;

            foreach (string s in arrayTables)
            {

                int startPos = s.IndexOf("[");
                int endPos = s.IndexOf("]");

                string json = s.Substring(startPos, endPos - startPos + 1);

                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(json, (typeof(DataTable)));


                DataTable newTable = new DataTable();
                newTable = dt.Copy();

                tbls[idx++] = newTable;
            }

            return tbls;
        }

    }
}
