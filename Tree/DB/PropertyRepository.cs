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
        const string TABLE_NAME = "[dbo].[Properties]";
        const string COLUMN_TO_ORDER = "[Name]";
        string _procPropertiesPerPageWithFilter = "[dbo].[PropertiesPerPageFilter]";
        string _fnCountPagesInFilteredByNamePropertyTable = "[dbo].[fnCountPagesInFilteredByNamePropertyTable]";
        public PropertyRepository() : base(TABLE_NAME)
        {
            _insertProperties = "[dbo].[InsertProperties]";
        }
        public int CountPages(int itemsPerPage,string filter)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_fnCountPagesInFilteredByNamePropertyTable, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
                command.Parameters.Add(AddSqlParameter("@Filter", filter));
                SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                returnValue.Direction = ParameterDirection.ReturnValue;
                command.ExecuteNonQuery();
                return (int)returnValue.Value;
            }
        }
        public List<Property> ReadPropertyWithUsagePageFromDb(int page, int itemsPerPage, string filter)
        {
            var items = new List<Property>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_procPropertiesPerPageWithFilter, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(AddSqlParameter("@Page", page));
                command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
                command.Parameters.Add(AddSqlParameter("@ColumnToOrderBy", COLUMN_TO_ORDER));
                command.Parameters.Add(AddSqlParameter("@Filter", filter));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            AddPropertyWithUsage(items, reader);
                    reader.Close();
                }
            }
            return items;
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
            if (reader.IsDBNull(1))
            {
                items.Add(new Property
                {
                    Name = reader?.GetString(0),
                    Type = null
                });
                return;
            }
            items.Add(new Property
            {
                Name = reader?.GetString(0),
                Type = reader?.GetString(1)
            });
        }
        void AddPropertyWithUsage(List<Property> properties, SqlDataReader reader)
        {
            if (reader.IsDBNull(1))
            {
                properties.Add(new Property
                {
                    Name = reader?.GetString(0),
                    Type = null,
                    CountOfUsage = reader.GetInt32(2) 
                });
                return;
            }
            properties.Add(new Property
            {
                Name = reader?.GetString(0),
                Type = reader?.GetString(1),
                CountOfUsage = reader.GetInt32(2)
            });
        }
    }
}
