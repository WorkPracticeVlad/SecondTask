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
    public class TreeController : ApiController
    {
        string pathToEnviroment = ConfigurationManager.AppSettings["pathToEnviroment"];
        private IOperationWithDb _operationWithDB;
        public TreeController()
        {
            _operationWithDB = new OperationWithDb();
        }
        public TreeModel Get()
        {
            _operationWithDB.ReadTreeFromDb();
            return new TreeModel { OrgUnits = _operationWithDB.OrgUnints, OrgUnitToProps = _operationWithDB.OrgUnitToProps, Props = _operationWithDB.Props };
        }

        public string Get(int id)
        {
            return "value";
        }

        public void Post([FromBody]string value)
        {
        }
        public void Put(int id, [FromBody]string value)
        {
            
        }
        public bool Delete()
        {
            _operationWithDB.DeleteTreeFromDb();
            _operationWithDB.InsertTreeToDb(pathToEnviroment);
            return true;
        }
        public void Delete(int id)
        {
        }
    }
}
