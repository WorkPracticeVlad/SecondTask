using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    public interface IOperationWithDb
    {
        List<OrganizationUnit> OrgUnints { get; }
        List<Property> Props { get; }
        List<OrganizationUnitToProperty> OrgUnitToProps { get; }
        int InsertTreeToDb(string path);
        int DeleteTreeFromDb();
        void ReadTreeFromDb();
    }
}
