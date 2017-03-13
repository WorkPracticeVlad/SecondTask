using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tree;

namespace WebTree.Controllers
{
    public class PropertyController : ApiController
    {
        private OperationWithDb _operationWithDB;
        int _pageCount;
        int itemsPerPage = 2;
        // GET: api/Property
        public PropertyController()
        {
            _operationWithDB = new OperationWithDb();
            _pageCount = _operationWithDB.CountPropertyPages(itemsPerPage);
        }
        // GET: api/Property
        public int Get()
        {
            return _pageCount;
        }

        // GET: api/Property/5
        public List<Property> Get(int id)
        {
            return _operationWithDB.ReadPage(id, itemsPerPage, "[Name]", new List<Property>());
        }

        // POST: api/Property
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Property/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Property/5
        public void Delete(int id)
        {
        }
    }
}
