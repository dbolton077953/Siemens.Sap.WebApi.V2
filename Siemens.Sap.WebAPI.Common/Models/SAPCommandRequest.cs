using Newtonsoft.Json;
using Siemens.Sap.ERPConnect.Utilities;
using System.Runtime.Serialization;

namespace Siemens.Sap.WebAPI.Common.Models
{
    [Serializable]
    [DataContract]
    public class SAPCommandRequest
    {
        public SAPCommandRequest()
        {

        }

    
        [JsonProperty("SAPRFCInformation")]
        public ExecuteInformation SAPRFCInformation { get; set; }

 
    }
}
