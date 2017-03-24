using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree.Data.SubData
{
    public class ForOrgUnitProperties
    {
        public List<OrgUnitIdentityTailPair> Header { get; set; }
        public List<PropertyToUnitsValuePairs> Data { get; set; }
    }
}
