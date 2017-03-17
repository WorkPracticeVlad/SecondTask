using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree.Data;

namespace Tree.DB
{
    public class OrganizationUnitToPropertyRepository : Repository<OrganizationUnitToProperty>
    {
        private string _insertOrganizationUnitToProperties;
        private string _selcetAllValuesForOrgUnit;
        public OrganizationUnitToPropertyRepository(string tableName, int itemsPerPage) : base(tableName, itemsPerPage)
        {
            _insertOrganizationUnitToProperties = "[dbo].[InsertOrganizationUnitToProperties]";
            _selcetAllValuesForOrgUnit = "[dbo].[SelectAllValuesForOrganizationUnitByIdentiy]";
        }

        public override int InsertToDb(List<OrganizationUnitToProperty> orgUnitToProperties)
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new DataColumn[3] { new DataColumn("[OrganizationUnitIdentity]", typeof(string)),
                new DataColumn("[PropertyName]", typeof(string)),
                    new DataColumn("[Value]", typeof(string))});
            foreach (var orgUnitToProperty in orgUnitToProperties)
            {
                string orgUnitIdentity = orgUnitToProperty.OrganizationUnitIdentity;
                string propertyName = orgUnitToProperty.PropertyName;
                string value = orgUnitToProperty.Value;
                dataTable.Rows.Add(orgUnitIdentity, propertyName, value);
            }
            return ExecuteInsertDataTable(dataTable, _insertOrganizationUnitToProperties, "@OrganizationUnitToProperties");
        }

        protected override void AddItem(List<OrganizationUnitToProperty> items, SqlDataReader reader)
        {
            if (reader.IsDBNull(1))
            {
                items.Add(new OrganizationUnitToProperty
                {
                    OrganizationUnitIdentity = reader.GetString(0),
                    PropertyName = null,
                    Value = null
                });
                return;
            }
            items.Add(new OrganizationUnitToProperty
            {
                OrganizationUnitIdentity = reader?.GetString(0),
                PropertyName = reader?.GetString(1),
                Value = reader?.GetString(2)
            });
        }
        public List<OrganizationUnitToProperty> ReadOrganizationUnitValuesFromDb(string unitIdentity)
        {
            List<OrganizationUnitToProperty> items = new List<OrganizationUnitToProperty>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_selcetAllValuesForOrgUnit, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@Identity", unitIdentity));
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                        AddItem(items, reader);
                    while (reader.NextResult())
                        while (reader.Read())
                            AddItem(items, reader);
                }
                reader.Close();
            }
            return items;
        }
    }
}
