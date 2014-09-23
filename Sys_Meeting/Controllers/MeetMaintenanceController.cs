using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sys_Meeting.generalHandler;

namespace Sys_Meeting.Controllers
{
    public class MeetMaintenanceController : Controller
    {
        //
        // GET: /MeetMaintenance/

        public ActionResult Index()
        {
            return View();
        }

        ///<summary>
        /// @20140922
        /// 獲取申請會議記錄
        /// </summary>
        /// <returns>JSON格式數據</returns>
        [HttpGet]
        public ActionResult GetMeet()
        {
            string sql = "";
            sql = "select * from(";
            sql += "select top (@pagenum * @pagesize) ROW_NUMBER() OVER (ORDER BY sys_id) AS rownum, sys_id,mt_id," +
                   "title,mt_dte,mt_time,addr from tb_meet WHERE 1=1 and is_del=0 ";
            sql += ") as tb";
            sql += " where rownum between ( @pagenum - 1 )* @pagesize + 1 AND (@pagenum*@pagesize) order by rownum";

            int page = 1;
            int rows = 10;
            
            DataSet ds = SqlHelper.ExecuteDataset(DbCommon.GConnectionString, CommandType.Text, sql
                , new SqlParameter("@pagesize", rows)
                , new SqlParameter("@pagenum", page));

            string json = DataTableConvertJson.Dataset2Json(ds);
            json = "{\"total\":30,\"rows\":" + json + "}";
            return Content(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Action()
        {
            return Json(new {result = "1"});
        }

        public ActionResult TestSaveGrid()
        {
            return View();
        }
    }
}
