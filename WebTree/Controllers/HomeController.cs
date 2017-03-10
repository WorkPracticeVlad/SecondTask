﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tree;

namespace WebTree.Controllers
{

    public class HomeController : Controller
    {

        public ActionResult Index()
        {           
            ViewBag.Title = "Home Page";
            var cr = new TreeCrawler();
            cr.EnterEnvironment(ConfigurationManager.AppSettings["pathToEnviroment"]);        
            return View();
        }
    }
}
