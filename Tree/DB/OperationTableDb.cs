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
    public class OperationTableDb

    {
        protected string _connString;
        protected string _fnPagesCount;
        protected string _procRowsPerPage;
        protected string _tableName;
        protected int _itemsPerPage;

        public OperationTableDb(string tableName, int itemsPerPage)
        {
            _connString = ConfigurationManager.ConnectionStrings["SecondTaskConnection"].ConnectionString; 
            _fnPagesCount = "[dbo].[fnCountPagesInTable]";
            _procRowsPerPage = "[dbo].[RowsPerPage]";
            _tableName = tableName;
            _itemsPerPage = itemsPerPage;
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
            command.CommandType = CommandType.StoredProcedure;
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
        public List<IStich> ReadPage(int page, string column)
        {
            var items = new List<IStich>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procRowsPerPage, connection);
                AddSqlParemeters(page, _itemsPerPage, _tableName, column, command);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            AddItem(items, reader, _tableName);
                        }
                    }
                    reader.Close();
                }
            }
            return items;
        }
        protected void AddItem(List<IStich> items, SqlDataReader reader, string table)
        {
            switch (table)
            {
                case "[dbo].[OrganizationUnits]":
                    items.Add(new OrganizationUnit
                    {
                        Identity = reader.GetString(0),
                        Description = reader.GetString(1),
                        IsVirtual = reader.GetBoolean(2),
                        ParentIdentity = reader.GetString(3)
                    });
                    break;
                case "[dbo].[OrganizationUnitToProperties]":
                    if (reader.IsDBNull(1))
                    {
                        items.Add(new OrganizationUnitToProperty
                        {
                            OrganizationUnitIdentity = reader.GetString(0),
                            PropertyName = null,
                            Value = null
                        });
                        break;
                    }
                    items.Add(new OrganizationUnitToProperty
                    {
                        OrganizationUnitIdentity = reader.GetString(0),
                        PropertyName = reader.GetString(1),
                        Value = reader.GetString(2)
                    });
                    break;
                case "[dbo].[Properties]":
                    items.Add(new Property
                    {
                        Name = reader.GetString(0),
                        Type = reader.GetString(1)
                    });
                    break;
                default:
                    throw new Exception();
                    //break;
            }
        }
    }
}
