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
    public class PropertyController : ApiController
    {
        private Manager _manager;
        int _itemsPerPage = 5;
        public PropertyController()
        {
             _manager=new Manager();
        }
        // GET: api/Property
        public int Get()
        {
            return _manager.PropertiesRepository.CountPages(_itemsPerPage);
        }

        // GET: api/Property/5
        public List<Property> Get(int id)
        {
            return _manager.PropertiesRepository.ReadPageFromDb(id, "[Name]", _itemsPerPage);
        }
    }
}
