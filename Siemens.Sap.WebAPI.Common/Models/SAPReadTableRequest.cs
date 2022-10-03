using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.WebAPI.Common.Models
{
    [Serializable]
    [DataContract]
    public class SAPReadTableRequest
    {
        public SAPReadTableRequest()
        {

        }

        [JsonProperty("SAPTableName")]
        public string SAPTableName { get; set; } = string.Empty;

        [JsonProperty("SAPTableFieldNames")]
        public string[] SAPTableFieldNames { get; set; }

        [JsonProperty("MaxRowsToReturn")]
        public int MaxRowsToReturn { get; set; } = 0;

        [JsonProperty("WhereClause")]
        public string WhereClause { get; set; } = string.Empty;


    }
}
