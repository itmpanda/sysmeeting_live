using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;
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

        public bool CheckIsLogin()
        {
            return Session["userid"] != null;
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

                        returnSuc = DBCommon.SqlHelper.ExecuteNonQuery(DBCommon.SqlHelper.ConntionString, CommandType.Text,
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
                        returnSuc = DBCommon.SqlHelper.ExecuteNonQuery(DBCommon.SqlHelper.ConntionString, CommandType.Text,
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
        }

        //獲取事項列表
        public JsonResult GetList()
        {
            int totalrows = 0;
            var sql = new StringBuilder();
            sql.Append("exec sp_getPageData 'select @pagemark sys_id,list_id,title FROM tb_list WHERE 1=1 and is_del=0',@orderby,@pagenum,@pagesize");

            var page = 1;
            var rows = 10;

            if (!string.IsNullOrEmpty(Request.QueryString["page"]))
            {
                page = Convert.ToInt16(Request.QueryString["page"]);
            }
            if (!string.IsNullOrEmpty( Request.QueryString["rows"] ))
            {
                rows = Convert.ToInt16(Request.QueryString["rows"]);
            }
            //if (page == 1)
            //{
            SqlDataReader dr = DBCommon.SqlHelper.ExecuteReader(DBCommon.SqlHelper.ConntionString, CommandType.Text, "select count(1) totalrows FROM tb_list WHERE 1=1 and is_del=0");
            while (dr.Read())
            {
                totalrows = (int) dr["totalrows"];
            }
            //}
            DataSet ds = DBCommon.SqlHelper.ExecuteDataset(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql.ToString()
                , new SqlParameter("@pagesize", rows)//每頁顯示記錄的數量
                , new SqlParameter("@pagenum", page)//當前頁
                ,new SqlParameter("@orderby","sys_id"));

            var list = new ArrayList();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(new
                {
                    //rownum=r["rownum"].ToString(),
                    sys_id=r["sys_id"].ToString(),
                    list_id=r["list_id"].ToString(),
                    title=r["title"].ToString()
                });
            }
            //string json = DataTableConvertJson.Dataset2Json(ds);
            //json = "{\"total\":30,\"rows\":" + json + "}";
            //return Content(json);
            return Json(new { total = totalrows, rows = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListItem(string sysid)
        {

            //string connString = SqlHelper.ReportCentreConnectionString;
            string sql = "select top 1 sys_id,list_id,title from tb_list where is_del=0 and sys_id=@sys_id";
            DataSet ds = DBCommon.SqlHelper.ExecuteDataset(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql
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
                    returnSuc = DBCommon.SqlHelper.ExecuteNonQuery(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql
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

        //搜索事項
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

            DataSet ds = DBCommon.SqlHelper.ExecuteDataset(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql
                , new SqlParameter("@pagesize", rows)
                , new SqlParameter("@pagenum", page)
                , new SqlParameter("@wd", "%" + wd + "%"));

            string json = DataTableConvertJson.Dataset2Json(ds);
            json = "{\"total\":"+ds.Tables[0].Rows.Count+",\"rows\":" + json + "}";
            return Content(json);
        }

        //搜索會議記錄事項和內容
        //[HttpPost]
        public ActionResult Search()
        {
            return View("Search");
        }

        public JsonResult Searchmeet(SearchMeetModels searchMeetModels)
        {
            bool ret = false;
            int iTotal = 0, iRows = 0;
            string sErrmsg = "";

            if (Session["userid"] == null)
            {
                return Json(new {result = ret, total = iTotal, rows = iRows, errmsg = "登陸超時請重新登陸！"},
                    JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(searchMeetModels.Keywords))
            {
                return Json(new {result = ret, total = iTotal, rows = iRows, errmsg = "關鍵字為空！"},
                    JsonRequestBehavior.AllowGet);
            }

            var sql = new StringBuilder();

            var listItems = new List<MeetListContent>();

            string[] keywords= searchMeetModels.Keywords.Split(',');

            sql.Append(
                "select dtl.*,lst.title from tb_meet_detail dtl left join tb_list lst on dtl.dtl_id = lst.sys_id ");
            sql.Append("where exists(select 1 from tb_list where is_del=0 and dtl.dtl_id=sys_id) and (");
            for (int i = 0; i < keywords.Length; i++)
            {
                if (i == keywords.Length - 1)
                {
                    sql.Append("charindex('" + keywords[i] + "',title)>0");
                }
                else
                {
                    sql.Append("charindex('" + keywords[i] + "',title)>0 or ");
                }
            }
            sql.Append(") and exists(select 1 from tb_meet where is_del=0 and sys_id=dtl.mt_id ");

            //添加會議日期條件
            if (!string.IsNullOrEmpty(searchMeetModels.Meetdatefrom) &&
                !string.IsNullOrEmpty(searchMeetModels.Meetdateto))
            {
                sql.Append(" and mt_dte between '" + searchMeetModels.Meetdatefrom + "' and '" +
                           searchMeetModels.Meetdateto + "' ");
            }
            sql.Append(")");
            sql.Append("union ");
            sql.Append(
                "select dtl.*,lst.title from tb_meet_detail dtl left join tb_list lst on dtl.dtl_id = lst.sys_id ");
            sql.Append("where exists(select 1 from tb_list where is_del=0 and dtl.dtl_id=sys_id)");
            sql.Append("and exists(select 1 from tb_meet where is_del=0 and sys_id=dtl.mt_id ");
            
            //添加會議日期條件
            if (!string.IsNullOrEmpty(searchMeetModels.Meetdatefrom) &&
                !string.IsNullOrEmpty(searchMeetModels.Meetdateto))
            {
                sql.Append(" and mt_dte between '" + searchMeetModels.Meetdatefrom + "' and '" +
                           searchMeetModels.Meetdateto + "' ");
            }
            sql.Append(")");
            sql.Append("and (");
            for (int i = 0; i < keywords.Length; i++)
            {
                if (i == keywords.Length - 1)
                {
                    sql.Append("charindex('" + keywords[i] + "',dtl.detail)>0");
                }
                else
                {
                    sql.Append("charindex('" + keywords[i] + "',dtl.detail)>0 or ");
                }
            }
            sql.Append(")");

            //搜索tb_meet title
            sql.Append(" union ");
            sql.Append(
                "select dtl.*,lst.title from tb_meet_detail dtl left join tb_list lst on dtl.dtl_id=lst.sys_id where exists (select 1 from tb_meet where is_del=0 and sys_id=dtl.mt_id and (");
            for (int i = 0; i < keywords.Length; i++)
            {
                if (i == keywords.Length - 1)
                {
                    sql.Append("charindex('" + keywords[i] + "',title)>0)");
                }
                else
                {
                    sql.Append("charindex('" + keywords[i] + "',title)>0 or ");
                }
            }
            sql.Append(")");

            //加載會議事項
            DataSet dsDetail = DBCommon.SqlHelper.ExecuteDataset(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql.ToString());
            foreach (DataRow dr in dsDetail.Tables[0].Rows)
            {

                //加載行動列表
                var sb = new StringBuilder();
                sb.Append(
                    "select act.sys_id ,act.mt_id ,act.list_id ,act.detail ,convert(varchar(10),rpt_dte,101) rpt_dte ,personid=(select person.wor_num+',' from tb_list_act_person person where person.act_id=act.sys_id for xml path(''))");
                sb.Append(
                    ",personname=(select usr.ful_name+',' from tb_list_act_person person left join tb_user usr on person.wor_num=usr.wor_num where person.act_id=act.sys_id for xml path(''))");
                sb.Append("from tb_list_act act ");
                sb.Append("where act.mt_id=@mt_id");
                sb.Append(" and act.list_id=@list_id");

                DataSet dsActDetail = DBCommon.SqlHelper.ExecuteDataset(DBCommon.SqlHelper.ConntionString, CommandType.Text,
                    sb.ToString()
                    , new SqlParameter("mt_id", dr["mt_id"].ToString())
                    , new SqlParameter("list_id", dr["dtl_id"].ToString()));
                //listActDetail.Clear();
                var listActDetail = new List<ListDetailModels>();
                foreach (DataRow drAct   in dsActDetail.Tables[0].Rows)
                {
                    listActDetail.Add(new ListDetailModels()
                    {
                        Actcontent = drAct["detail"].ToString(),
                        Actpersonid = drAct["personid"].ToString().Substring(0, drAct["personid"].ToString().Length - 1),
                        Rptdte = drAct["rpt_dte"].ToString(),
                        Actpersonname =
                            drAct["personname"].ToString().Substring(0, drAct["personname"].ToString().Length - 1),
                        Listsysid = dr["dtl_id"].ToString(),
                        Meetsysid = dr["mt_id"].ToString()
                    });
                }
                //listActDetail.AddRange(from DataRow drAct in dsActDetail.Tables[0].Rows
                //                       select new ListDetailModels()
                //                       {
                //                           Actcontent = drAct["detail"].ToString(),
                //                           Actpersonid = drAct["personid"].ToString().Substring(0, drAct["personid"].ToString().Length - 1),
                //                           Rptdte = drAct["rpt_dte"].ToString(),
                //                           Actpersonname = drAct["personname"].ToString().Substring(0, drAct["personname"].ToString().Length - 1),
                //                           Listsysid = dr["dtl_id"].ToString(),
                //                           Meetsysid = dr["mt_id"].ToString()
                //                       });
                //********************

                listItems.Add(new MeetListContent()
                {
                    title = dr["title"].ToString(),
                    meetsysid = dr["mt_id"].ToString(),
                    listsysid = dr["dtl_id"].ToString(),
                    content = dr["detail"].ToString(),
                    ActDetails = listActDetail
                });
                
                ret = true;
            }
            if (!ret)
            {
                sErrmsg = "未找到任何記錄";
            }
            return Json(new { total = listItems.Count, rows = listItems, result = ret, errmsg = sErrmsg }, JsonRequestBehavior.AllowGet);
        }
    }
}
