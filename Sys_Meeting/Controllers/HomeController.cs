using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sys_Meeting.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewData["key"] = "this is viewdata value";
            return View();
        }
        public ActionResult Admin()
        {

            return View("admin");
        }

        public ActionResult menu()
        {
            return View("menu");
        }

        public ActionResult lanmu_Menu()
        {
            return View("lanmu_Menu");
        }

        public ActionResult ajaxHandler()
        {
            return View("ajaxHandler");
        }

        public ActionResult grid()
        {
            return View("grid");
        }
    }
}
