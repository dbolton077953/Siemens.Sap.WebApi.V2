using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Siemens.Sap.ERPConnect.Utilities
{
    [Serializable]
    [DataContract]
    public class SAPParameter
    {
        public SAPParameter()
        {
            Name = string.Empty;
            Value = default(Object);
        }
        [JsonProperty("Name")]
        public string Name { get; set; } = string.Empty;
     
        [JsonProperty("Value")]
        public object Value { get; set; } = string.Empty;
    }
}
