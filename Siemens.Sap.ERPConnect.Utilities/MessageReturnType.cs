using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities
{

        public enum MessageReturnType
        {
            [Description("None")]
            N,
            [Description("Success")]
            S,
            [Description("Information")]
            I,
            [Description("Error")]
            E,
            [Description("Warning")]
            W,
            [Description("Abort")]
            A
        }

}
