using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Sys_Meeting.generalHandler;
using Sys_Meeting.Models;

namespace Sys_Meeting.Controllers
{
    public class MeetController : Controller
    {
        //
        // GET: /Meet/
        //public string gConnectionString = SqlHelper.ReportCentreConnectionString;
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View("Index");
        }

        [HttpPost]
        public ActionResult Add(MeetModels meetModels)
        {
            if (ModelState.IsValid)
            {
                string content = meetModels.Addr + meetModels.IdCard + meetModels.Name;
                return Content(content);
            }
            else
            {
                return Content("error!");
            }
        }

        //[HttpGet]
        //public ActionResult List()
        //{
        //    return View("List");
        //}
        public bool CheckIsLogin()
        {
            if (Session["userid"] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string GetErrDescription(int ErrNum)
        {
            string sErrMsg = "";
            switch (ErrNum)
            {
                case -1:
                    sErrMsg = "登陸超時,請重新登陸！";
                    break;
            }
            return sErrMsg;
        }

        [HttpGet]
        public ActionResult ListDetail()
        {
            return View("ListDetail");
        }

        [HttpPost]
        public ActionResult ListDetail(MeetListModels meetListModels)
        {
            string sErrmsg = "";
            string guid = Guid.NewGuid().ToString().ToUpper();
            //string resultSuc = meetListModels.ListId + "," + meetListModels.ListName;
            string sql = "";
            int returnSuc = 0;

            if (Session["userid"] == null)
            {
                returnSuc = -1;
                sErrmsg = GetErrDescription(returnSuc);//"登陸超時,請重新登陸！";
                //return RedirectToAction("Login", "Account");
            }
            else
            {
                try
                {
                    if (meetListModels.ListAction == "create")
                    {
                        sql =
                            "insert into tb_list(sys_id,list_id,title,create_by) values(@sys_id,@list_id,@title,@create_by)";

                        returnSuc = SqlHelper.ExecuteNonQuery(SqlHelper.ReportCentreConnectionString, CommandType.Text,
                            sql
                            , new SqlParameter("@sys_id", guid)
                            , new SqlParameter("@list_id", meetListModels.ListId)
                            , new SqlParameter("@title", meetListModels.ListName)
                            , new SqlParameter("@create_by", Session["userid"].ToString()));
                    }
                    else if (meetListModels.ListAction == "edit")
                    {
                        sql =
                            "update tb_list set title=@title,modi_by=@modi_by,modi_dte=@modi_dte where sys_id=@sys_id and is_del=0";
                        returnSuc = SqlHelper.ExecuteNonQuery(SqlHelper.ReportCentreConnectionString, CommandType.Text,
                            sql
                            , new SqlParameter("@title", meetListModels.ListName)
                            , new SqlParameter("@modi_by", Session["userid"].ToString())
                            , new SqlParameter("@modi_dte", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"))
                            , new SqlParameter("@sys_id", meetListModels.ListSysId));
                    }

                }
                catch (Exception e)
                {
                    sErrmsg = e.Message;
                    //throw;
                }
            }
            return Json(new { result = returnSuc, errmsg = sErrmsg });
            //return Json("result:" + result);
            //if (string.IsNullOrEmpty(meetListModels.ListName))
            //{
            //    ModelState.AddModelError("ListNameLoss","請輸入事項標題!");
            //    return View(meetListModels);
            //}
            //return Content(meetListModels.ListId);
            //return View("ListDetail");
        }

        //[HttpPost]
        //public ActionResult List(string listId,string listName)
        //{
        //    string connString = SqlHelper.ReportCentreConnectionString;
        //    //int page=1,int rows=1
        //    string sql = "";
        //    sql = "select * from(";
        //    sql += "select top (@pagenum * @pagesize) ROW_NUMBER() OVER (ORDER BY wor_id) AS rownum, wor_id id,ful_name name,age FROM empmas WHERE wor_num like 'ab%'";
        //    sql += ") as tb";
        //    sql += " where rownum between ( @pagenum - 1 )* @pagesize + 1 AND (@pagenum*@pagesize) order by rownum";

        //    //string sql = "select top 1000 wor_id as id,FUL_NAME as [name],age from empmas";
        //    int page = 1;
        //    int rows = 10;

        //    DataSet ds = SqlHelper.ExecuteDataset(connString, CommandType.Text, sql, new SqlParameter("@pagesize", rows), new SqlParameter("@pagenum", page));
        //    //SqlDataReader dr = UGSC.ReportCentre.Services.SqlHelper.ExecuteReader(connString, CommandType.Text, "select top 1 * from empmas");
        //    int dscount = ds.Tables[0].Rows.Count;
        //    //string json = JsonConvert.SerializeObject(ds, Formatting.None);

        //    string json = DataTableConvertJson.Dataset2Json(ds);


        //    //json = dt.ToJson();// Json(dt).ToString();
        //    json = "{\"total\":30,\"rows\":" + json + "}";

        //    string content = listId+ listName;
        //    //return View("List");
        //    return Content(json);
        //}

        public ActionResult New()
        {
            return View("New");
        }

        //獲取事項列表
        public ActionResult GetList()
        {
            string sql = "";
            sql = "select * from(";
            sql += "select top (@pagenum * @pagesize) ROW_NUMBER() OVER (ORDER BY sys_id) AS rownum, sys_id,list_id,title FROM tb_list WHERE 1=1 and is_del=0 ";
            sql += ") as tb";
            sql += " where rownum between ( @pagenum - 1 )* @pagesize + 1 AND (@pagenum*@pagesize) order by rownum";

            int page = 1;
            int rows = 10;

            DataSet ds = SqlHelper.ExecuteDataset(DbCommon.GConnectionString, CommandType.Text, sql, new SqlParameter("@pagesize", rows), new SqlParameter("@pagenum", page));

            string json = DataTableConvertJson.Dataset2Json(ds);
            json = "{\"total\":30,\"rows\":" + json + "}";
            return Content(json);
        }

        public ActionResult GetListItem(string sysid)
        {

            //string connString = SqlHelper.ReportCentreConnectionString;
            string sql = "select top 1 sys_id,list_id,title from tb_list where is_del=0 and sys_id=@sys_id";
            DataSet ds = SqlHelper.ExecuteDataset(DbCommon.GConnectionString, CommandType.Text, sql
                , new SqlParameter("@sys_id", sysid));
            string returnJson = DataTableConvertJson.Dataset2Json(ds);
            int returnSuc = ds.Tables[0].Rows.Count;
            //returnJson = "{\result\":"+returnSuc+":" + returnJson + "}";
            string sErrmsg = "";
            string sListName = "";
            string sListId = "";
            if (returnSuc == 0)
            {
                sErrmsg = "事項不存在！";
            }
            else
            {
                sListId = ds.Tables[0].Rows[0]["list_id"].ToString();
                sListName = ds.Tables[0].Rows[0]["title"].ToString();
            }

            return Json(new { result = returnSuc, errmsg = sErrmsg,listid=sListId, listname = sListName });
            //return Content(returnJson);
        }

        public ActionResult DelListItem(string sysid)
        {
            int returnSuc = 0;
            string sql = "update tb_list set is_del=1,del_by=@del_by,del_dte=@del_dte where sys_id=@sys_id and is_del=0";
            string sErrmsg = "";
            if (CheckIsLogin())
            {
                try
                {
                    returnSuc = SqlHelper.ExecuteNonQuery(DbCommon.GConnectionString, CommandType.Text, sql
                        , new SqlParameter("@sys_id", sysid)
                        , new SqlParameter("@del_by", Session["userid"].ToString())
                        , new SqlParameter("@del_dte", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
                }
                catch (Exception e)
                {
                    sErrmsg = e.Message;
                }
            }
            else
            {
                returnSuc = -1;
                sErrmsg = GetErrDescription(returnSuc);
            }
            return Json(new { result = returnSuc, errmsg = sErrmsg });
        }

        //搜索
        public ActionResult SearchList(string wd,string t)
        {
            string swhere = "";
            if (t == "事項編號")
            {
                swhere = " and (list_id like @wd)";
            }
            else if (t == "事項名稱")
            {
                swhere = " and (title like @wd)";
            }
            else
            {
                swhere = " and (1=1)";
            }

            string sql = "";
            sql = "select * from(";
            sql +=
                "select top (@pagenum * @pagesize) ROW_NUMBER() OVER (ORDER BY sys_id) AS rownum, sys_id,list_id,title FROM tb_list WHERE 1=1 and is_del=0 " +
                swhere;
            sql += ") as tb";
            sql += " where rownum between ( @pagenum - 1 )* @pagesize + 1 AND (@pagenum*@pagesize) order by rownum";

            int page = 1;
            int rows = 10;

            DataSet ds = SqlHelper.ExecuteDataset(DbCommon.GConnectionString, CommandType.Text, sql
                , new SqlParameter("@pagesize", rows)
                , new SqlParameter("@pagenum", page)
                , new SqlParameter("@wd", "%" + wd + "%"));

            string json = DataTableConvertJson.Dataset2Json(ds);
            json = "{\"total\":"+ds.Tables[0].Rows.Count+",\"rows\":" + json + "}";
            return Content(json);
        }
    }
}
