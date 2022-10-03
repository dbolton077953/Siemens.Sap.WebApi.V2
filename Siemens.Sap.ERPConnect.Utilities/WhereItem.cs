using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Siemens.Sap.ERPConnect.Utilities
{ 
    [Serializable]
    [DataContract]
    public class WhereItem
    {
        public WhereItem()
        {
            Name = String.Empty;
            Value = String.Empty;
        }
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Condition")]
        public WhereConditionType Condition { get; set; }
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}
