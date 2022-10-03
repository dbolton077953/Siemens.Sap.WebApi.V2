using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities.Interfaces
{
    public interface IConfiguration
    {
        string Name { get; set; }
        string User { get; set; }
        string Password { get; set; }
        string Client { get; set; }
        string Language { get; set; }
        int MaxPoolSize { get; set; }
        int IdleTimeout { get; set; }
        string Host { get; set; }
        HostType HostType { get; set; }
        string SystemNumber { get; set; }
        bool Logging { get; set; }
        string LogDir { get; set; }
        string SystemId { get; set; }
        string Group { get; set; }
    }
}
