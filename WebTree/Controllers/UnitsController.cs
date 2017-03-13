using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tree;
using WebTree.Models;

namespace WebTree.Controllers
{
    public class UnitsController : ApiController
    {
        private OperationWithDb _operationWithDB;
        int _pageCount;
        int itemsPerPage = 2;
        // GET: api/Units
        public UnitsController()
        {
            _operationWithDB = new OperationWithDb();
            _pageCount = _operationWithDB.CountUnitPages(itemsPerPage);
        }
        public int Get()
        {
            return _pageCount;
        }

        // GET: api/Units/5
        public List<OrganizationUnit> Get(int id)
        {
             return _operationWithDB.ReadPage(id, itemsPerPage, "[Identity]", new List<OrganizationUnit>()); 
        }

        // POST: api/Units
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Units/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Units/5
        public void Delete(int id)
        {
        }
    }
}
