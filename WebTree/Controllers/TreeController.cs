using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tree;
using Tree.Manage;
using WebTree.Models;

namespace WebTree.Controllers
{
    public class TreeController : ApiController
    {
        private Manager _manager;

        public TreeController()
        {
           _manager = new Manager();
        }
        public TreeModel Get()
        {
            return new TreeModel { OrgUnits = _manager.OrgUnitsRepository.ReadDataFromDb(),
                Values = _manager.ValuesRepository.ReadDataFromDb(),
                Properties = _manager.PropertiesRepository.ReadDataFromDb()
            };
        }
        public void Delete()
        {
            _manager.RefreshTree();
        }      
    }
}
