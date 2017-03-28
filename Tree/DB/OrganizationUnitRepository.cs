using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree.Data;
using Tree.Data.SubData;

namespace Tree.DB
{
    public class OrganizationUnitRepository : Repository<OrganizationUnit>
    {
        private string _insertOrganizationUnits;
        const string TABLE_NAME = "[dbo].[OrganizationUnits]";
        private string _procSelectOrgUnitsByParent;
        private string _procSelectOrgUnitsToAncestorsFiltered;

        public OrganizationUnitRepository()
            : base(TABLE_NAME)
        {
            _insertOrganizationUnits = "[dbo].[InsertOrganizationUnits]";
            _procSelectOrgUnitsByParent = "[dbo].[SelectOrgUnitsByParent]";
            _procSelectOrgUnitsToAncestorsFiltered = "[dbo].[SelectOrgUnitsToAncestorsFiltered]";           
        }
        public List<OrganizationUnit> ReadChildrenFromDb(string parent)
        {
            var items = new List<OrganizationUnit>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procSelectOrgUnitsByParent, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@Identity", parent));
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
        public List<UnitTreeNode> ReadBranchesFilteredFromDb(string filter)
        {
            var items = new List<OrganizationUnit>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procSelectOrgUnitsToAncestorsFiltered, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@Filter", filter));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            AddItem(items, reader);
                    reader.Close();
                }
            }
            var nodes = new List<UnitTreeNode>();
            nodes.Add(new UnitTreeNode(items, "Enviroment"));
            return nodes;
        }
        public override int InsertToDb(List<OrganizationUnit> units)
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new DataColumn[4] { new DataColumn("[Identity]", typeof(string)),
                    new DataColumn("[Description]", typeof(string)),
                    new DataColumn("[IsVirtual]",typeof(bool)),
                    new DataColumn("[ParentIdentity]", typeof(string))});
            foreach (var unit in units)
            {
                string identity = unit.Identity;
                string description = unit.Description;
                bool isVirtual = unit.IsVirtual;
                string parentIdentity = unit.ParentIdentity;
                dataTable.Rows.Add(identity, description, isVirtual, parentIdentity);
            }
            return ExecuteInsertDataTable(dataTable, _insertOrganizationUnits, "@OrgUnits");
        }
        protected override void AddItem(List<OrganizationUnit> items, SqlDataReader reader)
        {
            if (reader.IsDBNull(1))
            {
                items.Add(new OrganizationUnit
                {
                    Identity = reader.GetString(0),
                    Description = null,
                    IsVirtual = reader.GetBoolean(2),
                    ParentIdentity = reader.GetString(3)
                });
                return;
            }
            items.Add(new OrganizationUnit
            {
                Identity = reader.GetString(0),
                Description = reader.GetString(1),
                IsVirtual = reader.GetBoolean(2),
                ParentIdentity = reader.GetString(3)
            });
        }

    }
}
