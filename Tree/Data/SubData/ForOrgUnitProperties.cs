using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree.Data.SubData
{
    public class ForOrgUnitProperties
    {
        public ForOrgUnitProperties(List<OrganizationUnitToProperty> items, List<string> orgUnitsId)
        {
            var forOrgUnitProperties = this;
            forOrgUnitProperties.Header = new List<OrgUnitIdentityTailPair>();
            forOrgUnitProperties.Data = new List<PropertyToUnitsValuePairs>();
            foreach (var item in orgUnitsId)
            {
                forOrgUnitProperties.Header.Add(new OrgUnitIdentityTailPair { Identity = item, Tail = item.Substring(item.LastIndexOf('.')) });
            }
            foreach (var item in items.Select(p => p.PropertyName).Distinct().ToList())
            {
                var listUnitsToValues = new List<OrgUnitValuePair>();
                foreach (var orgUnitId in orgUnitsId)
                {
                    listUnitsToValues.Add(new OrgUnitValuePair { OrgUnitIdentity = orgUnitId });
                }
                foreach (var i in items.ToList())
                {
                    if (i.PropertyName == item)
                        listUnitsToValues.Find(j => j.OrgUnitIdentity == i.OrganizationUnitIdentity).Value = i.Value;
                }
                forOrgUnitProperties.Data.Add(new PropertyToUnitsValuePairs
                {
                    Property = item,
                    UnitsToValues = listUnitsToValues
                });
            }
        }
        public List<OrgUnitIdentityTailPair> Header { get; set; }
        public List<PropertyToUnitsValuePairs> Data { get; set; }
    }
}
