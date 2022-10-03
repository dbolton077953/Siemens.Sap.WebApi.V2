using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Siemens.Sap.ERPConnect.Utilities
{
    [Serializable]
    [DataContract]
    public class WhereClause
    {
        public WhereClause()
        {
            this.ContainerType = ContainerType.Table;
            ContainerName = String.Empty;
            Items = new List<WhereItem>();
        }
        [JsonProperty("ContainerType")]
        public ContainerType ContainerType { get; private set; } = ContainerType.Function;

        [JsonProperty("ContainerName")]
        public string ContainerName { get; set; } = String.Empty;

        [JsonProperty("Items")]
        public List<WhereItem> Items { get; set; }= new List<WhereItem>();
    }
}
