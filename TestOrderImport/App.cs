//using Newtonsoft.Json;
using Siemens.Sap.ERPConnect.Utilities;
using Siemens.Sap.WebAPI.Common.Models;
using System.Data;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace TestOrderImport
{
    public  class App
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _cf;
        private string _webAPIEndPoint;
        public App(IHttpClientFactory httpCF, string endPointURL )
        {
            _cf = httpCF;
            _webAPIEndPoint = endPointURL;

            
        }


        public DataTable[] GetOrders(string orderNumber)
        {

            DataTable[] tbls =null;

            try
            {
               HttpResponseMessage result = GetOrderOperations(new SAPCommandRequest()
                {
                   SAPRFCInformation = new ExecuteInformation()
                   {
                       RFCUser="RFC_CPO",
                       CommandText = "/SIE/AD_Z1PC_READ_PROD_POR_MTO",
                       ParameterInformationArray = new ParameterInformation[]
                        {
                            new ParameterInformation()
                            {
                                 ContainerName = "I_AUFNR",
                                 Parameters = new List<SAPParameter>(new SAPParameter[]
                                 {
                                    new SAPParameter { Name = "I_AUFNR", Value = orderNumber }
                                 })
                            }
                        }
                   }
                });

                if (result.StatusCode == HttpStatusCode.OK)
                {

                    string apiResponse = result.Content.ReadAsStringAsync().Result;

                    var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPCommandResponse>(apiResponse);


                    var tables = res.Tables.Replace("\\", "");

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

                   
                }


                return tbls;

            }
            catch (Exception)
            {
                throw;
            }
        }

        //public static object DeserializeFromStream(Stream stream)
        //{
        //    var serializer = new Newtonsoft.Json.JsonSerializer();

        //    using (var sr = new StreamReader(stream))
        //    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(sr))
        //    {
        //        return serializer.Deserialize(jsonTextReader);
        //    }
        //}




        private HttpResponseMessage GetOrderOperations(SAPCommandRequest request)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(request);
            return CallEndPoint(json,_webAPIEndPoint);
        }


        private HttpResponseMessage CallEndPoint(string json, string endPoint)
        {

            using (var client = _cf.CreateClient(_webAPIEndPoint))
            {

                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(endPoint, UriKind.Absolute),
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                    

                };

                request.Version = HttpVersion.Version10;
                

                try
                {
                    var response = client.SendAsync(request).Result;

 

                    return response;
                }
                catch (IOException io)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
               
            }

        }


    }
}
