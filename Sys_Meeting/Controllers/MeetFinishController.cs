using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sys_Meeting.Controllers
{
    public class MeetFinishController : Controller
    {
        //
        // GET: /MeetFinish/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save()
        {
            return Content("1");
        }

        public ActionResult SearchResult()
        {
            return View();
        }

        
    }
}
