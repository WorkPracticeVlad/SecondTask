using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Tree.Data;

namespace Tree.DB
{
    public class OrganizationUnitToPropertyRepository : Repository<OrganizationUnitToProperty>
    {
        private string _insertOrganizationUnitToProperties;
        private string _selcetAllValuesForOrgUnit;
        const string TABLE_NAME = "[dbo].[OrganizationUnitToProperties]";
        private string _procRowsPerPageValuesForOrganizationUnitByIdentiyFiltered;
        private string _procCountPagesValuesForOrganizationUnitByIdentiyFiltered;

        public OrganizationUnitToPropertyRepository() : base(TABLE_NAME)
        {
            _insertOrganizationUnitToProperties = "[dbo].[InsertOrganizationUnitToProperties]";
            _selcetAllValuesForOrgUnit = "[dbo].[SelectAllValuesForOrganizationUnitByIdentiy]";
            _procRowsPerPageValuesForOrganizationUnitByIdentiyFiltered = "[dbo].[RowsPerPageValuesForOrganizationUnitByIdentiyFiltered]";
            _procCountPagesValuesForOrganizationUnitByIdentiyFiltered = "[dbo].[CountPagesValuesForOrganizationUnitByIdentiyFiltered]";
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
        public int CountPagesByOrgUnit(int itemsPerPage, string identity, string filter)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                int pageCount = 0;
                connection.Open();
                SqlCommand command = new SqlCommand(_procCountPagesValuesForOrganizationUnitByIdentiyFiltered, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
                command.Parameters.Add(AddSqlParameter("@Identity", identity));
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
        public Dictionary<string, List<OrganizationUnitToProperty>> ReadPageOrganizationUnitValuesFilteredFromDb(string unitIdentity, int page, int itemsPerPage, string filter)
        {
            List<OrganizationUnitToProperty> items = new List<OrganizationUnitToProperty>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procRowsPerPageValuesForOrganizationUnitByIdentiyFiltered, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@Identity", unitIdentity));
                command.Parameters.Add(AddSqlParameter("@Page", page));
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
                command.Parameters.Add(AddSqlParameter("@Filter", filter));
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
            }//
            var valuesForResponse = new Dictionary<string, List<OrganizationUnitToProperty>>();
            if (!items.Any(i => i.OrganizationUnitIdentity == unitIdentity))
                valuesForResponse.Add("OrgUnitName-" + unitIdentity, null);
            foreach (var p in items.Select(u => u.PropertyName).Distinct().ToList())
            {
                valuesForResponse.Add(p, items.Where(i => i.PropertyName == p).ToList());
            }
            foreach (var u in items.Select(u => u.OrganizationUnitIdentity).Distinct().ToList())
            {
                valuesForResponse.Add("OrgUnitName-" + u, items.Where(i => i.PropertyName == u).ToList());
            }
            return valuesForResponse;
        }
    }
}
