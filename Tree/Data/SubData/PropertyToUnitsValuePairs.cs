using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree.Data.SubData
{
    public class PropertyToUnitsValuePairs
    {
        public string Property { get; set; }
        public List<OrgUnitValuePair> UnitsToValues { get; set; }
    }
}
