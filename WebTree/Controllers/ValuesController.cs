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

namespace WebTree.Controllers
{
    public class ValuesController : ApiController
    {
        private Manager _manager;

        public ValuesController()
        {
            _manager = new Manager();
        }
        // GET: api/Values
        public int Get()
        {
            return _manager.ValuesRepository.CountPages();
        }

        // GET: api/Values/5
        public List<OrganizationUnitToProperty> Get(int id)
        {
            return _manager.ValuesRepository.ReadPageFromDb(id, "[OrganizationUnitIdentity]");
        }
    }
}
