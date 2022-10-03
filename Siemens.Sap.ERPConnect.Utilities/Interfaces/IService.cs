using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities.Interfaces
{
    internal interface IService
    {
        public SAPCommand GetSAPCommand(ExecuteInformation information);
        public SAPCommand GetSAPCommand(string commandText);
        public DataTable GetReturnInfoAsDataTable(Dictionary<string, string> messages);
        public ReturnInfo GetReturnInfo(Dictionary<string, string> messages);
        public string GetTablesAsJsonDelimitedList(DataSet ds);
        public string ReadSAPTable(string sapTableName, string[] sapTableFields, string whereClause, int maxRows);
        public string DataTableToJSONWithStringBuilder(DataTable table);


    }
}
