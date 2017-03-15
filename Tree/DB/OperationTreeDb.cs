using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree.Data;

namespace Tree.DB
{
    public class OperationTreeDb
    {
        List<OrganizationUnit> _units = new List<OrganizationUnit>();
        List<Property> _properties = new List<Property>();
        List<OrganizationUnitToProperty> _values = new List<OrganizationUnitToProperty>();
        public List<OrganizationUnit> OrgUnints { get { return _units; } }
        public List<Property> Props { get { return _properties; } }
        public List<OrganizationUnitToProperty> OrgUnitToProps { get { return _values; } }
        string _connString = ConfigurationManager.ConnectionStrings["SecondTaskConnection"].ConnectionString;
        string _selectAllFromTables = "[dbo].[SelectAllFromTables]";
        string _deleteAllFromTables = "[dbo].[DeleteAllFromTables]";
        string _insertOrganizationUnits = "[dbo].[InsertOrganizationUnits]";
        public void ReadTreeFromDb()
        {
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_selectAllFromTables, connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        _units.Add(new OrganizationUnit
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
                            _properties.Add(new Property
                            {
                                Name = reader.GetString(0),
                                Type = reader.GetString(1)
                            });
                        }
                    if (reader.NextResult())
                        while (reader.Read())
                        {
                            _values.Add(new OrganizationUnitToProperty
                            {
                                OrganizationUnitIdentity = reader.GetString(0),
                                PropertyName = reader.GetString(1),
                                Value = reader.GetString(2)
                            });
                        }
                }
                reader.Close();
            }
        }
        public int DeleteTreeFromDb()
        {
            var numberAffectedRows = 0;
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_deleteAllFromTables, connection);
                command.CommandType = CommandType.StoredProcedure;
                numberAffectedRows = command.ExecuteNonQuery();
            }
            return numberAffectedRows;
        }
        public int InsertOrgUnitsToDb(List<OrganizationUnit> units)
        {
            var numberAffectedRows = 0;
            var dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("[Identity]", typeof(string)),
                    new DataColumn("[Description]", typeof(string)),
                    new DataColumn("[IsVirtual]",typeof(bool)),
                    new DataColumn("[ParentIdentity]", typeof(string))});
            foreach (var unit in units)
            {
                string identity = unit.Identity;
                string description = unit.Description;
                bool isVirtual = unit.IsVirtual;
                string parentIdentity = unit.ParentIdentity;
                dt.Rows.Add(identity, description, isVirtual, parentIdentity);
            }
            var sqlParam = new SqlParameter("@OrgUnits", SqlDbType.Structured);
            sqlParam.Value = dt;
            using (SqlConnection connection = new SqlConnection(_connString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(_insertOrganizationUnits, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(sqlParam);
                numberAffectedRows = command.ExecuteNonQuery();
            }
            return numberAffectedRows;          
        }
    }
}
