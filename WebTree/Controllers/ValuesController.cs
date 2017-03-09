using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tree;
using WebTree.Models;

namespace WebTree.Controllers
{
    public class ValuesController : ApiController
    {
        string pathToEnviroment = ConfigurationManager.AppSettings["pathToEnviroment"];   
        private IOperationWithDb _operationWithDB;
        public ValuesController()
        {
            _operationWithDB = new OperationWithDb();
        }               
        // GET api/values
        public TreeModel Get()
        {
            _operationWithDB.ReadTreeFromDb();
            return new TreeModel { OrgUnits = _operationWithDB.OrgUnints, OrgUnitToProps = _operationWithDB.OrgUnitToProps, Props = _operationWithDB.Props }; 
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        public bool Delete()
        {
            _operationWithDB.DeleteTreeFromDb();
            _operationWithDB.InsertTreeToDb(pathToEnviroment);
            return true;
        }
        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
