using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sys_Meeting.generalHandler;
using Sys_Meeting.Models;

namespace Sys_Meeting.Controllers
{
    
    public class Columns
    {
        public  string field { get; set; }
        public string title { get; set; }
        public int width { get; set; }
        public bool checkbox { get; set; }
    }

    public class JsonData
    {
        public Columns columns { get; set; }
        public string rows { get; set; }
    }

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
            //return View();
            
            string sql = "";
            sql = DbCommon.GetPageSql("tb_list", "sys_id ,list_id ,title", "sys_id");
            DataSet ds = SqlHelper.ExecuteDataset(DbCommon.GConnectionString, CommandType.Text, sql
                , new SqlParameter("@pagenum", 1)
                , new SqlParameter("@pagesize", DbCommon.GetPageSize));

            //string json = DataTableConvertJson.Dataset2Json(ds);
            
            List<MeetListModels> listRows =new List<MeetListModels>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                listRows.Add(new MeetListModels()
                {
                    ListSysId = dr["sys_id"].ToString(),
                    ListId = dr["list_id"].ToString(),
                    ListName = dr["title"].ToString()
                });
            }
            
            List<Columns> cols = new List<Columns>();
            cols.Add(new Columns() { field = "ck", title = "", width = 50, checkbox = true });
            cols.Add(new Columns() { field = "ListSysId", title = "", width = 50, checkbox = true });
            cols.Add(new Columns() { field = "ListId", title = "事項編號", width = 100, checkbox = false });
            cols.Add(new Columns() { field = "ListName", title = "事項標題", width = 100, checkbox = false });
            
            return Json(new {columns = cols,rows=listRows,total=30});

            //string jsoncolumns = "";
            //string jsonrows = json;
            //return Content(json);
        }
    }
}
