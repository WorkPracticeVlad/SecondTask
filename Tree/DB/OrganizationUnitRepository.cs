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
    public class OrganizationUnitRepository : Repository<OrganizationUnit>
    {
        private string _insertOrganizationUnits;
        const string TABLE_NAME = "[dbo].[OrganizationUnits]";
        public OrganizationUnitRepository() : base(TABLE_NAME)
        {
            _insertOrganizationUnits = "[dbo].[InsertOrganizationUnits]";         
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
                    ParentIdentity = reader?.GetString(3)
                });
                return;
            }
            items.Add(new OrganizationUnit
            {
                Identity = reader.GetString(0),
                Description = reader?.GetString(1),
                IsVirtual = reader.GetBoolean(2),
                ParentIdentity = reader?.GetString(3)
            });
        }
        
    }
}
