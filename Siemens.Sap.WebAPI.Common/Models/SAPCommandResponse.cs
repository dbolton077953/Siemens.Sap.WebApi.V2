using Newtonsoft.Json;
using Siemens.Sap.ERPConnect.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Siemens.Sap.WebAPI.Common.Models
{
    [Serializable]
    [DataContract]
    public class SAPCommandResponse
    {
        public SAPCommandResponse()
        {

        }



        [JsonProperty("outParams")]
        [JsonPropertyName("outParams")]
        public SAPParameter[] OutParams { get; set; }


        [JsonProperty("tables")]
        [JsonPropertyName("tables")]
        public string Tables { get; set; }
      

   
    }
}
