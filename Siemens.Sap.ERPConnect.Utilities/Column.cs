using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities
{
    public class Column
    {
        public Column()
        {
            Direction = string.Empty;
        }

        public string Name { get; set; }
        public string Direction { get; set; }
        public string DataType { get; set; }
    }
}
