using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree.Data;

namespace Tree.DB
{
    public class OperationUnitDb : OperationDb
    {
        public OperationUnitDb()
        {
            _tableName = "[dbo].[OrganizationUnits]";
            _itemsPerPage = 4;
        }
        public override List<IStich> ReadPage(int page, string column)
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
                            items.Add(new OrganizationUnit
                            {
                                Identity = reader.GetString(0),
                                Description = reader.GetString(1),
                                IsVirtual = reader.GetBoolean(2),
                                ParentIdentity = reader.GetString(3)
                            });
                        }
                    }
                    reader.Close();
                }
            }
            return items;
        }
    }
}
