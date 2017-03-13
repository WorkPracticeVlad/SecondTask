using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        string[] tables = new string[] { "[dbo].[OrganizationUnitToProperties]", "[dbo].[OrganizationUnits]", "[dbo].[Properties]" };
        string[] procs = new string[] { "[dbo].[RowsPerPage]", "[dbo].[SelectAllValuesForOrganizationUnit]", "[dbo].[SelectOrganizationUnitToAncestors]" };
        string[] func = new string[] { "[dbo].[fnItemsCountPerPageInTable]" };
        List<OrganizationUnit> _orgUnits = new List<OrganizationUnit>();
        List<Property> _props = new List<Property>();
        List<OrganizationUnitToProperty> _orgUnitToProps = new List<OrganizationUnitToProperty>();
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
                    ReaderReadSet(_orgUnitToProps, reader);
                    if (reader.NextResult())
                        ReaderReadSet(_orgUnits, reader);
                    if (reader.NextResult())
                        ReaderReadSet(_props, reader);
                }
                reader.Close();
            }

        }
        public int CountPropertyPages(int itemsPerPage)
        {
           return  CountPages(itemsPerPage, tables[2]);
        }
        public int CountUnitPages(int itemsPerPage)
        {
            return CountPages(itemsPerPage, tables[1]);
        }
        public int CountValuesPages(int itemsPerPage)
        {
            return CountPages(itemsPerPage, tables[0]);
        }
        int CountPages(int itemsPerPage, string table)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(func[0], connection);
                var pageCountSqlPar = AddSqlParemeters(itemsPerPage, table, command);
                command.ExecuteNonQuery();
                return (int)pageCountSqlPar.Value;
            }
        }
        public List<OrganizationUnitToProperty> ReadOrganizationUnitValuesFromDb(string unitIdentity)
        {
            List<OrganizationUnitToProperty> unitValues = new List<OrganizationUnitToProperty>();
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(procs[1], connection);
                AddSqlParemeters(unitIdentity, command);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    ReaderReadSet(unitValues, reader);
                    while (reader.NextResult())
                        ReaderReadSet(unitValues, reader);
                }
                reader.Close();
            }
            return unitValues;
        }
        public List<OrganizationUnitToProperty> ReadPage(int page, int itemsPerPage, string columnToOrder, List<OrganizationUnitToProperty> unitValues)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(procs[0], connection);
                AddSqlParemeters(page, itemsPerPage, tables[0], columnToOrder, command);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        ReaderReadSet(unitValues, reader);
                    }
                    reader.Close();
                }
            }
            return unitValues;
        }
        public List<OrganizationUnit> ReadPage(int page, int itemsPerPage, string columnToOrder, List<OrganizationUnit> units)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(procs[0], connection);
                AddSqlParemeters(page, itemsPerPage, tables[1], columnToOrder, command);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        ReaderReadSet(units, reader);
                    }
                    reader.Close();
                }
            }
            return units;
        }
        public List<Property> ReadPage(int page, int itemsPerPage, string columnToOrder, List<Property> props)
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(procs[0], connection);
                AddSqlParemeters(page, itemsPerPage, tables[2], columnToOrder, command);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        ReaderReadSet(props, reader);
                    }
                    reader.Close();
                }
            }
            return props;
        }
        private void AddSqlParemeters(int page, int itemsPerPage, string table, string columnToOrder, SqlCommand command)
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(AddSqlParameter("@Page", page));
            command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
            command.Parameters.Add(AddSqlParameter("@TableName", table));
            command.Parameters.Add(AddSqlParameter("@ColumnToOrderBy", columnToOrder));
        }
        private SqlParameter AddSqlParemeters(int itemsPerPage,string table, SqlCommand command)
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(AddSqlParameter("@ItemsPerPage", itemsPerPage));
            command.Parameters.Add(AddSqlParameter("@TableName", table));
            SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;
            return returnValue;
        }
        private void AddSqlParemeters(string identity, SqlCommand command)
        {
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(AddSqlParameter("@Identity", identity));
        }
        private SqlParameter AddSqlParameter(string name, string value)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                Value = value
            };
            return param;
        }
        private SqlParameter AddSqlParameter(string name, int value)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = name,
                Value = value
            };
            return param;
        }
        private void ReaderReadSet(List<OrganizationUnitToProperty> values, SqlDataReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsDBNull(1))
                {
                    values.Add(new OrganizationUnitToProperty
                    {
                        OrganizationUnitIdentity = reader.GetString(0),
                        PropertyName = null,
                        Value = null
                    });
                    continue;
                }
                values.Add(new OrganizationUnitToProperty
                {
                    OrganizationUnitIdentity = reader.GetString(0),
                    PropertyName = reader.GetString(1),
                    Value = reader.GetString(2)
                });
            }
        }
        private void ReaderReadSet(List<OrganizationUnit> units, SqlDataReader reader)
        {
            while (reader.Read())
            {
                units.Add(new OrganizationUnit
                {
                    Identity = reader.GetString(0),
                    Description = reader.GetString(1),
                    IsVirtual = reader.GetBoolean(2),
                    ParentIdentity = reader.GetString(3)
                });
            }
        }
        private void ReaderReadSet(List<Property> props, SqlDataReader reader)
        {
            while (reader.Read())
            {
                props.Add(new Property
                {
                    Name = reader.GetString(0),
                    Type = reader.GetString(1)
                });
            }
        }
        StringBuilder FormatInsertCommandSql(Dictionary<string, OrganizationUnit> orgUnits)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("INSERT INTO {0} VALUES ", tables[1]);
            foreach (var orgUnit in orgUnits)
            {
                strBuilder.AppendFormat("('{0}', '{1}', '{2}', '{3}'), ", orgUnit.Key, orgUnit.Value.Description, orgUnit.Value.IsVirtual, orgUnit.Value.ParentIdentity);
            }
            strBuilder.Remove(strBuilder.Length - 2, 2);
            strBuilder.Append(" ");
            return strBuilder;
        }
        StringBuilder FormatInsertCommandSql(Dictionary<string, Property> props)
        {
            var propsStrBuilder = new StringBuilder();
            propsStrBuilder.AppendFormat("INSERT INTO {0} VALUES ", tables[2]);
            foreach (var prop in props)
            {
                propsStrBuilder.AppendFormat("('{0}', '{1}'), ", prop.Key, prop.Value.Type);
            }
            propsStrBuilder.Remove(propsStrBuilder.Length - 2, 2);
            propsStrBuilder.Append(" ");
            return propsStrBuilder;
        }
        StringBuilder FormatInsertCommandSql(IDictionary<string, OrganizationUnitToProperty> orgUnitToProps)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("INSERT INTO {0} VALUES ", tables[0]);
            foreach (var orgUnitToProp in orgUnitToProps)
            {
                strBuilder.AppendFormat("('{0}', '{1}', '{2}'), ", orgUnitToProp.Value.OrganizationUnitIdentity, orgUnitToProp.Value.PropertyName, /*Regex.Unescape(*/orgUnitToProp.Value.Value/*)*/);
            }
            strBuilder.Remove(strBuilder.Length - 2, 2);
            strBuilder.Append(" ");
            return strBuilder;
        }
        string FormatFullInsertCommandSql(Dictionary<string, OrganizationUnit> orgUnits, Dictionary<string, Property> props, IDictionary<string, OrganizationUnitToProperty> orgUnitToProps)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(FormatInsertCommandSql(orgUnits));
            strBuilder.Append(FormatInsertCommandSql(props));
            strBuilder.Append(FormatInsertCommandSql(orgUnitToProps));
            return strBuilder.ToString();
        }
        string FormatFullDeleteCommandSql()
        {
            var strBuilder = new StringBuilder();
            foreach (var table in tables)
                strBuilder.AppendFormat("DELETE FROM {0}; ", table);
            return strBuilder.ToString();
        }
        string FormatFullSelectCommandSql()
        {
            var strBuilder = new StringBuilder();
            foreach (var table in tables)
            {
                strBuilder.AppendFormat("SELECT * FROM {0}; ", table);
            }
            return strBuilder.ToString();
        }
    }
}
