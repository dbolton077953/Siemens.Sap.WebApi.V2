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
        public void ReadCAUFVTable()
        {
            var request = new SAPReadTableRequest()
            {
                MaxRowsToReturn = 3000,
                SAPTableFieldNames = new string[7] { "MANDT","STLBEZ", "AUFNR","IDAT3","LOEKZ","GAMNG","IGMNG"},
                SAPTableName = "CAUFV",
                WhereClause = "STLBEZ EQ 'A5E31408304SM' AND LOEKZ = ''"
            };


            HttpResponseMessage response = _app.ReadSAPTable(request);
            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPReadTableResponse>(apiResponse);


            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            DataTable[] tbls = GetTables(res.Results);

            Assert.IsTrue(tbls.Length == 1);




        }


        [Test]
        public void ReadcaufvdgetTable()
        {
            var request = new SAPReadTableRequest()
            {
                MaxRowsToReturn = 3000,
                SAPTableFieldNames = new string[1] { "AUFNR"},
                SAPTableName = "CAUFVDGET",
                WhereClause = "AUFNR EQ '900001444002'"
                //WhereClause = "AUFNR = '900001444002'"
            };


            HttpResponseMessage response = _app.ReadSAPTable(request);
            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPReadTableResponse>(apiResponse);


            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            DataTable[] tbls = GetTables(res.Results);

            Assert.IsTrue(tbls.Length == 1);




        }



        [Test]
        public void ReadAFPOTable()
        {
            var request = new SAPReadTableRequest()
            {
                MaxRowsToReturn = 3000,
                SAPTableFieldNames = new string[6] { "MATNR","AUFNR","PSMNG","WEMNG","DAUAT","DNREL" },
                SAPTableName = "AFPO",
                WhereClause = "MATNR EQ 'A5E31408304SM' AND DNREL <> 'X'"
                //WhereClause = "AUFNR = '900001444002'"
            };


            HttpResponseMessage response = _app.ReadSAPTable(request);
            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPReadTableResponse>(apiResponse);


            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            DataTable[] tbls = GetTables(res.Results);

            List<OpenOrdersMessage> orders = new List<OpenOrdersMessage>();
            foreach (DataRow r in tbls[0].Rows)
            {
                orders.Add(new OpenOrdersMessage()
                {
                     
                    MaterialNumber = r["MATNR"].ToString(),
                    OrderNumber = r["AUFNR"].ToString(),
                    TargetQty = Convert.ToDecimal(r["PSMNG"]),
                    ConfirmedQty = Convert.ToDecimal(r["WEMNG"])
                });

            }

            var totalTargetQty = orders.Sum(o => o.TargetQty);

            var totalConfirmedQty = orders.Sum(o => o.ConfirmedQty);


            var outstandingQty = totalTargetQty - totalConfirmedQty;



            Assert.IsTrue(tbls.Length == 1);




        }



    }
  
}
