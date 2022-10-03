using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities
{
    public class Function : DataStructure
    {
        public Table[] Tables { get; set; }
        public Structure[] Structures { get; set; }
    }
}
