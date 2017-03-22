using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tree;
using Tree.Data;
using Tree.Manage;
using WebTree.Models;

namespace WebTree.Controllers
{
    public class UnitsController : ApiController
    {
        private Manager _manager;
        int _itemsPerPage = 5;
        public UnitsController()
        {
            _manager = new Manager();
        }
        public int Get()
        {
            return _manager.OrgUnitsRepository.CountPages(_itemsPerPage);
        }
        public List<OrganizationUnit> Get(int id)
        {
            return _manager.OrgUnitsRepository.ReadPageFromDb(id, "[Identity]",_itemsPerPage);  
        }
        [Route("api/units/get/{id}/{identity}")]
        public List<OrganizationUnitToProperty> Get(int id, string identity)
        {
            return _manager.ValuesRepository.ReadOrganizationUnitValuesFromDb(identity.Replace('-', '.'));
        }
        [HttpGet]
        [Route("api/units/childrenbyparent/{id}")]
        public List<OrganizationUnit> ChildrenByParent(string id = "")
        {
            return _manager.OrgUnitsRepository.ReadChildrenFromDb(id.Replace('-','.'));
        }
    }
}
