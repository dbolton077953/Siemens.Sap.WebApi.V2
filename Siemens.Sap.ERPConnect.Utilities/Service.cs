using ERPConnect;
using ERPConnect.Utils;
using System.Data;
using Microsoft.Extensions.Configuration;
using Siemens.Sap.ERPConnect.Utilities.Interfaces;
using System.Drawing;
using System.Globalization;

namespace Siemens.Sap.ERPConnect.Utilities
{
    public class Service : IDisposable,IService
    {
        private R3Connection _connection;
        private bool _disposed;
        private SAPConnectionSettings _settings=null;
     

        public R3Connection Connection
        {
            get
            {
                return _connection;
            }
        }

        public Service(SAPConnectionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settings = settings;

            if (_connection == null)
            {
                _connection = new R3Connection();
                _connection.Protocol = ClientProtocol.NWRFC;
                _connection.Logging = settings.Logging;
                if (_connection.Logging)
                {
                    _connection.LogDir = settings.LogDir;
                }
                _connection.Client = settings.Client;
                _connection.MessageServer = settings.MessageServer;
                _connection.SNCSettings.QualityOfProtection = SNCQualityOfProtection.Default;
                _connection.SNCSettings.LibraryPath = settings.SNCLib;
                _connection.SID = settings.SID;
                _connection.SystemNumber = Convert.ToInt32(settings.SysNr);
                _connection.UserName = settings.User;
                _connection.LogonGroup = settings.Group;
                _connection.SNCSettings.PartnerName = settings.SNCPartnerName;
                _connection.SNCSettings.Enabled = true;
                _connection.Language = settings.Lang;
               
                
            
               
            } 
        }

        public SAPCommand GetSAPCommand(ExecuteInformation information)
        {
        
            SAPCommand cmd = new SAPCommand(_settings
                                            , information.CommandText
                                            , Connection);


            cmd.ExecuteInformation = information;

            return cmd;
        }


        public SAPCommand GetSAPCommand(string commandText)
        {
            return new SAPCommand(_settings
                                  , commandText
                                  , Connection);
        }

        public DataTable GetReturnInfoAsDataTable(Dictionary<string, string> messages)
        {
            DataTable dt = new DataTable("ReturnInfo");

            dt.Columns.Add("Code", typeof(String));
            dt.Columns.Add("LogMsgNo", typeof(String));
            dt.Columns.Add("LogNo", typeof(String));
            dt.Columns.Add("Message", typeof(String));
            dt.Columns.Add("MessageV1", typeof(String));
            dt.Columns.Add("MessageV2", typeof(String));
            dt.Columns.Add("MessageV3", typeof(String));
            dt.Columns.Add("MessageV4", typeof(String));
            dt.Columns.Add("Type", typeof(String));

            if (messages != null && messages.Count > 0)
                dt.Rows.Add(new object[]
                {
                    messages["CODE"],
                    messages["LOG_MSG_NO"],
                    messages["LOG_NO"],
                    messages["MESSAGE"],
                    messages["MESSAGE_V1"],
                    messages["MESSAGE_V2"],
                    messages["MESSAGE_V3"],
                    messages["MESSAGE_V4"],
                    messages["TYPE"]
                });

            return dt;
        }

        public ReturnInfo GetReturnInfo(Dictionary<string, string> messages)
        {
            ReturnInfo info = new ReturnInfo();

            foreach (var key in messages.Keys)
            {
                switch (key.ToLower())
                {
                    case "message":
                        info.Message = messages[key];
                        break;
                    case "message_v1":
                        info.MessageV1 = messages[key];
                        break;
                    case "message_v2":
                        info.MessageV2 = messages[key];
                        break;
                    case "message_v3":
                        info.MessageV3 = messages[key];
                        break;
                    case "message_v4":
                        info.MessageV4 = messages[key];
                        break;
                    case "number":
                        info.Number = messages[key];
                        break;
                    default:
                        break;
                }
            }

            return info;
        }



        public string GetTablesAsJsonDelimitedList(DataSet ds)
        {
            string tables = string.Empty;

            if (ds.Tables.Count > 0)
            {

                int idx = 0;
                string table = string.Empty;

                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        table = DataTableToJSONWithStringBuilder(dt);

                        if (idx == 0)
                        {
                            tables = table;
                        }
                        else
                        {
                            tables = tables + "@@" + table;
                        }

                        idx++;
                    }
                }



            }
            return tables;
        }



        public string ReadSAPTable(string sapTableName, string[] sapTableFields, string whereClause, int maxRows)
        {
            ReadTable table = new ReadTable(this.Connection);

            if (!Connection.IsOpen)
            {
                Connection.Open();
            }


         
            foreach(string s in sapTableFields)
            {
                table.AddField(s);
            }

            table.WhereClause = whereClause;
            table.TableName = sapTableName;
            table.RowCount = maxRows;

            table.Run();

            DataTable resulttable = table.Result;

            Connection.Close();

            return DataTableToJSONWithStringBuilder(resulttable);
        }

        public string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new System.Text.StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
             
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + GetFieldInfo(table.Columns[j].ColumnName.ToUpper(), table.Rows[i][j].ToString(), table.Columns[j].DataType) + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + GetFieldInfo(table.Columns[j].ColumnName.ToUpper(), table.Rows[i][j].ToString(), table.Columns[j].DataType) + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1 || table.Rows.Count==1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }

                JSONString.Append("]");

            }
            return JSONString.ToString();
        }

        private string GetFieldInfo(string columnName, string? columnData, Type t)
        {
            DateTime tempDate = new DateTime();
           if (t.FullName=="System.DateTime" || (columnName.Contains("DATE") && columnData.Length==8))
           {
                CultureInfo provider = CultureInfo.InvariantCulture;
                if (columnData == null || columnData == "00000000")
                {
                    tempDate = DateTime.MinValue.Date;
    
                }
                else
                {
                    try
                    {
                        tempDate = DateTime.ParseExact(columnData, "dd/MM/yyyy hh:mm:ss", provider);
                    }
                    catch (Exception )
                    {
                        return columnData;
                    }

                 }
                return tempDate.ToString("yyyy.MM.dd");
           }
           else
            {
                return columnData;
            }
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
                    if (Connection != null)
                    {
                        if (!Connection.IsOpen) Connection.Close();
                        Connection.Dispose();
                    }
                }
            }
            _disposed = true;
        }

        #endregion
    }
}
