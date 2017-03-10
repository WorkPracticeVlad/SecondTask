using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tree
{
    public class OperationWithDb : IOperationWithDb
    {
        private string _connString;
        private TreeCrawler _treeCrawler;
        List<OrganizationUnit> _orgUnits = new List<OrganizationUnit>();
        List<Property> _props = new List<Property>();
        List<OrganizationUnitToProperty> _orgUnitToProps = new List<OrganizationUnitToProperty>();
        Dictionary<Item, string> _items = new Dictionary<Item, string>();
        public List<OrganizationUnit> OrgUnints { get { return _orgUnits; } }
        public List<Property> Props { get { return _props; } }
        public List<OrganizationUnitToProperty> OrgUnitToProps { get { return _orgUnitToProps; } }
        public OperationWithDb()
        {
            _connString = ConfigurationManager.ConnectionStrings["SecondTaskConnection"].ConnectionString;
            _treeCrawler = new TreeCrawler();
        }
        public int InsertTreeToDb(string path)
        {
            _treeCrawler.EnterEnvironment(path);
            var insertExpression = FormatFullInsertCommandSql(_treeCrawler.OrgUnints, _treeCrawler.Props, _treeCrawler.OrgUnitToProps);
            var numberAffectedRows = 0;
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(insertExpression, connection);
                numberAffectedRows = command.ExecuteNonQuery();
            }
            return numberAffectedRows;
        }
        public int DeleteTreeFromDb()
        {
            var deleteExpression = FormatFullDeleteCommandSql();
            var numberAffectedRows = 0;
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(deleteExpression, connection);
                numberAffectedRows = command.ExecuteNonQuery();
            }
            return numberAffectedRows;
        }
        public void ReadTreeFromDb()
        {
            var selectExpression = FormatFullSelectCommandSql();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(selectExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        _orgUnitToProps.Add(new OrganizationUnitToProperty
                        {
                            OrganizationUnitIdentity = reader.GetString(0),
                            PropertyName = reader.GetString(1),
                            Value = reader.GetString(2)
                        });
                    }
                    if (reader.NextResult())
                        while (reader.Read())
                        {
                            _items.Add(new Item
                            {
                                Id = reader.GetInt32(0),
                                RecMode = reader.GetString(2),
                                Value = reader.GetString(3)
                            }, reader.GetString(1));
                        }
                    if (reader.NextResult())
                        while (reader.Read())
                        {
                            _orgUnits.Add(new OrganizationUnit
                            {
                                Identity = reader.GetString(0),
                                Description = reader.GetString(1),
                                IsVirtual = reader.GetBoolean(2),
                                ParentIdentity = reader.GetString(3)
                            });
                        }
                    if (reader.NextResult())
                        while (reader.Read())
                        {
                            _props.Add(new Property
                            {
                                Name = reader.GetString(0),
                                Type = reader.GetString(1)
                            });
                        }
                }
                reader.Close();
            }
            foreach (var prop in _props)
            {
                var temp = _items.Where(i => i.Value == prop.Name).Select(i => i.Key).ToList();
                if (temp.Count != 0)
                {
                    prop.Items = new List<Item>();
                    prop.Items.AddRange(temp);
                }             
            }
        }
        string FormatInsertCommandSql(Dictionary<string, OrganizationUnit> orgUnits)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO OrganizationUnits VALUES ");
            foreach (var orgUnit in orgUnits)
            {
                strBuilder.AppendFormat("('{0}', '{1}', '{2}', '{3}'), ", orgUnit.Key, orgUnit.Value.Description, orgUnit.Value.IsVirtual, orgUnit.Value.ParentIdentity);
            }
            strBuilder.Remove(strBuilder.Length - 2, 2);
            strBuilder.Append(" ");
            return strBuilder.ToString();
        }
        string[] FormatInsertCommandSql(Dictionary<string, Property> props)
        {
            var propsStrBuilder = new StringBuilder();
            var itemsStrBuilder = new StringBuilder();
            propsStrBuilder.Append("INSERT INTO Properties VALUES ");
            itemsStrBuilder.Append("INSERT INTO Items VALUES ");
            foreach (var prop in props)
            {
                propsStrBuilder.AppendFormat("('{0}', '{1}'), ", prop.Key, prop.Value.Type);
                if (prop.Value.Items != null)
                    foreach (var item in prop.Value.Items)
                    {
                        itemsStrBuilder.AppendFormat("('{0}', '{1}', '{2}'), ", prop.Key, item.RecMode, item.Value);
                    }
            }
            propsStrBuilder.Remove(propsStrBuilder.Length - 2, 2);
            propsStrBuilder.Append(" ");
            itemsStrBuilder.Remove(itemsStrBuilder.Length - 2, 2);
            itemsStrBuilder.Append(" ");
            return new string[] { propsStrBuilder.ToString(), itemsStrBuilder.ToString() };
        }
        string FormatInsertCommandSql(IDictionary<string, OrganizationUnitToProperty> orgUnitToProps)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO OrganizationUnitToProperties VALUES ");
            foreach (var orgUnitToProp in orgUnitToProps)
            {
                strBuilder.AppendFormat("('{0}', '{1}', '{2}'), ", orgUnitToProp.Value.OrganizationUnitIdentity, orgUnitToProp.Value.PropertyName, /*Regex.Unescape(*/orgUnitToProp.Value.Value/*)*/);
            }
            strBuilder.Remove(strBuilder.Length - 2, 2);
            strBuilder.Append(" ");
            return strBuilder.ToString();
        }
        string FormatFullInsertCommandSql(Dictionary<string, OrganizationUnit> orgUnits, Dictionary<string, Property> props, IDictionary<string, OrganizationUnitToProperty> orgUnitToProps)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(FormatInsertCommandSql(orgUnits));
            strBuilder.Append(FormatInsertCommandSql(props)[0]);
            strBuilder.Append(FormatInsertCommandSql(props)[1]);
            strBuilder.Append(FormatInsertCommandSql(orgUnitToProps));
            //strBuilder.Append(" IF OBJECT_ID('dbo.[fk_OrgUnitParent]', 'F') IS NULL ALTER TABLE OrganizationUnits ADD CONSTRAINT fk_OrgUnitParent FOREIGN KEY(ParentIdentity) REFERENCES [OrganizationUnits]([Identity]); ");
            return strBuilder.ToString();
        }
        string FormatFullDeleteCommandSql()
        {
            var strBuilder = new StringBuilder();
            var tables = new string[] { "OrganizationUnitToProperties", "Items", "OrganizationUnits", "Properties" };
            foreach (var table in tables)
                strBuilder.AppendFormat("DELETE FROM {0}; ", table);
            //strBuilder.Append(" IF OBJECT_ID('dbo.[fk_OrgUnitParent]', 'F') IS NOT NULL ALTER TABLE OrganizationUnits DROP CONSTRAINT fk_OrgUnitParent; ");
            return strBuilder.ToString();
        }
        string FormatFullSelectCommandSql()
        {
            var strBuilder = new StringBuilder();
            var tables = new string[] { "OrganizationUnitToProperties", "Items", "OrganizationUnits", "Properties" };
            foreach (var table in tables)
            {
                strBuilder.AppendFormat("SELECT * FROM {0}; ", table);
            }
            return strBuilder.ToString();
        }
    }
}
