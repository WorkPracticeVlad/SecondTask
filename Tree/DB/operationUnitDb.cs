using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree.Data;

namespace Tree.DB
{
    public class OperationUnitDb : OperationTableDb
    {
        private string _procSelectAllValuesForOrganizationUnit;
        public OperationUnitDb(int itemsPePage)
            : this("[dbo].[OrganizationUnits]", itemsPePage)
        {

        }
        public OperationUnitDb(string tableName, int itemsPerPage)
            : base( tableName,itemsPerPage)
        {
            tableName = "[dbo].[OrganizationUnits]";
            _tableName = tableName;
            _procSelectAllValuesForOrganizationUnit = "[dbo].[SelectAllValuesForOrganizationUnit]";
        }
        public List<IStich> ReadOrganizationUnitValuesFromDb(string unitIdentity)
        {
            List<IStich> items = new List<IStich>();
            var selectAllValuesForOrganizationUnitExpression = _procSelectAllValuesForOrganizationUnit;
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(selectAllValuesForOrganizationUnitExpression, connection);
                AddSqlParemeters(unitIdentity, command);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.IsDBNull(1))
                            {
                                AddItem(items, reader, "[dbo].[OrganizationUnitToProperties]");
                                continue;
                            }
                            AddItem(items, reader, "[dbo].[OrganizationUnitToProperties]");
                        }
                    }
                    while (reader.NextResult())
                        while (reader.Read())
                        {
                            if (reader.IsDBNull(1))
                            {
                                AddItem(items, reader, "[dbo].[OrganizationUnitToProperties]");
                                continue;
                            }
                            AddItem(items, reader, "[dbo].[OrganizationUnitToProperties]");
                        }
                }
                reader.Close();
            }
            return items;
        }
        private void AddSqlParemeters(string identity, SqlCommand command)
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(AddSqlParameter("@Identity", identity));
        }
    }
}
