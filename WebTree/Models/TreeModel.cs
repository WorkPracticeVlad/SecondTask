using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tree;
using Tree.Data;

namespace WebTree.Models
{
    public class TreeModel
    {
        public List<OrganizationUnit> OrgUnits { get; set; }
        public List<Property> Properties { get; set; } 
        public List<OrganizationUnitToProperty> Values { get; set; } 
    }
}