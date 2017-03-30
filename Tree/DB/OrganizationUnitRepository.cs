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
        private string _procCountPagesUnitNode;
        private string _procRowsPerUnitNode;

        public OrganizationUnitRepository()
            : base(TABLE_NAME)
        {
            _insertOrganizationUnits = "[dbo].[InsertOrganizationUnits]";
            _procSelectOrgUnitsByParent = "[dbo].[SelectOrgUnitsByParent]";
            _procSelectOrgUnitsToAncestorsFiltered = "[dbo].[SelectOrgUnitsToAncestorsFiltered]";
            _procCountPagesUnitNode = "[dbo].[CountPagesUnitNode]";
            _procRowsPerUnitNode = "[dbo].[RowsPerUnitNode]";
        }
        int CountUnitNodePages(int itemsPerPage, string parent)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                int pageCount = 0;
                connection.Open();
                SqlCommand command = new SqlCommand(_procCountPagesUnitNode, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
                command.Parameters.Add(AddSqlParameter("@Identity", parent));
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
        public List<OrgUnitWithPagesCount> ReadUnitNodePageFromDb(string parent,int page, int itemsPerPage)
        {
            var items = new List<OrganizationUnit>();
            var itemsToPages = new List<OrgUnitWithPagesCount>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procRowsPerUnitNode, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@Identity", parent));
                command.Parameters.Add(AddSqlParameter("@Page", page));
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            AddItem(items, reader);
                    reader.Close();
                }
            }
            foreach (var item in items)
            {
                itemsToPages.Add(new OrgUnitWithPagesCount { OrgUnit= item, PagesCount= CountUnitNodePages(itemsPerPage, item.Identity) });
            }
            return itemsToPages;
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
