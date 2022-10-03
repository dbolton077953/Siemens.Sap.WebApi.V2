using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities.Interfaces
{
    public interface ISncConfiguration : IConfiguration
    {
        string SncLibraryPath { get; set; }
        string SncPartnerName { get; set; }
        string SncMyName { get; set; }
        SncMode SncMode { get; set; }
        SncQualityOfProtection SncQoP { get; set; }

    }
}
