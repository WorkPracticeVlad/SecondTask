using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tree;
using Tree.Manage;

namespace WebTree.Controllers
{
    public class TreeController : ApiController
    {
        private Manager _manager;

        public TreeController()
        {
           _manager = new Manager();
        }        
        public void Delete()
        {
            _manager.RefreshTree();
        }      
    }
}
