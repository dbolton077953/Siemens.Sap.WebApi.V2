using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Siemens.Sap.WebAPI.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace UnitTestSAPWebAPI
{
    internal class App
    {



        private string _webAPIEndPoint;

        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _cf;

        public App(IHttpClientFactory httpCF, string endPointURL)
        {
            _cf = httpCF;
            _webAPIEndPoint = endPointURL;
        }


        public HttpResponseMessage GetInfoFromSAP(SAPCommandRequest request)
        {
            string json = JsonConvert.SerializeObject (request);
            return CallEndPoint(json, _webAPIEndPoint);
        }


        public HttpResponseMessage ReadSAPTable(SAPReadTableRequest request)
        {
            string json = JsonConvert.SerializeObject(request);
            return CallEndPoint(json, _webAPIEndPoint);
        }

        private HttpResponseMessage CallEndPoint(string json, string endPoint)
        {

            using (var client = _cf.CreateClient(_webAPIEndPoint))
            {

                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(endPoint, UriKind.Absolute),
                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")

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
                catch (Exception)
                {
                    throw;
                }

            }

        }


    }
}
