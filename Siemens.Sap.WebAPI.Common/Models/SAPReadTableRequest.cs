using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Siemens.Sap.WebAPI.Common.Models
{
    [Serializable]
    [DataContract]
    public class SAPReadTableRequest
    {
        public SAPReadTableRequest()
        {

        }

        [JsonProperty("sapTableName")]
        [JsonPropertyName("sapTableName")]
        public string SAPTableName { get; set; } = string.Empty;

        [JsonProperty("sapTableFieldNames")]
        [JsonPropertyName("sapTableFieldNames")]
        public string[] SAPTableFieldNames { get; set; }

        [JsonProperty("maxRowsToReturn")]
        [JsonPropertyName("maxRowsToReturn")]
        public int MaxRowsToReturn { get; set; } = 0;

        [JsonProperty("whereClause")]
        [JsonPropertyName("whereClause")]
        public string WhereClause { get; set; } = string.Empty;


    }
}
