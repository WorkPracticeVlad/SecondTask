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
   public class PropertyRepository : Repository<Property>
    {
        private string _insertProperties;

        public PropertyRepository(string tableName, int itemsPerPage) : base(tableName, itemsPerPage)
        {
            _insertProperties = "[dbo].[InsertProperties]";
        }

        public override int InsertToDb(List<Property> properties)
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new DataColumn[2] { new DataColumn("[Name]", typeof(string)),
                    new DataColumn("[Type]", typeof(string))});
            foreach (var property in properties)
            {
                string name = property.Name;
                string type = property.Type;              
                dataTable.Rows.Add(name,type);
            }
            return ExecuteInsertDataTable(dataTable, _insertProperties, "@Properties");
        }

        protected override void AddItem(List<Property> items, SqlDataReader reader)
        {
            items.Add(new Property
            {
                Name = reader?.GetString(0),
                Type = reader?.GetString(1)
            });
        }
    }
}
