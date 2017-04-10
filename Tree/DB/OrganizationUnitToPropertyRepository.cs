using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Tree.Data;
using Tree.Data.SubData;

namespace Tree.DB
{
    public class OrganizationUnitToPropertyRepository : Repository<OrganizationUnitToProperty>
    {
        private string _insertOrganizationUnitToProperties;
        const string TABLE_NAME = "[dbo].[OrganizationUnitToProperties]";
        const string COLUMN_UNIT = "[OrganizationUnitIdentity]";
        const string COLUMN_PROPERTY = "[PropertyName]";
        private string _procRowsPerPageValuesForOrganizationUnitByIdentiyFiltered;
        private string _procCountPagesValuesForOrganizationUnitByIdentiyFiltered;
        private string _procSelectOrgUnitsToAncestorsIdentity;
        private string _procCountPagesCurrentValuesForOrganizationUnitByIdentiyFiltered;
        private string _procRowsPerPageCurrentValuesForOrganizationUnitByIdentiyFiltered;

        public OrganizationUnitToPropertyRepository()
            : base(TABLE_NAME)
        {
            _insertOrganizationUnitToProperties = "[dbo].[InsertOrganizationUnitToProperties]";
            _procRowsPerPageValuesForOrganizationUnitByIdentiyFiltered = "[dbo].[RowsPerPageValuesForOrganizationUnitByIdentiyFiltered]";
            _procCountPagesValuesForOrganizationUnitByIdentiyFiltered = "[dbo].[CountPagesValuesForOrganizationUnitByIdentiyFiltered]";
            _procSelectOrgUnitsToAncestorsIdentity = "[dbo].[SelectOrgUnitsToAncestorsIdentity]";
            _procCountPagesCurrentValuesForOrganizationUnitByIdentiyFiltered = "[dbo].[CountPagesCurrentValuesForOrganizationUnitByIdentiyFiltered]";
            _procRowsPerPageCurrentValuesForOrganizationUnitByIdentiyFiltered = "[dbo].[RowsPerPageCurrentValuesForOrganizationUnitByIdentiyFiltered]";
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
                OrganizationUnitIdentity = reader.GetString(0),
                PropertyName = reader.GetString(1),
                Value = reader.GetString(2)
            });
        }
        public int CountPagesByOrgUnit(int itemsPerPage, string identity, string filter)
        {
            return CountPagesByUnit(itemsPerPage, identity, filter, _procCountPagesValuesForOrganizationUnitByIdentiyFiltered);
        }
        public int CountPagesByCurrentOrgUnit(int itemsPerPage, string identity, string filter)
        {
            return CountPagesByUnit(itemsPerPage, identity, filter, _procCountPagesCurrentValuesForOrganizationUnitByIdentiyFiltered);
        }
        int CountPagesByUnit(int itemsPerPage, string identity, string filter, string procedure)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                int pageCount = 0;
                connection.Open();
                SqlCommand command = new SqlCommand(procedure, connection);
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
        public ForOrgUnitProperties ReadPageOrganizationUnitValuesFilteredFromDb(string unitIdentity, int page, int itemsPerPage, string filter)
        {
            List<OrganizationUnitToProperty> items = GetUnitsToPropertyPerRow(unitIdentity, page, itemsPerPage, filter, _procRowsPerPageValuesForOrganizationUnitByIdentiyFiltered);/*new List<OrganizationUnitToProperty>();*/
            List<string> orgUnitsId = new List<string>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procSelectOrgUnitsToAncestorsIdentity, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@Identity", unitIdentity));
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                        orgUnitsId.Add(reader.GetString(0));
                }
                reader.Close();
            }
            return new ForOrgUnitProperties(items, orgUnitsId,true);
        }
        public ForOrgUnitProperties ReadPageOrganizationUnitCurrentValuesFilteredFromDb(string unitIdentity, int page, int itemsPerPage, string filter)
        {
            List<OrganizationUnitToProperty> items = GetUnitsToPropertyPerRow(unitIdentity, page, itemsPerPage, filter, _procRowsPerPageCurrentValuesForOrganizationUnitByIdentiyFiltered);/*new List<OrganizationUnitToProperty>();*/
            List <string> orgUnitsId = new List<string>();
            orgUnitsId.Add(unitIdentity);
            return new ForOrgUnitProperties(items, orgUnitsId,false);
        }
        List<OrganizationUnitToProperty> GetUnitsToPropertyPerRow(string unitIdentity, int page, int itemsPerPage, string filter, string procedure)
        {
            List<OrganizationUnitToProperty> items = new List<OrganizationUnitToProperty>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(procedure, connection);
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
                }
                reader.Close();
            }
            return items;
        }
        public List<OrganizationUnitToProperty> ReadFilteredPropertyPageFromDb(int page, int itemsPerPage, string filter, string value)
        {
            return ReadFilteredPageFromDb(page, itemsPerPage, COLUMN_PROPERTY,COLUMN_UNIT, filter, value);
        }     
        public int CountPagesPropertyFiltered(int itemsPerPage, string filter, string value)
        {       
            return CountPagesFiltered(itemsPerPage, COLUMN_UNIT, filter ,COLUMN_PROPERTY, value);
        }
    }
}
