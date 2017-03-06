using System;
using System.Collections.Generic;
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
            var intWDB = new InteractionWithDb(@"Data Source=WS-INTERN-ORLOV\SQLEXPRESS;Initial Catalog=SecondTask;Integrated Security=True", new TreeCrawler());
            intWDB.InsertTreeToDb(@"C:\Users\vorlov\Desktop\FolderStaff\test\environment.config");
            return View();
        }
    }
}
