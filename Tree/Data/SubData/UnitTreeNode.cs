using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree.Data.SubData
{
    public class UnitTreeNode
    {
        public UnitTreeNode()
        {

        }
        public UnitTreeNode(List<OrganizationUnit> items, string parentIdentity)
        {
            Init(items, parentIdentity, this);
        }
        public string Identity { get; set; }
        public string Description { get; set; }
        public bool IsVirtual { get; set; }
        public string ParentIdentity { get; set; }
        public List<UnitTreeNode> Children { get; set; }
         void Init(List<OrganizationUnit> items, string parentIdentity, UnitTreeNode node)
        {            
            node.Children = new List<UnitTreeNode>();
            foreach (var item in items.Where(i => i.ParentIdentity == parentIdentity))
            {
                node.Children.Add(new UnitTreeNode
                {
                    Identity = item.Identity,
                    Description = item.Description,
                    IsVirtual = item.IsVirtual,
                    ParentIdentity = item.ParentIdentity,
                    Children = null
                });
            }
            if (node.Children != null)
                foreach (var branch in node.Children)
                    Init(items, branch.Identity, branch);
        }
    }
}
