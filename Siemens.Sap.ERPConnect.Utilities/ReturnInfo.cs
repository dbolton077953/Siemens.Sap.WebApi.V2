using System.Data;
using System.Runtime.Serialization;

namespace Siemens.Sap.ERPConnect.Utilities
{
    [DataContract]
    public class ReturnInfo
    {
        [DataMember]
        public DataTable Data { get; set; }

        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string MessageV1 { get; set; }
        [DataMember]
        public string MessageV2 { get; set; }
        [DataMember]
        public string MessageV3 { get; set; }
        [DataMember]
        public string MessageV4 { get; set; }
        [DataMember]
        public string Number { get; set; }
        [DataMember]
        public bool OperationOK { get; set; }

        [DataMember]
        public string Code { get; set; }
    }
}