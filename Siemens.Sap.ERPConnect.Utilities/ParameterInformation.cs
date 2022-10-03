using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Siemens.Sap.ERPConnect.Utilities
{
    [Serializable]
    [DataContract]
    public class ParameterInformation
    {
        public ParameterInformation()
        {
            
            ContainerOrdinalPosition = -1;
            ContainerType = ContainerType.Function;
            ContainerName = string.Empty;
            Parameters = new List<SAPParameter>(); 
        }
        [JsonProperty("ContainerType")]
        public ContainerType ContainerType { get; set; }
        [JsonProperty("ContainerName")]
        public string ContainerName { get; set; }
        [JsonProperty("Parameters")]
        public List<SAPParameter> Parameters { get; set; }

        [JsonProperty("ContainerOrdinalPosition")]
        public int ContainerOrdinalPosition { get; set; }

    }
}
