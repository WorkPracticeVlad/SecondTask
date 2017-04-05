using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree.DB
{
    abstract public class Repository<T>
    {
        protected string _connString;
        protected string _fnPagesCount;
        protected string _procRowsPerPage;
        protected string _tableName;
        protected string _selectAllFromTable;
        protected string _deleteAllFromTable;
        protected string _procRowsPerPageFiltered;
        protected string _procPagesCountFiltered;
        protected string _procRowPerPageFilterAndColumnValue;
        protected string _procCountPagesInFilteredByColumnValue;
        protected string _columnToOrder;
        protected string _columnToFilter;

        public Repository(string tableName)
        {
            _connString = ConfigurationManager.ConnectionStrings["SecondTaskConnection"].ConnectionString;
            _fnPagesCount = "[dbo].[fnCountPagesInTable]";
            _procRowsPerPage = "[dbo].[RowsPerPage]";
            _tableName = tableName;
            _selectAllFromTable = "[dbo].[SelectAllFromTable]";
            _deleteAllFromTable = "[dbo].[DeleteAllFromTable]";
            _procRowsPerPageFiltered = "[dbo].[RowsPerPageFilter]";
            _procPagesCountFiltered = "[dbo].[CountPagesInFiltered]";
            _procRowPerPageFilterAndColumnValue = "[dbo].[RowPerPageFilterAndColumnValue]";
            _procCountPagesInFilteredByColumnValue = "[dbo].[CountPagesInFilteredByColumnValue]";
        }
        public int CountPages(int itemsPerPage)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_fnPagesCount, connection);
                var pageCountSqlPar = AddSqlParemeters(itemsPerPage, _tableName, command);
                command.ExecuteNonQuery();
                return (int)pageCountSqlPar.Value;
            }
        }
        protected int CountPagesFiltered(int itemsPerPage, string columnToFilter ,string filter)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                int pageCount=0;
                connection.Open();
                SqlCommand command = new SqlCommand(_procPagesCountFiltered, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
                command.Parameters.Add(AddSqlParameter("@TableName", _tableName));
                command.Parameters.Add(AddSqlParameter("@ColumnToFilter", columnToFilter));
                command.Parameters.Add(AddSqlParameter("@Filter", filter));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            pageCount = reader.GetInt32(0);
                    reader.Close();
                }
                return pageCount;
            }
        }
        protected int CountPagesFiltered(int itemsPerPage, string columnToFilter, string filter, string columnForValue, string value)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                int pageCount = 0;
                connection.Open();
                SqlCommand command = new SqlCommand(_procCountPagesInFilteredByColumnValue, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
                command.Parameters.Add(AddSqlParameter("@TableName", _tableName));
                command.Parameters.Add(AddSqlParameter("@ColumnToFilter", columnToFilter));
                command.Parameters.Add(AddSqlParameter("@Filter", filter));
                command.Parameters.Add(AddSqlParameter("@ColumnForValue", columnForValue));
                command.Parameters.Add(AddSqlParameter("@Value", value));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            pageCount = reader.GetInt32(0);
                    reader.Close();
                }
                return pageCount;
            }
        }
        public List<T> ReadPageFromDb(int page, int itemsPerPage)
        {
            var items = new List<T>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procRowsPerPage, connection);
                AddSqlParemeters(page, itemsPerPage, _tableName, _columnToOrder, command);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            AddItem(items, reader);
                    reader.Close();
                }
            }
            return items;
        }
        protected List<T> ReadFilteredPageFromDb(int page,  int itemsPerPage,  string filter)
        {
            var items = new List<T>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procRowsPerPageFiltered, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@TableName", _tableName));
                command.Parameters.Add(AddSqlParameter("@Page", page));
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));               
                command.Parameters.Add(AddSqlParameter("@ColumnToOrderBy", _columnToOrder));
                command.Parameters.Add(AddSqlParameter("@ColumnToFilter", _columnToFilter));
                command.Parameters.Add(AddSqlParameter("@Filter", filter));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            AddItem(items, reader);
                    reader.Close();
                }
            }
            return items;
        }
        protected List<T> ReadFilteredPageFromDb(int page, int itemsPerPage,string columnToOrder, string columnToFilter, string filter, string value)
        {
            var items = new List<T>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procRowPerPageFilterAndColumnValue, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@TableName", _tableName));
                command.Parameters.Add(AddSqlParameter("@Page", page));
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
                command.Parameters.Add(AddSqlParameter("@ColumnToOrderBy", columnToOrder));
                command.Parameters.Add(AddSqlParameter("@ColumnToFilter", columnToFilter));
                command.Parameters.Add(AddSqlParameter("@Filter", filter));
                command.Parameters.Add(AddSqlParameter("@ColumnForValue", columnToOrder));
                command.Parameters.Add(AddSqlParameter("@Value", value));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            AddItem(items, reader);
                    reader.Close();
                }
            }
            return items;
        }
        public List<T> ReadDataFromDb()
        {
            var items = new List<T>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_selectAllFromTable, connection);
                command.CommandType = CommandType.StoredProcedure;
                AddSqlParemeters(_tableName, command);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)      
                        while (reader.Read())
                            AddItem(items, reader);                  
                    reader.Close();
                }
            }
            return items;
        }
        public int DeleteDataFromDb()
        {
            var numberAffectedRows = 0;
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_deleteAllFromTable, connection);
                command.CommandType = CommandType.StoredProcedure;
                AddSqlParemeters(_tableName, command);
                numberAffectedRows = command.ExecuteNonQuery();
            }
            return numberAffectedRows;
        }
        protected SqlParameter AddSqlParemeters(int itemsPerPage, string table, SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
            command.Parameters.Add(AddSqlParameter("@TableName", table));
            SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;
            return returnValue;
        }
        protected void AddSqlParemeters(int page, int itemsPerPage, string table, string columnToOrder, SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(AddSqlParameter("@Page", page));
            command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
            command.Parameters.Add(AddSqlParameter("@TableName", table));
            command.Parameters.Add(AddSqlParameter("@ColumnToOrderBy", columnToOrder));
        }
        protected void AddSqlParemeters( string table, SqlCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(AddSqlParameter("@TableName", table));
        }
        protected SqlParameter AddSqlParameter(string name, string value)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                Value = value
            };
            return param;
        }
        protected SqlParameter AddSqlParameter(string name, int value)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                Value = value
            };
            return param;
        }
        protected int ExecuteInsertDataTable(DataTable dataTable, string procedureInsert, string paramName)
        {
            int numberAffectedRows;
            var sqlParam = new SqlParameter(paramName, SqlDbType.Structured);
            sqlParam.Value = dataTable;
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(procedureInsert, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(sqlParam);
                numberAffectedRows = command.ExecuteNonQuery();
            }
            return numberAffectedRows;
        }
        protected abstract void AddItem(List<T> items, SqlDataReader reader);
        public abstract int InsertToDb(List<T> items);

    }
}
