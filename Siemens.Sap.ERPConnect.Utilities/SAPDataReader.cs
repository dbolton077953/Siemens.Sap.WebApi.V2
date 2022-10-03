using ERPConnect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Siemens.Sap.ERPConnect.Utilities
{
    public class SAPDataReader : IDataReader
    {
        private bool _disposed = false;
        private RFCFunction _function;
        private RFCTable _table;
        private RFCStructure _item;

        private string[] _tableNames;
        private int _currentTableIndex;

        private int _index = -1;

        public SAPDataReader(RFCFunction function)
        {
            if (function == null) throw new ArgumentNullException("function");
            _function = function;
            int x = 0;
            foreach(RFCTable t in function.Tables)
            {
                _tableNames[x++] = t.Name;
            }

       
            _currentTableIndex = 0;
            _table = _function.Tables[_currentTableIndex];
        }

        private int RowCount { get { return _table.RowCount; } }

        public bool Read()
        {
            if (!HasRows) return false;

            _index++;

            if (_index == RowCount) return false;

       
            _currentTableIndex = _index;

            _item = _table.Rows[_index];

            return true;
        }

       

        public bool HasRows { get { return RowCount > 0; } }

        public void Close()
        {
            Dispose(true);
        }


        #region Methods not implemented as yet
        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed
        {
            get { throw new NotImplementedException(); }
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        #endregion

        public bool NextResult()
        {
            _currentTableIndex++;
            try
            {
                _table = _function.Tables[_currentTableIndex];
                _index = -1;
                _item = null;
            }
            catch (Exception ex)
            {
            }
            return _table != null;
        }

        public int RecordsAffected
        {
            get { return RowCount; }
        }

        public int FieldCount { get { return _table.Columns.Count; } }

  
        public byte GetByte(int i)
        {

            return byte.Parse(_item[i].ToString());
        }

 
        public char GetChar(int i)
        {
            return char.Parse(_item[i].ToString());
 
        }

 
        public DateTime GetDateTime(int i)
        {
            var value = _item[i].ToString().ToDateTimeFromSAPDateTime();
            if (value == null || value == System.DBNull.Value)
            {
                throw new InvalidCastException("cannot convert sap datetime to system.DateTime.");
            }

            return (DateTime)value;
        }

        public decimal GetDecimal(int i)
        {
            return decimal.Parse(_item[i].ToString());
        }

        public double GetDouble(int i)
        {
            return double.Parse(_item[i].ToString());
        }

 
        public float GetFloat(int i)
        {
            return float.Parse(_item[i].ToString());
        }

 

        public short GetInt16(int i)
        {
            return Int16.Parse(_item[i].ToString());
        }

        public int GetInt32(int i)
        {
            return Int32.Parse(_item[i].ToString());
        }

        public long GetInt64(int i)
        {
            return Int64.Parse(_item[i].ToString());
        }



        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return _item[i].ToString();
        }

        public object GetValue(int i)
        {
            return (object) _item[i];
        }


        public object this[int index]
        {
            get
            {
                return _item[index].ToString();
            }
        }

        public object this[string name]
        {
            get
            {
                return (object) _item[name];
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
                    _table = null;
                    _function = null;
                    _item = null;
                }
                _disposed = true;
            }
        }

        ~SAPDataReader()
        {
            Dispose(true);
        }


        #endregion

        #region helpers

        private string[] GetResultTableNames(string metadataDescription)
        {
            var regex = new Regex(@"TABLES (\w+):");

            List<string> list = new List<string>();

            foreach (Match match in regex.Matches(metadataDescription))
            {
                list.Add(match.Groups[1].Value);
            }
            return list.ToArray();
        }

        #endregion
    }

}
