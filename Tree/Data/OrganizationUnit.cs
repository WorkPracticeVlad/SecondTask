using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree.Data
{
    public class OrganizationUnit 
    {
        public string Identity { get; set; }
        public string Description { get; set; }
        public bool IsVirtual { get; set; }
        public string ParentIdentity { get; set; }
    }
}
