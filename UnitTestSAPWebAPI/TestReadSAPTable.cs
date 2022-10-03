using Microsoft.Extensions.DependencyInjection;
using Siemens.Sap.WebAPI.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestSAPWebAPI
{
    internal class TestReadSAPTable: UnitTestBase
    {
        protected string _webAPIEndPoint = "https://localhost:7145/api/v1/Rfc/ReadFromSAPTable";

        //protected string _webAPIEndPoint = "https://stage-core.con.siemens.co.uk/Siemens.Sap.WebAPI/api/v1/Rfc/ReadFromSAPTable";
        private App _app;

        [SetUp]
        public void Setup()
        {
            InitHttpClientFactory();

            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            _app = new App(_httpClientFactory, _webAPIEndPoint);
        }

        [Test]
        public void ReadSpecificTable()
        {
            var request = new SAPReadTableRequest()
            {
                MaxRowsToReturn = 15,
                SAPTableFieldNames = new string[3] { "MATNR", "MEINS", "MATKL" },
                SAPTableName = "MARA",
                WhereClause = "MEINS = 'ST'"
            };


            HttpResponseMessage response = _app.ReadSAPTable(request);
            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPReadTableResponse>(apiResponse);


            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            DataTable[] tbls = GetTables(res.Results);

            Assert.IsTrue(tbls.Length == 1);




        }


    }
}
