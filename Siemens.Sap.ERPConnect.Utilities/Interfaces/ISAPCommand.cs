using ERPConnect;
using StrongInterop.ADODB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities.Interfaces
{
    internal interface ISAPCommand
    {
        public DataSet ExecuteDataSet(ExecuteInformation information);
        public DataSet ExecuteDataSet();

        public SAPDataReader ExecuteReader(string tableName = null);

        public Recordset[] ExecuteRecordset();

        public RFCTable GetTable(string tableName);
        public Recordset GetReturnStructureAsRecordset();

        public Recordset GetRecordset(string tableName);

        public Function GetFunctionStructure();

        public Recordset ExecuteRecordset(string tableName);

        public bool ExecuteRequest();

        public bool ExecuteRequest(ExecuteInformation information);

        public void Close();
     

    }
}
