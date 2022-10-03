using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Siemens.Sap.ERPConnect.Utilities
{
    [Serializable]
    [DataContract]
    public class ExecuteInformation
    {
        public ExecuteInformation()
        {
            RequiresSession = false;
            CommandText = string.Empty;
        }

        [JsonProperty("RFCUser")]
        public string RFCUser { get; set; } = "RFC_GMC";


        [JsonProperty("CommandText")]
        public string CommandText { get; set; }

        [JsonProperty("RequiresSession")]
        public bool RequiresSession { get; set; }

        [JsonProperty("ParameterInformationArray")]
        public ParameterInformation[] ParameterInformationArray { get; set; } = new ParameterInformation[] { };

        [JsonProperty("OutParameterInformationArray")]
        public ParameterInformation[] OutParameterInformationArray { get; set; } = new ParameterInformation[] { };

        [JsonProperty("Where")]
        public WhereClause Where { get; set; } = new WhereClause();

    }
}
