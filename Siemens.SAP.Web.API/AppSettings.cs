using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.WebAPI
{
    public class AppSettings
    {
        public string SAPSys { get; set; }
        public string LicenceKey { get; set; }

        public bool ShowDebugInfo { get; set; } = false;

        public string ErrorLog { get; set; } = @"C:\Temp\SapWebAPIErrs.Log";
       
    }
}
