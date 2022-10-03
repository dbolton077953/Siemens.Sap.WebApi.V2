using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.WebAPI.Common.Models
{
    public class SAPReadTableResponse
    {
        [JsonProperty("Results")]
        public string Results { get; set; }
    }
}
