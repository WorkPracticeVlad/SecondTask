using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tree;
using Tree.Data;
using Tree.Data.SubData;
using Tree.Manage;
using WebTree.Models;

namespace WebTree.Controllers
{
    public class UnitsController : ApiController
    {
        private Manager _manager;
        int _itemsPerNode = 1;
        public UnitsController()
        {
            _manager = new Manager();
        }
        [HttpGet]
        [Route("api/units/childrenbyparent/{id}")]
        public List<OrganizationUnit> ChildrenByParent(string id = "")
        {
            return _manager.OrgUnitsRepository.ReadChildrenFromDb(id.Replace('-','.'));
        }       
        [HttpGet]
        [Route("api/units/branchesfiltered/{id}")]
        public List<UnitTreeNode> BranchesFiltered(string id = "")
        {
            return _manager.OrgUnitsRepository.ReadBranchesFilteredFromDb(id.Replace('-', '.'));
        }
        [HttpGet]
        [Route("api/units/rowinnode/{id}/{page}")]
        public List<OrgUnitWithPagesCount> RowInNode(string id , int page)
        {
            return _manager.OrgUnitsRepository.ReadUnitNodePageFromDb(id.Replace('-', '.'),page,_itemsPerNode);
        }
    }
}
