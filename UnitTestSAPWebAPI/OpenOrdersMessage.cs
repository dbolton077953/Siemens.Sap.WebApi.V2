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
    public class OpenOrdersMessage
    {
    


            [JsonProperty("AUFNR")]
            [JsonPropertyName("AUFNR")]
            public string OrderNumber { get; set; }

            [JsonProperty("MATNR")]
            [JsonPropertyName("MATNR")]
            public string MaterialNumber { get; set; }

            [JsonProperty("PSMNG")]
            [JsonPropertyName("PSMNG")]
            public decimal TargetQty { get; set; }


            [JsonProperty("WEMNG")]
            [JsonPropertyName("WEMNG")]
            public decimal ConfirmedQty { get; set; }



      
    }
}
