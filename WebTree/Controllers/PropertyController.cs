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
        [HttpGet]
        public int PagesByFilter(string id="")
        {
            return _manager.PropertiesRepository.CountPages(_itemsPerPage,id);
        }      
        public List<Property> Get(int id)
        {
            return _manager.PropertiesRepository.ReadPropertyWithUsagePageFromDb(id, "[Name]", _itemsPerPage,"");
        }
        [Route("api/property/get/{id}/{filter}")]
        public List<Property> Get(int id, string filter)
        {
            return _manager.PropertiesRepository.ReadPropertyWithUsagePageFromDb(id, "[Name]", _itemsPerPage,filter);
        }
    }
}
