using Newtonsoft.Json;
using Siemens.Sap.ERPConnect.Utilities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Siemens.Sap.WebAPI.Common.Models
{
    [Serializable]
    [DataContract]
    public class SAPCommandRequest
    {
        public SAPCommandRequest()
        {

        }

    
        [JsonProperty("saprfcInformation")]
        [JsonPropertyName("saprfcInformation")]
        public ExecuteInformation SAPRFCInformation { get; set; }

 
    }
}
