using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tree;
using Tree.Data;
using Tree.DB;

namespace WebTree.Controllers
{

    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            
            ViewBag.Title = "Home Page"; 
            return View();
        }
        public ActionResult TreePages()
        {
            return View();
        }
    }
}
