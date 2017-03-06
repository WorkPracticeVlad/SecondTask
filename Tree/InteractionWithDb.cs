using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    public class InteractionWithDb
    {
        private string _connString;
        private TreeCrawler _treeCrawler;
        public InteractionWithDb(string connString, TreeCrawler treeCrawler)
        {
            _connString = connString;
            _treeCrawler = treeCrawler;
        }
        public void InsertTreeToDb(string path)
        {
            _treeCrawler.EnterEnviroment(path);
            var insertExpression = FormatFullInsertCommandSql(_treeCrawler.OrgUnints, _treeCrawler.Props, _treeCrawler.OrgUnitToProps);
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(insertExpression, connection);
                int number = command.ExecuteNonQuery();                
            }          
        }
        string FormatInsertCommandSql(Dictionary<string,OrganizationUnit> orgUnits)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO OrganizationUnits VALUES ");
            foreach (var orgUnit in orgUnits)
            {
                strBuilder.AppendFormat("('{0}', '{1}', '{2}'), ", orgUnit.Key, orgUnit.Value.Description, orgUnit.Value.IsVirtual);
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
                if(prop.Value.Items!=null)
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
        string FormatInsertCommandSql(IDictionary<string,OrganizationUnitToProperty> orgUnitToProps)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO OrganizationUnitToProperties VALUES ");
            foreach (var orgUnitToProp in orgUnitToProps)
            {
                strBuilder.AppendFormat("('{0}', '{1}', '{2}'), ", orgUnitToProp.Value.OrganizationUnitIdentity, orgUnitToProp.Value.PropertyName, orgUnitToProp.Value.Value);
            }
            strBuilder.Remove(strBuilder.Length - 2, 2);
            strBuilder.Append(" ");
            return strBuilder.ToString();
        }
        string FormatFullInsertCommandSql(Dictionary<string, OrganizationUnit> orgUnits, Dictionary<string, Property> props, IDictionary<string, OrganizationUnitToProperty> orgUnitToProps)
        {
            var strBuilderer = new StringBuilder();
            strBuilderer.Append(FormatInsertCommandSql(orgUnits));
            strBuilderer.Append(FormatInsertCommandSql(props)[0]);
            strBuilderer.Append(FormatInsertCommandSql(props)[1]);
            strBuilderer.Append(FormatInsertCommandSql(orgUnitToProps));
            return strBuilderer.ToString();
        }
    }
}
