using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities
{
    internal class ApplicationParameters
    {
        private static string _file = string.Empty;
        internal static  bool IsTracingEnabled { get; set; }

        internal static string TraceOutput { get; set; }
        internal static string TraceFile
        {
            get
            {
                string file = TraceOutput;
                if (string.IsNullOrWhiteSpace(file))
                {
                    if (string.IsNullOrWhiteSpace(_file))
                    {
                        string[] items = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < items.Length - 1; i++)
                        {
                            _file += items[i] + "/";
                        }
                        _file += "Nco3_{0}.trc";
                    }
                    file = _file;
                }
                return file;
            }
        }
    }
}
