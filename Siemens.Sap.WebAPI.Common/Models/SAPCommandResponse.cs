using Microsoft.AspNetCore.Mvc;
using Siemens.Sap.ERPConnect.Utilities;
using System.Data;
using System.Runtime.Serialization;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Siemens.Sap.WebAPI.Common.Models
{
    [Serializable]
    [DataContract]
    public class SAPCommandResponse
    {
        public SAPCommandResponse()
        {

        }



        [JsonProperty("OutParams")]
        public SAPParameter[] OutParams { get; set; }


        [JsonProperty("Tables")]
        public string Tables { get; set; }
      

   
    }
}
