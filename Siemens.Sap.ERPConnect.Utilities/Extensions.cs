using ERPConnect;
using StrongInterop.ADODB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Siemens.Sap.ERPConnect.Utilities
{
    public static class Extensions
    {
        internal static Type DotNetType(this RFCTYPE dt)
        {
            switch (dt)
            {
                case RFCTYPE.BCD:
                    return typeof(decimal);
                case RFCTYPE.INT1:
                case RFCTYPE.INT2:
                case RFCTYPE.INT8:
                    return typeof(int);
                case RFCTYPE.FLOAT:
                    return typeof(double);
                case RFCTYPE.CHAR:
                case RFCTYPE.STRING:
                case RFCTYPE.DATE:
                default:
                    return typeof(string);
            }
        }

        internal static Column[] Columns(this RFCFunction rfcFunction)
        {
            List<Column> parameters = new List<Column>();

            if (rfcFunction != null && rfcFunction.HasReturnStructure() == true)
            {

                for (int i = 0; i < rfcFunction.Exports.Count; i++)
                {
                    if (rfcFunction.Exports[i].IsStructure() == false && rfcFunction.Exports[i].IsTable() == false)
                        if (rfcFunction.Exports[i] != null)
                            parameters.Add(new Column { Name = rfcFunction.Exports[i].Name, DataType = rfcFunction.Exports[i].Type.ToString() });
                }
            }
            return parameters.ToArray();
        }

        internal static Column[] Columns(this RFCStructure rfcStructure)
        {
            List<Column> parameters = new List<Column>();

            for (int i = 0; i < rfcStructure.Length; i++)
            {
                parameters.Add(new Column()
                {
                    Name = rfcStructure.Columns[i].Name,
                    DataType = rfcStructure.Columns[i].Type.ToString()
                });
            }

            return parameters.ToArray();
        }

        internal static Column[] Columns(this RFCTable rfcTable)
        {
            List<Column> parameters = new List<Column>();



            for (int i = 0; i < rfcTable.Columns.Count; i++)
            {
                parameters.Add(new Column { Name = rfcTable.Columns[i].Name, DataType = rfcTable.Columns[i].Type.ToString() });
            }

            return parameters.ToArray();
        }

        internal static Structure[] Structures(this RFCFunction rfcFunction)
        {
            List<Structure> parameters = new List<Structure>();

            var strucs = rfcFunction.Structures();


            for (int i=0; i < strucs.Length; i++)
            {

                parameters.Add(new Structure()
                {
                    ContainerName = strucs[i].ContainerName,
                    ContainerType = ContainerType.Structure,
                    Columns = Columns(rfcFunction.Tables[i])
                });
            }

            return parameters.ToArray();
        }

        internal static Table[] Tables(this RFCFunction rfcFunction)
        {
            List<Table> parameters = new List<Table>();

            var tbls = rfcFunction.Tables();

            

            for (int i = 0; i < tbls.Length; i++)
            {

                parameters.Add(new Table()
                {
                    ContainerName = tbls[i].ContainerName,
                    ContainerType = ContainerType.Table,
                    Columns = Columns(rfcFunction.Tables[i])
                });
            }

            return parameters.ToArray();
        }


        internal static Recordset ToAdodbRecordset(this DataTable inTable)
        {
            Recordset result = new Recordset();
            result.CursorLocation = CursorLocationEnum.adUseClient;

            Fields resultFields = result.Fields;
            System.Data.DataColumnCollection inColumns = inTable.Columns;

            foreach (DataColumn inColumn in inColumns)
            {
                resultFields.Append(inColumn.ColumnName
                    , TranslateType(inColumn.DataType)
                    , inColumn.MaxLength
                    , inColumn.AllowDBNull ? FieldAttributeEnum.adFldIsNullable :
                                             FieldAttributeEnum.adFldUnspecified
                    , null);
            }

            result.Open(System.Reflection.Missing.Value
                    , System.Reflection.Missing.Value
                    , CursorTypeEnum.adOpenStatic
                    , LockTypeEnum.adLockOptimistic, 0);

            foreach (DataRow dr in inTable.Rows)
            {
                result.AddNew(System.Reflection.Missing.Value,
                              System.Reflection.Missing.Value);

                for (int columnIndex = 0; columnIndex < inColumns.Count; columnIndex++)
                {
                    resultFields[columnIndex].Value = dr[columnIndex];
                }
            }

            return result;

        }


        static DataTypeEnum TranslateType(Type columnType)
        {
            switch (columnType.UnderlyingSystemType.ToString())
            {
                case "System.Boolean":
                    return DataTypeEnum.adBoolean;

                case "System.Byte":
                    return DataTypeEnum.adUnsignedTinyInt;

                case "System.Char":
                    return DataTypeEnum.adChar;

                case "System.DateTime":
                    return DataTypeEnum.adDate;

                case "System.Decimal":
                    return DataTypeEnum.adCurrency;

                case "System.Double":
                    return DataTypeEnum.adDouble;

                case "System.Int16":
                    return DataTypeEnum.adSmallInt;

                case "System.Int32":
                    return DataTypeEnum.adInteger;

                case "System.Int64":
                    return DataTypeEnum.adBigInt;

                case "System.SByte":
                    return DataTypeEnum.adTinyInt;

                case "System.Single":
                    return DataTypeEnum.adSingle;

                case "System.UInt16":
                    return DataTypeEnum.adUnsignedSmallInt;

                case "System.UInt32":
                    return DataTypeEnum.adUnsignedInt;

                case "System.UInt64":
                    return DataTypeEnum.adUnsignedBigInt;

                case "System.String":
                default:
                    return DataTypeEnum.adVarChar;
            }
        }


        internal static bool Success(this RFCFunction function, out MessageReturnType returnType)
        {
            RFCStructure structure = function.Exports["RETURN"].ToStructure();

            try
            {
                returnType = (MessageReturnType)Enum.Parse(typeof(MessageReturnType), structure.GetType().Name.ToUpper(), true);
            }
            catch
            {
                returnType = MessageReturnType.S;
            }

            return (structure != null && returnType != MessageReturnType.E && returnType != MessageReturnType.A);
        }

        internal static Dictionary<string, string> GetMessages(this RFCFunction function)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            RFCStructure structure = function.Exports["RETURN"].ToStructure();

            dictionary.Add("TYPE", structure["TYPE"].ToString());
            dictionary.Add("LOG_NO", structure["LOG_NO"].ToString());
            dictionary.Add("LOG_MSG_NO", structure["LOG_MSG_NO"].ToString());
            dictionary.Add("MESSAGE", structure["MESSAGE"].ToString());
            dictionary.Add("MESSAGE_V1", structure["MESSAGE_V1"].ToString());
            dictionary.Add("MESSAGE_V2", structure["MESSAGE_V2"].ToString());
            dictionary.Add("MESSAGE_V3", structure["MESSAGE_V3"].ToString());
            dictionary.Add("MESSAGE_V4", structure["MESSAGE_V4"].ToString());
            dictionary.Add("NUMBER", structure["NUMBER"].ToString());
            dictionary.Add("CODE", structure["CODE"].ToString());

            return dictionary;
        }

        internal static bool HasReturnStructure(this RFCFunction function)
        {
            try
            {
                if (function.Exports.Contains("RETURN"))
                {
                    return (function.Exports["RETURN"].ToString() != String.Empty);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        internal static DataSet ToDotNetDataSet(this RFCFunction function)
        {
            DataSet ds = new DataSet();


            foreach (RFCTable t in function.Tables)
            {

                ds.Tables.Add(t.ToADOTable());
            }


            return ds;
        }

        internal static DataSet ToDotNetDataSet(this RFCFunction function, string[] columnNames, string delimiter)
        {
            DataSet ds = new DataSet();


            foreach (RFCTable t in function.Tables)
            {
                DataTable dt = t.ToADOTable();

                DataTable table = new DataTable(t.Name);

                if (t.Name.ToLower() == "data")
                {

                    for (int j = 0; j < columnNames.Length; j++)
                    {
                        table.Columns.Add(new DataColumn { ColumnName = columnNames[j], DataType = typeof(string) });
                    }

                    foreach (var row in dt.Rows.OfType<DataRow>())
                    {
                        var newRow = table.NewRow();

                        string[] values = row[0].ToString().Split(new char[] { ',' });

                        int j = 0;
                        foreach (var v in values)
                        {
                            newRow[j] = v;
                            j++;
                        }

                        table.Rows.Add(newRow);
                    }
                }

                ds.Tables.Add(table);
            }
            return ds;
        }

        internal static Recordset[] ToAdodbRecordsetArray(this RFCFunction function)
        {

            List<Recordset> list = new List<Recordset>();

            foreach (RFCTable t in function.Tables)
                list.Add(t.ToADOTable().ToAdodbRecordset());

            return list.ToArray();
        }

        internal static Recordset ToAdodbRecordset(this RFCFunction function, string tableName)
        {
            //TODO: Check works with upper & lowervase table name
            RFCTable t = function.Tables[tableName] as RFCTable;

            if (t == null)
                return null;
            else
                return t.ToADOTable().ToAdodbRecordset();
        }

        internal static IDataReader ToDotNetDataReader(this RFCFunction function)
        {
            return new SAPDataReader(function);
        }

        internal static object ToDateTimeFromSAPDateTime(this RFCStructure item, string name)
        {
            string i = item[name].ToString();
            return i.ToDateTimeFromSAPDateTime();
        }

        public static object ToDateTimeFromSAPDateTime(this string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value)) return System.DBNull.Value;

                return DateTime.ParseExact(value, "yyyyMMdd", null);
            }
            catch
            {
                throw new InvalidCastException("cannot convert sap datetime to system.DateTime.");
            }
        }

        public static string ToSAPDateTimeFromDateTime(this DateTime value)
        {
            return value.ToString("yyyyMMdd");
        }

        private static string GetItemValueAsString(this RFCStructure structure, string name)
        {
            try
            {
                return structure[name].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static DataTypeEnum ToAdodbDataType(this RFCTYPE dt)
        {
            switch (dt)
            {
                case RFCTYPE.BCD:
                    return DataTypeEnum.adDecimal;
                case RFCTYPE.INT:
                case RFCTYPE.INT1:
                case RFCTYPE.INT2:
                case RFCTYPE.INT8:
                    return DataTypeEnum.adInteger;
                case RFCTYPE.FLOAT:
                    return DataTypeEnum.adDouble;
                case RFCTYPE.CHAR:
                case RFCTYPE.STRING:
                case RFCTYPE.DATE:
                default:
                    return DataTypeEnum.adWChar;
            }
        }

        public static int GetAdoDataTypeSize(this DataTypeEnum dt)
        {
            switch (dt)
            {
                case DataTypeEnum.adWChar:
                    return 512;
                default:
                    return 0;
            }
        }
        public static bool IsNumeric(this object val)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(val), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
        #region string extensions

        public static bool ToBoolean(this string instance)
        {
            bool value = false;

            if (!bool.TryParse(instance, out value)) value = false;

            return value;

        }

        internal static int ToInt(this string instance)
        {
            int retValue = 0;
            if (!int.TryParse(instance, out retValue)) throw new InvalidCastException("cannot convert string to int");
            return retValue;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> e, T value)
        {
            foreach (var cur in e)
            {
                yield return cur;
            }
            yield return value;
        }

        internal static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }

            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        #endregion
    }
}