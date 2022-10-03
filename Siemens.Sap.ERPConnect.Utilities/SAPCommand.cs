using System.Data;
using System.Diagnostics;
using StrongInterop.ADODB;
using System.Reflection;
using System.Text;
using ERPConnect;
using Siemens.Sap.ERPConnect.Utilities.Interfaces;
using ERPConnect.Utils;

namespace Siemens.Sap.ERPConnect.Utilities
{
    [Serializable]
    public class SAPCommand : IDisposable,ISAPCommand
    {
        private bool _disposed;
        private RFCFunction _function;
        RFCFunction _commit;
        private bool _isExecuteCalled = false;
        private string _traceName = string.Empty;
        private SAPConnectionSettings _settings;
        private bool _isTracingEnabled = false;


        public SAPCommand(SAPConnectionSettings settings)
        {

            _settings = settings;

            if (_settings.EnableTracing)
            {
                EnableTracing(_settings.TraceFile);
            }

            ReturnType = MessageReturnType.N;
            Out = new List<SAPParameter>();
        }

        public SAPCommand(SAPConnectionSettings settings, string cmdText)
            : this(settings)
        {
            if (string.IsNullOrWhiteSpace(cmdText))
                throw new ArgumentException("cmdText cannot be null or empty");
            AppendToTraceFile(string.Format("SAPCommand Constructor, Command Text: {0}", cmdText));
            CommandText = cmdText;
        }

        public SAPCommand(SAPConnectionSettings settings, string cmdText, R3Connection connection)
            : this(settings, cmdText)
        {
            if (connection == null) 
                throw new ArgumentNullException("connection");
            AppendToTraceFile("SAPCommand Constructor, Connection");
            this.Connection = connection;
        }


        private void EnableTracing(string traceFile)
        {
            _traceName = string.Format("{0}_{1}", Guid.NewGuid().ToString().Replace("-", string.Empty), Thread.CurrentThread.ManagedThreadId.ToString());

            Trace.Listeners.Add(new TextWriterTraceListener(string.Format(traceFile, _traceName), _traceName));

            _isTracingEnabled = true;
        }

        public R3Connection Connection { get; private set; }

        public string CommandText { get; private set; }

        public Dictionary<string, string> Messages { get; private set; }

        public ExecuteInformation ExecuteInformation { get; set; }

        public MessageReturnType ReturnType { get; protected set; }

        public List<SAPParameter> Out { get; private set; }

        public bool Success { get; private set; }


        public DataSet ExecuteDataSet(ExecuteInformation information)
        {
            ExecuteInformation = information;
            AppendToTraceFile( "SAPCommand ExecuteDataset(ExecuteInformation)");
            return ExecuteDataSet();
        }

        /// <summary>
        /// ExecuteDataSet(): Calls RFC with the specified import and/or table arguments
        /// Tested: 12.09.22
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteDataSet()
        {
            AppendToTraceFile( "SAPCommand ExecuteDataset()");
            Execute();
            if (Success)
            {
                AppendToTraceFile( string.Format("SAPCommand ExecuteDataset(), Success: {0}, returning dataset", Success));
                try
                {

                    if (ExecuteInformation.Where.Items.Count>0)
                    {
                        string[] columns = (from pi in ExecuteInformation.ParameterInformationArray.Where(pi => pi.ContainerName == "FIELDS")
                                            from p in pi.Parameters
                                            select p.Value.ToString()).ToArray();
                        ParameterInformation delimiter =
                            ExecuteInformation.ParameterInformationArray.Where(pi => pi.ContainerType == ContainerType.Function)
                            .FirstOrDefault();

                        return _function.ToDotNetDataSet(columns, delimiter.Parameters.Where(p => p.Name == "DELIMITER")
                            .Select(p => p.Value.ToString())
                            .SingleOrDefault());
                    }
                    else
                        return _function.ToDotNetDataSet();
                }
                catch (Exception ex)
                {
                    AppendToTraceFile( string.Format("SAPCommand ExecuteDataset(), ERROR: IRfcFunction.ToDotNetDataSet(), {0}"
                                        , ex.ToString()));
                    throw ex;
                }
            }
            else return null;
        }

        public SAPDataReader ExecuteReader(string tableName = null)
        {
            AppendToTraceFile( string.Format("SAPCommand ExecuteReader(tableName), tableName: {0}", tableName));
            Execute();
            if (Success)
            {
                AppendToTraceFile( string.Format("SAPCommand ExecuteReader(), Success: {0}, returning DataReader", Success));
                try
                {
                    return _function.ToDotNetDataReader() as SAPDataReader;
                }
                catch (Exception ex)
                {
                    AppendToTraceFile( string.Format("SAPCommand ExecuteDataset(), ERROR: IRfcFunction.ToDotNetDataReader(), {0}"
                                        , ex.ToString()));
                    throw ex;
                }
            }
            else return null;
        }

        public Recordset[] ExecuteRecordset()
        {
            AppendToTraceFile( "SAPCommand ExecuteRecordset()");
            Execute();
            if (Success)
            {
                AppendToTraceFile( string.Format("SAPCommand ExecuteRecordset(), Success: {0}, returning recordset array"
                                    , Success));
                try
                {
                    return _function.ToAdodbRecordsetArray();
                }
                catch (Exception ex)
                {
                    AppendToTraceFile( string.Format("SAPCommand ExecuteDataset(), ERROR: IRfcFunction.ToAdodbRecordsetArray(), {0}"
                                        , ex.ToString()));
                    throw ex;
                }
            }
            else return null;
        }

        public RFCTable GetTable(string tableName)
        {
            if (!_isExecuteCalled) 
                throw new Exception(string.Format("Please execute the command before, calling SAPCommand.GetTable({0}) method."
                                    , tableName));

            return _function.Tables[tableName];
        }

        public Recordset GetReturnStructureAsRecordset()
        {
            if (Messages != null && Messages.Count > 0)
            {
                Recordset rs = new Recordset();
                List<object> fields = new List<object>();
                List<object> values = new List<object>();

                foreach (var key in Messages.Keys)
                {
                    rs.Fields.Append(key, DataTypeEnum.adWChar, 512, FieldAttributeEnum.adFldIsNullable, null);
                    fields.Add(key);
                    values.Add(Messages[key]);
                }
                rs.Open(Missing.Value, Missing.Value, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockPessimistic, 0);

                rs.AddNew(fields.ToArray(), values.ToArray());

                return rs;
            }
            else
            {
                return null;
            }
        }

        public Recordset GetRecordset(string tableName)
        {
            if (Success)
            {
                AppendToTraceFile( string.Format("SAPCommand GetRecordset(tableName), Success: {0}, returning recordset {1}"
                                                    , Success
                                                    , tableName));
                try
                {
                    return _function.ToAdodbRecordset(tableName);
                }
                catch (Exception ex)
                {
                    AppendToTraceFile( string.Format("SAPCommand GetRecordset(), ERROR: IRfcFunction.ToAdodbRecordset({1}), {0}"
                                        , ex.ToString()
                                        , tableName));
                    throw ex;
                }
            }
            else return null;
        }

        public Function GetFunctionStructure()
        {
  
            AppendToTraceFile( "SAPCommand GetFunctionStructure()");

            CreateRfcFunction();

            AppendToTraceFile( string.Format("SAPCommand GetFunctionStructure(), created function: {0}", CommandText));

            AppendToTraceFile( string.Format("SAPCommand GetFunctionStructure(), building structure", CommandText));

            return new Siemens.Sap.ERPConnect.Utilities.Function()
            {
                Columns = _function.Columns(),
                Structures = _function.Structures(),
                Tables = _function.Tables()
            };

        }

        public Recordset ExecuteRecordset(string tableName)
        {
            AppendToTraceFile( "SAPCommand ExecuteRecordset(tableName)");
            Execute();
            if (Success)
            {
                AppendToTraceFile( string.Format("SAPCommand ExecuteRecordset(tableName), Success: {0}, returning recordset {1}"
                                    , Success
                                    , tableName));
                try
                {
                    return _function.Tables[tableName].ToADOTable().ToAdodbRecordset();
                }
                catch (Exception ex)
                {
                    AppendToTraceFile( string.Format("SAPCommand ExecuteRecordset(), ERROR: IRfcFunction.ToAdodbRecordset({1}), {0}"
                                        , ex.ToString()
                                        , tableName));
                    throw ex;
                }
            }
            else return null;
        }

        public bool ExecuteRequest()
        {
            AppendToTraceFile( "SAPCommand ExecuteRequest()");
            Execute();
            AppendToTraceFile( string.Format("SAPCommand ExecuteRequest(), Success: {0}", Success));
            return Success;
        }

        public bool ExecuteRequest(ExecuteInformation information)
        {
            AppendToTraceFile( "SAPCommand ExecuteRequest(ExecuteInformation)");
            ExecuteInformation = information;
            return ExecuteRequest();
        }


        public void Close()
        {
            Dispose(true);
        }

        private void Execute()
        {
            AppendToTraceFile( "PRIVATE: SAPCommand Execute()");

            ExecuteQueryRfc();

            if (ExecuteInformation.OutParameterInformationArray.Length > 0)
            {
                AppendToTraceFile( "PRIVATE: SAPCommand Execute(), retrieving OUT Parameters");
                RetrieveOutParameters(_function);

                if (ExecuteInformation.RequiresSession)
                {
                    _commit.Execute();
                }
            }
        }

        private void RetrieveOutParameters(RFCFunction dataContainer)
        {
            var parameterInformationArray = ExecuteInformation.OutParameterInformationArray;

            foreach (var pInfo in parameterInformationArray)
            {
            

                if (pInfo.ContainerType == ContainerType.Function)
                {
                    RFCFunction container = default(RFCFunction);
                    container = dataContainer;
                    if (pInfo.Parameters != null && pInfo.Parameters.Count > 0)
                    {
                        RetrieveParameterValues(container, pInfo.Parameters, pInfo.ContainerOrdinalPosition);
                    }
                }
                else if (pInfo.ContainerType == ContainerType.Structure)
                {
                    RFCStructure s = dataContainer.Imports[pInfo.ContainerName.ToUpper()].ToStructure() as RFCStructure;
                    if (pInfo.Parameters != null && pInfo.Parameters.Count > 0)
                    {
                        RetrieveParameterValues(s, pInfo.Parameters, pInfo.ContainerOrdinalPosition);
                    }


                }
                else if (pInfo.ContainerType == ContainerType.Table)
                {
                    RFCTable t = dataContainer.Tables[pInfo.ContainerName.ToUpper()] as RFCTable;
                    if (pInfo.Parameters != null && pInfo.Parameters.Count > 0 && t.RowCount>0)
                    {
                        RetrieveParameterValues(t, pInfo.Parameters, pInfo.ContainerOrdinalPosition);
                    }
                }


              

            }
        }

        private void RetrieveParameterValues(RFCTable dataContainer, List<SAPParameter> parameters, int ordinalPosition)
        {
            if (dataContainer == null) 
                throw new ArgumentNullException(nameof(dataContainer));

            if (dataContainer.RowCount == 0)
                return;

            foreach (var param in parameters)
            {
                if (ordinalPosition > -1)
                    Out.Add(new SAPParameter() 
                    { 
                        Name = param.Name,
                        Value = dataContainer.Rows[ordinalPosition][param.Name] 
                    });

            }
        }

        private void RetrieveParameterValues(RFCStructure dataContainer, List<SAPParameter> parameters, int ordinalPosition)
        {
            if (dataContainer == null) return;

            foreach (var param in parameters)
            {
                if (ordinalPosition > -1)
                    Out.Add(new SAPParameter()
                    {
                        Name = param.Name,
                        Value = dataContainer[param.Name].ToString()
                    });

            }
        }

        private void RetrieveParameterValues(RFCFunction dataContainer, List<SAPParameter> parameters, int ordinalPosition)
        {
            if (dataContainer == null) return;

            foreach (var param in parameters)
            {
                if (ordinalPosition > -1)
                    Out.Add(new SAPParameter()
                    {
                        Name = param.Name,
                        Value = dataContainer.Imports[param.Name].ParamValue.ToString()
                    });  

            }
        }


        /// <summary>
        /// Creates an ERP RfcFunction object
        /// Tested: 12.09.22
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void CreateRfcFunction()
        {
            AppendToTraceFile( "PRIVATE: SAPCommand ExecuteQueryRfc()");
            if (string.IsNullOrEmpty(CommandText))
            {
                throw new Exception("command text cannot be null");
            }
            if (Connection == null)
            {
                throw new Exception("Connection cannot be null.");
            }
            AppendToTraceFile( string.Format("SAPCommand ExecuteRfc, Connection Is Open: {0}", Connection.IsOpen.ToString()));
            if (!Connection.IsOpen)
            {
                AppendToTraceFile( "SAPCommand ExecuteRfc, Opening Connection");
                Connection.Open();
            }

            _function =
               Connection.CreateFunction(CommandText);

          

        }

        private void ExecuteQueryRfc()
        {
            CreateRfcFunction();

            try
            {
                ApplyExecutionInformation(_function);
            }
            catch (Exception ex)
            {
                AppendToTraceFile( string.Format("PRIVATE: SAPCommand ExecuteQueryRfc(), ERROR: ApplyExecutionInformation(IRfcFunction) {0}", ex.Message));
                throw new Exception("An exception occurred when trying to apply execute information.", ex);
            }



            try
            {
                AppendToTraceFile( string.Format("PRIVATE: SAPCommand ExecuteQueryRfc(), IRfcFunction.Invoke(Destination.Connection), Destination Name {0}", Connection.LogonGroup));

                // Call RFC/BAPI in SAP
                _function.Execute();

                if (ExecuteInformation.RequiresSession)
                {
                    AppendToTraceFile( "PRIVATE: SAPCommand ExecuteQueryRfc(), Commit");
                    CommitSession();
                }
            }
            catch (Exception ex)
            {
                AppendToTraceFile( string.Format("PRIVATE: SAPCommand ExecuteQueryRfc(), ERROR: IRfcFunction.Invoke(Destination.Connection), Destination Name {0},  {1}", Connection.LogonGroup, ex.Message));
                throw new Exception(string.Format("An exception occurred when trying to execute function '{0}' on server.", CommandText), ex);
            }
            finally
            {
         
            }

            if (_function.HasReturnStructure())
            {
                MessageReturnType returnType = MessageReturnType.N;

                Success = _function.Success(out returnType);

                ReturnType = returnType;

                Messages = _function.GetMessages();
            }
            else
            {
                Success = true;
            }
            _isExecuteCalled = true;
        }

        private void CommitSession()
        {

            
            _commit = Connection.CreateFunction("BAPI_TRANSACTION_COMMIT");
            _commit.Exports["WAIT"].ParamValue = "X";

        }

        private void ApplyExecutionInformation(RFCFunction function)
        {
            AppendToTraceFile( "PRIVATE: SAPCommand ApplyExecutionInformation()");
            if (ExecuteInformation == null || ExecuteInformation.ParameterInformationArray == null || ExecuteInformation.ParameterInformationArray.Length == 0)
                return;

            ApplyExecutionInformation(function, ExecuteInformation.ParameterInformationArray);

            if (ExecuteInformation.Where.Items.Count >0)
                ApplyWhereClause(function, ExecuteInformation.Where);
        }

        private void ApplyWhereClause(RFCFunction container, WhereClause where)
        {
            AppendToTraceFile( "PRIVATE: SAPCommand ApplyWhereClause()");
            if (where == null || where.Items == null || where.Items.Count == 0) return;


            StringBuilder sb = new StringBuilder();

            foreach (var item in where.Items)
            {
                string value = string.Format("{0} {1} {2}{3}{4}", item.Name, GetConditionAsString(item.Condition), (item.Condition == WhereConditionType.NONE ? "" : "'"),
                    GetConditionalValue(item.Condition, item.Value), (item.Condition == WhereConditionType.NONE ? "" : "'"));

                sb.Append(string.Format(" {0}", value));

                RFCStructure newRow = container.Tables["OPTIONS"].Rows.Add();

              

                newRow["TEXT"] =value;
                
            }

            AppendToTraceFile( string.Format("PRIVATE: SAPCommand ApplyWhereClause() WHERE TEXT: {0}", sb.ToString()));
        }

        private string GetConditionalValue(WhereConditionType condition, string value)
        {
            switch (condition)
            {
                case WhereConditionType.STARTSWITH:
                    return value + "%";
                case WhereConditionType.LIKE:
                    return "%" + value + "%";
                case WhereConditionType.EQUALS:
                case WhereConditionType.GREATERTHAN:
                case WhereConditionType.LESSTHAN:
                case WhereConditionType.NOTEQUAL:
                case WhereConditionType.NONE:
                default:
                    return value;
            }
        }

        private string GetConditionAsString(WhereConditionType condition)
        {
            switch (condition)
            {
                case WhereConditionType.NOTEQUAL:
                    return "<>";
                case WhereConditionType.LESSTHAN:
                    return "<";
                case WhereConditionType.GREATERTHAN:
                    return ">";
                case WhereConditionType.NONE:
                    return string.Empty;
                case WhereConditionType.STARTSWITH:
                case WhereConditionType.LIKE:
                    return "LIKE";
                case WhereConditionType.EQUALS:
                default:
                    return "=";
            }
        }

        private void ApplyExecutionInformation(RFCFunction dataContainer, ParameterInformation[] parameterInformationArray)
        {
            AppendToTraceFile( "PRIVATE: SAPCommand ApplyExecutionInformation()");

            if (parameterInformationArray == null || parameterInformationArray.Length == 0) return;

            AppendToTraceFile( string.Format("PRIVATE: SAPCommand ApplyExecutionInformation() ParameterInformation[] Count: {0}", parameterInformationArray.Length));

            foreach (var pInfo in parameterInformationArray)
            {
                AppendToTraceFile(string.Format("PRIVATE: SAPCommand ApplyExecutionInformation() ParameterInformation - ContainerType: {0}, ContainerName: {1}, ContainerOrdinalPosition : {2}"
                                    , pInfo.ContainerType.ToString()
                                    , pInfo.ContainerName
                                    , pInfo.ContainerOrdinalPosition.ToString()));

                Object container = default(Object);

                if (pInfo.ContainerType == ContainerType.Function && dataContainer is RFCFunction)
                {
                    container = dataContainer as RFCFunction;
                    // Set the pameters on the container
                    if (pInfo.Parameters != null && pInfo.Parameters.Count > 0)
                    {
                        SetParameterValues((RFCFunction)container, pInfo.Parameters, pInfo.ContainerOrdinalPosition);
                    }

                }
                else if (pInfo.ContainerType == ContainerType.Structure )
                {
            
                    // Get RFCStructure cotainer  (object)
                    container = dataContainer.Exports[pInfo.ContainerName.ToUpper()].ToStructure() as RFCStructure;
                    // Set the pameters on the container
                    if (pInfo.Parameters != null && pInfo.Parameters.Count > 0)
                    {
                        SetParameterValues((RFCStructure)container, pInfo.Parameters, pInfo.ContainerOrdinalPosition);
                    }

                }
                else if (pInfo.ContainerType == ContainerType.Table)
                {
                    container = dataContainer.Tables[pInfo.ContainerName.ToUpper()];
                    // Set the pameters on the container
                    if (pInfo.Parameters != null && pInfo.Parameters.Count > 0)
                    {
                        SetParameterValues((RFCTable)container, pInfo.Parameters, pInfo.ContainerOrdinalPosition);
                    }
                }


  
            }
        }

        private void SetParameterValues(RFCTable dataContainer, List<SAPParameter> parameters, int ordinalPosition = -1)
        {
            AppendToTraceFile( "PRIVATE: SAPCommand SetParameterValues()");
            if (dataContainer == null)
                throw new ArgumentNullException(nameof(dataContainer));

            DataTable dt = dataContainer.ToADOTable();


            RFCStructure itemRow = dataContainer.Rows.Add();

            foreach (var param in parameters)
            {

                AppendToTraceFile(string.Format("PRIVATE: SAPCommand SetParameterValues() SAPParameter - Name: {0}, Value: {1}"
                                    , param.Name
                                    , param.Value));
 
                if (param.Value != null)
                {
 
                    itemRow[param.Name] = SetParamValue(param.Value, dt.Columns[param.Name].DataType);

                }
      

            }
        }

        private object SetParamValue(object value, Type t)
        {

        
            switch (t.ToString())
       
            {
                       
                case "System.Byte":
                    return Convert.ToByte(value);
                case "System.Char":
                    return Convert.ToChar(value);
                case "System.Int16":
                case "System.Int32":
                case "System.Int":
                    return Convert.ToInt32(value);
                case "System.Int64":
                    return Convert.ToInt64(value);
                case "System.BCD":
                case "System.Decimal":
                    return Convert.ToDecimal(value.ToString());
                case "System.DateTime":
                    return string.Format("yyyyMMdd", value);
                 default:

                   return  value.ToString();


            }

        }

        private void SetParameterValues(RFCStructure dataContainer, List<SAPParameter> parameters, int ordinalPosition = -1)
        {
            AppendToTraceFile( "PRIVATE: SAPCommand SetParameterValues()");
            if (dataContainer == null) return;

            foreach (var param in parameters)
            {
                AppendToTraceFile(string.Format("PRIVATE: SAPCommand SetParameterValues() SAPParameter - Name: {0}, Value: {1}"
                                , param.Name
                                , param.Value));

                Type t = dataContainer[param.Name].GetType();
                dataContainer[param.Name] = SetParamValue(param.Value, t);

            }
        }


        private void SetParameterValues(RFCFunction dataContainer, List<SAPParameter> parameters, int ordinalPosition = -1)
        {
            AppendToTraceFile( "PRIVATE: SAPCommand SetParameterValues()");
            if (dataContainer == null) return;

            foreach (var param in parameters)
            {
                AppendToTraceFile(string.Format("PRIVATE: SAPCommand SetParameterValues() SAPParameter - Name: {0}, Value: {1}"
                                    , param.Name
                                    , param.Value));

                Type t = dataContainer.Exports[param.Name].GetType();

                dataContainer.Exports[param.Name].ParamValue = SetParamValue(param.Value, t);

            }
        }

        private void AppendToTraceFile(string text)
        {
            Trace.WriteLineIf(_isTracingEnabled, text);
        }


        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _function = null;
                    if (this.Connection != null)
                    {
                        this.Connection = null;
                    }
                }

                if (_isTracingEnabled)
                {
                    var listener = System.Diagnostics.Trace.Listeners.OfType<TextWriterTraceListener>().Where(t => t.Name.ToLower() == _traceName.ToLower()).FirstOrDefault();

                    listener.Flush();
                    listener.Close();
                    listener.Dispose();

                    System.Diagnostics.Trace.Listeners.Remove(_traceName);
                }


                _disposed = true;
            }
        }

        ~SAPCommand()
        {
            Dispose(false);
        }

        #endregion
    }

}
