using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;

public static class DatabaseManager
{
    //public const string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\\M05_UF3_P2_Template\\App_Data\\mainDB.mdf;Integrated Security=True;Connect Timeout=30";
    public static readonly string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + AppDomain.CurrentDomain.GetData("RelativePath") + "\\App_Data\\mainDB.mdf;Integrated Security=True;Connect Timeout=30";

    #region TYPES
    public class DB_Field
    {
        public string field;
        public object value;
        /// <summary>
        /// Create a database field+value
        /// </summary>
        /// <param name="_field">Name of the column</param>
        /// <param name="_value">Value</param>
        public DB_Field(string _field, object _value)
        {
            field = _field;
            value = _value;
        }
        public string ValueToQuery()
        {
            if (value == null)
                return " NULL ";
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return " NULL ";
                case TypeCode.Boolean:
                    return " " + ((bool)value ? "1 " : "0 ");
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt32:
                case TypeCode.UInt16:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return " " + value.ToString() + " ";
                case TypeCode.DateTime:
                case TypeCode.Char:
                case TypeCode.Object:
                default:
                    return " '" + value.ToString() + "' ";
            }
        }
        public string ToUpdate()
        {
            return " " + field + " = " + ValueToQuery();
        }
    }
    public class DB_Comparer : DB_Field
    {
        public enum COMPARE_TYPE
        {
            EQUALS,
            GREATER,
            LOWER,
            GREATER_OR_EQUAL,
            LOWER_OR_EQUAL,
            BETWEEN,
            LIKE,
            IN
        }
        public COMPARE_TYPE comparer;
        /// <summary>
        /// Creates a comparing condition
        /// </summary>
        /// <param name="_field">Name of the column</param>
        /// <param name="_value">Value to compare</param>
        /// <param name="_comparer">Type of comparision</param>
        public DB_Comparer(string _field, object _value, COMPARE_TYPE _comparer) : base(_field, _value)
        {
            comparer = _comparer;
        }
        public static string ComparerToQuery(COMPARE_TYPE comparer)
        {
            switch (comparer)
            {
                case COMPARE_TYPE.GREATER:
                    return " > ";
                case COMPARE_TYPE.LOWER:
                    return " < ";
                case COMPARE_TYPE.GREATER_OR_EQUAL:
                    return " >= ";
                case COMPARE_TYPE.LOWER_OR_EQUAL:
                    return " <= ";
                case COMPARE_TYPE.BETWEEN:
                    return " BETWEEN ";
                case COMPARE_TYPE.LIKE:
                    return " LIKE ";
                case COMPARE_TYPE.IN:
                    return " IN ";
                default:
                    return " = ";
            }
        }
        public string ToSelect()
        {
            return " " + field + ComparerToQuery(comparer) + ValueToQuery();
        }
    }
    #endregion TYPES

    #region METHODS

    #region SELECT

    /// <summary>
    /// Returns a DataTable with the resoults of a select query
    /// </summary>
    /// <param name="commandString"> select command</param>
    /// <returns></returns>
    public static DataTable Select(string commandString)
    {
        DataTable result = new DataTable();
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            using (SqlCommand command = new SqlCommand(commandString, connection))
            {
                connection.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }
            }
        }
        return result;
    }
    /// <summary>
    /// Returns a DataTable with the resoults of a select query
    /// </summary>
    /// <param name="table">Name of the table</param>
    /// <param name="fields">List of columns to retrieve information (Optional)</param>
    /// <param name="condition">Search condition (Optional)</param>
    /// <param name="sorting">Sort (Optional)</param>
    /// <returns></returns>
    public static DataTable Select(string table, string[] fields = null, string condition = null, string sorting = null)
    {
        StringBuilder sb = new StringBuilder(" SELECT  ");
        if (fields == null)
        {
            sb.Append(" * ");
        }
        else
        {
            for (int i = 0; i < fields.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(" " + fields[i] + " ");
            }
        }
        sb.Append(" FROM " + table + " ");
        if (!string.IsNullOrWhiteSpace(condition))
            sb.Append(" WHERE " + condition);
        if (!string.IsNullOrWhiteSpace(sorting))
            sb.Append(" ORDER BY " + sorting);
        return Select(sb.ToString());
    }
    /// <summary>
    /// Returns a DataTable with the resoults of a select query
    /// </summary>
    /// <param name="table">Name of the table</param>
    /// <param name="fields">List of columns to retrieve information (Optional)</param>
    /// <param name="conditions">List of conditions to meet</param>
    /// <param name="sorting">Sort (Optional)</param>
    /// <returns></returns>
    public static DataTable Select(string table, string[] fields = null, DB_Comparer[] conditions = null, string sorting = null)
    {
        StringBuilder sb = new StringBuilder();
        if (conditions != null)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(" AND ");
                }
                sb.Append(" " + conditions[i].ToSelect() + " ");
            }
        }
        return Select(table, fields, sb.ToString(), sorting);
    }

    #endregion SELECT

    #region SCALAR

    public static object Scalar(string commandString)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            using (SqlCommand command = new SqlCommand(commandString, connection))
            {
                connection.Open();
                return (object)command.ExecuteScalar();
            }
        }
    }

    public enum SCALAR_TYPE
    {
        COUNT,
        AVG,
        SUM,
        MIN,
        MAX
    }
    public static object Scalar(string table, SCALAR_TYPE type, string[] fields = null, string condition = null)
    {
        StringBuilder sb = new StringBuilder(" SELECT " + type.ToString() + "(");
        if (fields == null)
        {
            sb.Append(" * ");
        }
        else
        {
            for (int i = 0; i < fields.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(" " + fields[i] + " ");
            }
        }
        sb.Append(")  FROM " + table + " ");
        if (!string.IsNullOrWhiteSpace(condition))
            sb.Append(" WHERE " + condition);
        return Scalar(sb.ToString());
    }
    public static object Scalar(string table, SCALAR_TYPE type, string[] fields = null, DB_Comparer[] conditions = null)
    {
        StringBuilder sb = new StringBuilder();
        if (conditions != null)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(" AND " + conditions[i].ToSelect() + " ");
            }
        }
        return Scalar(table, type, fields, sb.ToString());
    }

    #endregion SCALAR

    #region EXECUTE
    /// <summary>
    /// Execute a command (Update, Insert, Delete)
    /// </summary>
    /// <param name="commandString"></param>
    /// <returns></returns>
    public static int Execute(string commandString)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            using (SqlCommand command = new SqlCommand(commandString, connection))
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Updates the data of a table given a conition
    /// </summary>
    /// <param name="table">Name of the table</param>
    /// <param name="values">Columns & Values to update</param>
    /// <param name="condition">Conditions (Optional)</param>
    /// <returns></returns>
    public static int Update(string table, DB_Field[] values, string condition = null)
    {
        StringBuilder sb = new StringBuilder(" UPDATE " + table + " SET ");
        for (int i = 0; i < values.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }
            sb.Append(" " + values[i].ToUpdate() + " ");
        }
        if (!string.IsNullOrWhiteSpace(condition))
            sb.Append(" WHERE " + condition);
        return Execute(sb.ToString());
    }
    /// <summary>
    /// Updates the data of a table given an id
    /// </summary>
    /// <param name="table">Name of the table</param>
    /// <param name="values">Columns & Values to update</param>
    /// <param name="id">Id of the row to update</param>
    /// <returns></returns>
    public static int Update(string table, DB_Field[] values, int id)
    {
        return Update(table, values, " ID = " + id + " ");
    }
    public static int Update(string table, DB_Field[] values, DB_Comparer[] conditions = null)
    {
        StringBuilder sb = new StringBuilder();
        if (conditions != null)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(" AND " + conditions[i].ToSelect() + " ");
            }
        }
        return Update(table, values, sb.ToString());
    }
    public static int Delete(string table, string condition = null)
    {
        StringBuilder sb = new StringBuilder(" DELETE FROM " + table + " ");
        if (!string.IsNullOrWhiteSpace(condition))
            sb.Append(" WHERE " + condition);
        return Execute(sb.ToString());
    }
    public static int Delete(string table, int id)
    {
        return Delete(table, " ID = " + id + " ");
    }
    public static int Delete(string table, DB_Comparer[] conditions = null)
    {
        StringBuilder sb = new StringBuilder();
        if (conditions != null)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(" AND " + conditions[i].ToSelect() + " ");
            }
        }
        return Delete(table, sb.ToString());
    }

    public static int Insert(string table, DB_Field[] values)
    {
        StringBuilder sb = new StringBuilder(" INSERT INTO " + table + " ( ");
        for (int i = 0; i < values.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }
            sb.Append(" " + values[i].field + " ");
        }
        sb.Append(" ) VALUES ( ");
        for (int i = 0; i < values.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }
            sb.Append(" " + values[i].ValueToQuery() + " ");
        }
        sb.Append(" ) ");
        return Execute(sb.ToString());
    }
    #endregion

    #endregion METHODS
}