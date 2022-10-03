using System.Runtime.Serialization;

namespace Siemens.Sap.ERPConnect.Utilities
{
    [Serializable]
    [DataContract]
    public enum ContainerType
    {
        Function = 0,
        Structure = 1,
        Table = 2
    }
}
