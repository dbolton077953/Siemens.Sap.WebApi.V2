using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities
{
    public enum SncQualityOfProtection
    {
        None = 0,
        ApplyAuthenticationOnly = 1,
        ApplyIntegrityProtection = 2, //authentication
        ApplyPrivacyProtection = 3, //integrity and authentication
        ApplyDefaultProtection = 8,
        ApplyMaximumProtection = 9
    }
}
