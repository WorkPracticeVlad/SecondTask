using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree.Data
{
    public class OrganizationUnitToProperty : IStich
    {
        public string OrganizationUnitIdentity { get; set; }
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }
}
