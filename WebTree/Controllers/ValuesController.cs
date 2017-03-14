using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tree;
using Tree.Data;

namespace WebTree.Controllers
{
    public class ValuesController : ApiController
    {
        private OperationWithDb _operationWithDB;
        int _pageCount;
        int itemsPerPage = 2;
        public ValuesController()
        {
            _operationWithDB = new OperationWithDb();
            _pageCount = _operationWithDB.CountValuesPages(itemsPerPage);
        }
        // GET: api/Values
        public int Get()
        {
            return  _pageCount;
        }

        // GET: api/Values/5
        public List<OrganizationUnitToProperty> Get(int id)
        {
            return _operationWithDB.ReadPage(id, itemsPerPage, "[Value]", new List<OrganizationUnitToProperty>());
        }

        // POST: api/Values
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Values/5
        public void Delete(int id)
        {
        }
    }
}
