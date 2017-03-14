using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree.Data;

namespace Tree.DB
{
    abstract public class OperationDb
    {
        protected string _connString;
        protected string _fnPagesCount;
        protected string _procRowsPerPage;
        protected string _tableName;
        protected int _itemsPerPage;

        public OperationDb()
        {
            _connString = ConfigurationManager.ConnectionStrings["SecondTaskConnection"].ConnectionString;        
            _fnPagesCount = "[dbo].[fnItemsCountPerPageInTable]";
            _procRowsPerPage = "[dbo].[RowsPerPage]";
            
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
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(AddSqlParameter("@Page", page));
            command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
            command.Parameters.Add(AddSqlParameter("@TableName", table));
            command.Parameters.Add(AddSqlParameter("@ColumnToOrderBy", columnToOrder));
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
        public abstract List<IStich> ReadPage(int page, string column);     
    }
}
