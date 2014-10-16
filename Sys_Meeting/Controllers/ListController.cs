using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBCommon;
using Sys_Meeting.Models;

namespace Sys_Meeting.Controllers
{


    public class ListController : Controller
    {
        //
        // GET: /List/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {   
            
            string sql = "";
            sql = DBCommon.SqlHelper.GetPageSql("tb_list", "sys_id ,list_id ,title", "sys_id");
            DataSet ds = DBCommon.SqlHelper.ExecuteDataset(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql
                , new SqlParameter("@pagenum", 1)
                , new SqlParameter("@pagesize", SqlHelper.GetPageSize));

            List<MeetListModels> listRows =new List<MeetListModels>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                listRows.Add(new MeetListModels()
                {
                    CK = 0,
                    ListSysId = dr["sys_id"].ToString(),
                    ListId = dr["list_id"].ToString(),
                    ListName = dr["title"].ToString()
                });
            }
            
            return Json(new { total = 30, rows = listRows});
        }
    }
}
