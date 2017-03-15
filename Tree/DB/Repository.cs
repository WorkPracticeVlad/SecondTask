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
        protected int _itemsPerPage;
        protected string _selectAllFromTable;
        protected string _deleteAllFromTable;
        public Repository(string tableName, int itemsPerPage)
        {
            _connString = ConfigurationManager.ConnectionStrings["SecondTaskConnection"].ConnectionString;
            _fnPagesCount = "[dbo].[fnCountPagesInTable]";
            _procRowsPerPage = "[dbo].[RowsPerPage]";
            _tableName = tableName;
            _itemsPerPage = itemsPerPage;
            _selectAllFromTable = "[dbo].[SelectAllFromTable]";
            _deleteAllFromTable = "[dbo].[DeleteAllFromTable]";
        }
        public int CountPages()
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_fnPagesCount, connection);
                var pageCountSqlPar = AddSqlParemeters(_itemsPerPage, _tableName, command);
                command.ExecuteNonQuery();
                return (int)pageCountSqlPar.Value;
            }
        }
        public List<T> ReadPageFromDb(int page, string column)
        {
            var items = new List<T>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procRowsPerPage, connection);
                AddSqlParemeters(page, _itemsPerPage, _tableName, column, command);
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
            command.CommandType = System.Data.CommandType.StoredProcedure;
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
