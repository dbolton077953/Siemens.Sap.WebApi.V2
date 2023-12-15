using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UnitTestSAPWebAPI
{
    [Serializable]
    
    public class OrdersForMaterialMessage
    {
        [JsonProperty("ORDER_NUMBER")]
        [JsonPropertyName("ORDER_NUMBER")]
        public string OrderNumber { get; set; }

        [JsonProperty("MATERIAL")]
        [JsonPropertyName("MATERIAL")]
        public string MaterialNumber { get; set; }

        [JsonProperty("SYSTEM_STATUS")]
        [JsonPropertyName("SYSTEM_STATUS")]
        public string SystemStatus { get; set; }

        [JsonProperty("TARGET_QUANTITY")]
        [JsonPropertyName("TARGET_QUANTITY")]
        public decimal TargetQty { get; set; }


        [JsonProperty("CONFIRMNED_QUANTITY")]
        [JsonPropertyName("CONFIRMED_QUANTITY")]
        public decimal ConfirmedQty { get; set; }


  
    }
}
