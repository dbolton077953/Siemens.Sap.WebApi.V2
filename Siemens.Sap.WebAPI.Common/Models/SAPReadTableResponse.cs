using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Siemens.Sap.WebAPI.Common.Models
{
    public class SAPReadTableResponse
    {
        [JsonProperty("results")]
        [JsonPropertyName("results")]
        public string Results { get; set; }
    }
}
