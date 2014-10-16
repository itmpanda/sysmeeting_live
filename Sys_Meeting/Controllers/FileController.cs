using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sys_Meeting.Controllers
{
    public class FileController : Controller
    {
        //
        // GET: /File/

        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Upload()
        {
            foreach (string upload in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[upload];
                if (file != null && file.ContentLength > 0)
                {
                    string filePath = AppDomain.CurrentDomain.BaseDirectory + "uploads/";
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    string ext = Path.GetExtension(file.FileName);
                    string newName = DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString()+ext;
                    newName = filePath + newName;
                    file.SaveAs(newName);
                }
            }
            return Content("1");
        }
        
    }
}
