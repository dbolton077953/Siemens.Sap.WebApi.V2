using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Siemens.Sap.ERPConnect.Utilities
{
    [Serializable]
    [DataContract]

    public enum WhereConditionType
    {
        EQUALS = 0,
        LIKE = 1,
        NOTEQUAL = 2,
        GREATERTHAN = 3,
        LESSTHAN = 4,
        STARTSWITH = 5,
        NONE = 6
    }
}
