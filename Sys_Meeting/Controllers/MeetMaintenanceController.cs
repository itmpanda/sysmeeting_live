using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;
using Sys_Meeting.generalHandler;
using Sys_Meeting.Models;

namespace Sys_Meeting.Controllers
{
    public class MeetMaintenanceController : Controller
    {

        public enum ActionMethods
        {
            Add,
            Edit,
            Delete
        }

        public enum LoadType
        {
            Master,
            Join,
            UnJoin,
            Share,
            MeetList
        }
        //
        // GET: /MeetMaintenance/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetMeet(MeetMaintenanceModels meetMaintenanceModels)
        {
            return Json(new {result = "0"});
        }

        ///<summary>
        /// @20140922
        /// 獲取申請會議記錄
        /// </summary>
        /// <returns>JSON格式數據</returns>
        [HttpGet]
        public ActionResult GetMeet()
        {

            string sysid = Request.RequestContext.RouteData.Values["id"] == null ? "" : Request.RequestContext.RouteData.Values["id"].ToString();
            
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from(");
            sql.Append("select top (@pagenum * @pagesize) ROW_NUMBER() OVER (ORDER BY sys_dte) AS rownum, sys_id,mt_id,");
            sql.Append("title,mt_dte,mt_time,addr from tb_meet WHERE 1=1 and is_del=0 ");
            sql.Append(sysid == "" ? "" : " and sys_id='"+sysid+"'");
            sql.Append(") as tb");
            sql.Append(" where rownum between ( @pagenum - 1 )* @pagesize + 1 AND (@pagenum*@pagesize) order by rownum");

            int page = 1;
            int rows = 10;
            
            DataSet ds = SqlHelper.ExecuteDataset(DbCommon.GConnectionString, CommandType.Text, sql.ToString()
                , new SqlParameter("@pagesize", rows)
                , new SqlParameter("@pagenum", page));
            
            List<MeetMaintenanceModels> listRows=new List<MeetMaintenanceModels>();
            
            List<AccountModels> listMasters = new List<AccountModels>();
            List<AccountModels> listJoins = new List<AccountModels>();
            List<AccountModels> listUnJoins = new List<AccountModels>();
            List<AccountModels> listShares = new List<AccountModels>();
            List<MeetListContent> listItems = new List<MeetListContent>();

            if (sysid != "" && ds.Tables[0].Rows.Count > 0)
            {
                //加載主席
                DataSet dsDetail = GetDataSet(sysid, LoadType.Master);
                foreach (DataRow dr in dsDetail.Tables[0].Rows)
                {
                    listMasters.Add(new AccountModels()
                    {
                        UserId = dr["wor_num"].ToString(),
                        FulName = dr["ful_name"].ToString()
                    });
                }

                //加載出席人員
                dsDetail = GetDataSet(sysid, LoadType.Join);
                foreach (DataRow dr in dsDetail.Tables[0].Rows)
                {
                    listJoins.Add(new AccountModels()
                    {
                        UserId = dr["wor_num"].ToString(),
                        FulName = dr["ful_name"].ToString()
                    });
                }

                //加載缺席人員
                dsDetail = GetDataSet(sysid, LoadType.UnJoin);
                foreach (DataRow dr in dsDetail.Tables[0].Rows)
                {
                    listUnJoins.Add(new AccountModels()
                    {
                        UserId = dr["wor_num"].ToString(),
                        FulName = dr["ful_name"].ToString()
                    });
                }

                //共享人員
                dsDetail = GetDataSet(sysid, LoadType.Share);
                foreach (DataRow dr in dsDetail.Tables[0].Rows)
                {
                    listShares.Add(new AccountModels()
                    {
                        UserId = dr["wor_num"].ToString(),
                        FulName = dr["ful_name"].ToString()
                    });
                }

                //加載會議事項
                dsDetail = GetDataSet(sysid, LoadType.MeetList);
                foreach (DataRow dr in dsDetail.Tables[0].Rows)
                {
                    listItems.Add(new MeetListContent()
                    {
                        content = dr["detail"].ToString(),
                        listsysid = dr["dtl_id"].ToString(),
                        meetsysid = dr["mt_id"].ToString(),
                        title = dr["title"].ToString()
                    });
                }
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                listRows.Add(new MeetMaintenanceModels()
                {
                    sysid = dr["sys_id"].ToString(),
                    id = dr["mt_id"].ToString(),
                    date = dr["mt_dte"].ToString(),
                    time = dr["mt_time"].ToString(),
                    addr = dr["addr"].ToString(),
                    name = dr["title"].ToString(),
                    listitems = listItems,
                    listmasters = listMasters,
                    listjoins = listJoins,
                    listunjoins = listUnJoins,
                    listsharelists = listShares
                });
            }

            return Json(new { total = 30, rows = listRows }, JsonRequestBehavior.AllowGet);

        }

        public DataSet GetDataSet(string id ,LoadType lt)
        {
            string sql = "";
            switch (lt)
            {
                case LoadType.Master:
                    sql = "select jlst.mt_id,jlst.wor_num,jlst.type,usr.ful_name from tb_meet_joinlist jlst left join tb_user usr on jlst.wor_num=usr.wor_num where jlst.mt_id=@mt_id and jlst.type=0";
                    break;
                case LoadType.Join:
                    sql = "select jlst.mt_id,jlst.wor_num,jlst.type,usr.ful_name from tb_meet_joinlist jlst left join tb_user usr on jlst.wor_num=usr.wor_num where jlst.mt_id=@mt_id and jlst.type=1";
                    break;
                case LoadType.UnJoin:
                    sql = "select jlst.mt_id,jlst.wor_num,jlst.type,usr.ful_name from tb_meet_joinlist jlst left join tb_user usr on jlst.wor_num=usr.wor_num where jlst.mt_id=@mt_id and jlst.type=2";
                    break;  
                case LoadType.Share:
                    sql = "select jlst.mt_id,jlst.wor_num,jlst.type,usr.ful_name from tb_meet_joinlist jlst left join tb_user usr on jlst.wor_num=usr.wor_num where jlst.mt_id=@mt_id and jlst.type=3";
                    break;
                case LoadType.MeetList:
                    sql = "select dtl.mt_id,dtl.dtl_id,dtl.detail,lst.title from tb_meet_detail dtl left join tb_list lst on dtl.dtl_id=lst.sys_id where mt_id=@mt_id and is_del=0";
                    break;
                default:
                    break;
            }
            
            DataSet ds = SqlHelper.ExecuteDataset(DbCommon.GConnectionString, CommandType.Text, sql
                , new SqlParameter("@mt_id", id));
            return ds;
        }

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        private string GetIP()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }


        public bool ValidInput()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Action(MeetMaintenanceModels meetMaintenanceModels)
        {
            //空缺
            //安全判斷是否有空值傳入
            //暫時未做驗證
            if (!ValidInput())
            {
                return Json(new {result = "0"});
            }

            if (Session["userid"] == null)
            {
                return Json(new {result = "0", errmsg = "登陸超時請重新登陸！"});
            }
            StringBuilder sbMeetSql = new StringBuilder();

            //-->數據插入操作
            string ip = GetIP();
            int isshare = 0;

            isshare = string.IsNullOrEmpty(meetMaintenanceModels.sharelist) || meetMaintenanceModels.sharelist=="" ? 0 : 1;

            string sMsg = "", retSuc="0";
            bool bResult= true;

            SqlConnection conn=new SqlConnection(DbCommon.GConnectionString);
            conn.Open();

            SqlTransaction tran = conn.BeginTransaction();

            try
            {
                //-->插入tb_meet數據
                sbMeetSql.Append(
                    "insert into tb_meet(sys_id,mt_id,title,mt_dte,mt_time,addr,create_by,last_ip,is_share)");
                sbMeetSql.Append("values(@sys_id,@mt_id,@title,@mt_dte,@mt_time,@addr,@create_by,@last_ip,@is_share)");

                int r1 = SqlHelper.ExecuteNonQuery(DbCommon.GConnectionString, CommandType.Text, sbMeetSql.ToString()
                    , new SqlParameter("@sys_id", meetMaintenanceModels.sysid)
                    , new SqlParameter("@mt_id", meetMaintenanceModels.id)
                    , new SqlParameter("@title", meetMaintenanceModels.name)
                    , new SqlParameter("@mt_dte", string.Format(meetMaintenanceModels.date, "yyyy-MM-dd"))
                    , new SqlParameter("@mt_time", meetMaintenanceModels.time)
                    , new SqlParameter("@addr", meetMaintenanceModels.addr)
                    , new SqlParameter("@create_by", Session["userid"].ToString())
                    , new SqlParameter("@last_ip", ip)
                    , new SqlParameter("@is_share", isshare));
                //--<

                //以下插入明細數據
                bResult = AddDetail(meetMaintenanceModels, conn, tran, out sMsg);
                //數據插入操作--<
                tran.Commit();
                retSuc = bResult == true ? "1" : "0";
                //retSuc = "1";
            }
            catch (Exception e)
            {
                tran.Rollback();
                retSuc = "0";
                sMsg = e.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return Json(new {result = retSuc,errmsg=sMsg});
        }

        //修改
        //返回JSON格式的數據
        [HttpPost]
        public ActionResult Edit(MeetMaintenanceModels meetMaintenanceModels)
        {
            string sMsg = "";
            string retval = "";
            bool bretsuc = false;

            if (Session["userid"] == null)
            {
                return Json(new { result = 0, errmsg = "登陸超時請重新登陸！" });
            }

            string sysid = meetMaintenanceModels.sysid;// Request.RequestContext.RouteData.Values["id"].ToString();
            string sql = "select * from tb_meet where sys_id=@sys_id";
            DataSet ds = SqlHelper.ExecuteDataset(DbCommon.GConnectionString, CommandType.Text, sql
                , new SqlParameter("@sys_id", meetMaintenanceModels.sysid));

            if (ds.Tables[0].Rows.Count > 0)
            {
                SqlConnection conn=new SqlConnection();
                conn.ConnectionString = DbCommon.GConnectionString;
                conn.Open();

                SqlTransaction sqlTransaction = conn.BeginTransaction();
                //刪除明細
                try
                {
                    //刪除所有事項列表
                    sql = "delete from tb_meet_detail where mt_id=@mt_id";
                    SqlHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql
                        , new SqlParameter("@mt_id", meetMaintenanceModels.sysid));

                    //刪除會議參與人員
                    sql = "delete from tb_meet_joinlist where mt_id=@mt_id and type in(0,1,2,3)";
                    SqlHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql
                        , new SqlParameter("@mt_id", meetMaintenanceModels.sysid));

                    //更新tb_meet數據
                    sql =
                        "update tb_meet set mt_dte=@mt_dte,mt_time=@mt_time,addr=@addr,title=@title,modi_by=@modi_by,modi_dte=getdate(),last_ip=@last_ip where sys_id=@sys_id and is_del=0";
                    SqlHelper.ExecuteNonQuery(sqlTransaction, CommandType.Text, sql
                        , new SqlParameter("@sys_id", meetMaintenanceModels.sysid)
                        , new SqlParameter("@mt_dte", meetMaintenanceModels.date)
                        , new SqlParameter("@mt_time", meetMaintenanceModels.time)
                        , new SqlParameter("@addr", meetMaintenanceModels.addr)
                        , new SqlParameter("@title", meetMaintenanceModels.name)
                        , new SqlParameter("@modi_by", Session["userid"].ToString())
                        , new SqlParameter("@last_ip", GetIP()));

                    //插入明細
                    bretsuc = AddDetail(meetMaintenanceModels, conn, sqlTransaction, out sMsg);

                    sqlTransaction.Commit();
                    sMsg = null;
                    retval = "1";
                }
                catch (Exception e)
                {
                    sqlTransaction.Rollback();
                    retval = "0";
                    sMsg = e.Message;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            else
            {
                sMsg = "記錄不存在或已被刪除!";
                retval = "0";
            }

            return Json(new { result = retval, errmsg = sMsg });
        }

        //刪除
        public ActionResult Delete()
        {
            string sMsg = "";
            int retSuc = 0;

            if (Session["userid"] == null)
            {
                return Json(new {result = 0, errmsg = "登陸超時請重新登陸！"},JsonRequestBehavior.AllowGet);
            }

            string sysid = Request.RequestContext.RouteData.Values["id"].ToString();
            string sql = "update tb_meet set is_del=1,del_by=@del_by,del_dte=getdate(),last_ip=@last_ip where sys_id=@sys_id";
            
            try
            {
                retSuc = SqlHelper.ExecuteNonQuery(DbCommon.GConnectionString, CommandType.Text, sql
                                , new SqlParameter("@sys_id", sysid)
                                ,new SqlParameter("@del_by",Session["userid"])
                                ,new SqlParameter("@last_ip",GetIP()));
            }
            catch (Exception e)
            {
                sMsg = e.Message;
            }

            return Json(new { result = retSuc, errmsg = sMsg },JsonRequestBehavior.AllowGet);
        }

        public bool AddDetail(MeetMaintenanceModels meetMaintenanceModels,SqlConnection sqlConnection,SqlTransaction sqlTransaction, out string errMsg)
        {
            bool retval = false;
            errMsg = null;
            try
            {
                //-->插入tb_meet_joinlist參會人員的數據
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("mt_id", typeof (string)),
                    new DataColumn("wor_num", typeof (string)),
                    new DataColumn("type", typeof (Int16))
                });

                string[] aJoinLists;

                //主席
                aJoinLists = meetMaintenanceModels.master.Split(',');
                for (int i = 0; i < aJoinLists.Length; i++)
                {
                    DataRow r = dt.NewRow();
                    r["mt_id"] = meetMaintenanceModels.sysid;
                    r["wor_num"] = aJoinLists[i];
                    r["type"] = 0;
                    dt.Rows.Add(r);
                }

                //出席
                Array.Clear(aJoinLists, 0, aJoinLists.Length);
                aJoinLists = meetMaintenanceModels.joins.Split(',');
                for (int i = 0; i < aJoinLists.Length; i++)
                {
                    DataRow r = dt.NewRow();
                    r["mt_id"] = meetMaintenanceModels.sysid;
                    r["wor_num"] = aJoinLists[i];
                    r["type"] = 1;
                    dt.Rows.Add(r);
                }

                //未出席
                Array.Clear(aJoinLists, 0, aJoinLists.Length);
                aJoinLists = meetMaintenanceModels.unjoins.Split(',');
                for (int i = 0; i < aJoinLists.Length; i++)
                {
                    DataRow r = dt.NewRow();
                    r["mt_id"] = meetMaintenanceModels.sysid;
                    r["wor_num"] = aJoinLists[i];
                    r["type"] = 2;
                    dt.Rows.Add(r);
                }

                //-->共享列表
                Array.Clear(aJoinLists, 0, aJoinLists.Length);
                aJoinLists = meetMaintenanceModels.sharelist.Split(',');
                for (int i = 0; i < aJoinLists.Length; i++)
                {
                    DataRow r = dt.NewRow();
                    r["mt_id"] = meetMaintenanceModels.sysid;
                    r["wor_num"] = aJoinLists[i];
                    r["type"] = 3;
                    dt.Rows.Add(r);
                }

                retval=DbCommon.BulkToDB(sqlConnection, sqlTransaction, dt, "tb_meet_joinlist", out errMsg);
                
                //-->插入tb_meet_detail的數據
                dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("mt_id", typeof (string)),
                    new DataColumn("dtl_id", typeof (string)),
                    new DataColumn("detail", typeof (string))
                });

                List<MeetListContent> lists = meetMaintenanceModels.listitems;
                foreach (MeetListContent l in lists)
                {
                    DataRow r = dt.NewRow();
                    r["mt_id"] = meetMaintenanceModels.sysid;
                    r["dtl_id"] = l.listsysid;
                    r["detail"] = l.content;
                    dt.Rows.Add(r);
                }

                retval=DbCommon.BulkToDB(sqlConnection,sqlTransaction ,dt, "tb_meet_detail",out errMsg);
                
                //--<

                //--<

                return true;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                throw e;
                //return false;
            }
            return retval;
        }

        public ActionResult GetGUID()
        {
            //前臺和后臺的的GUID匹配，一致就保存
            //暫時不用
            
            string guid = Guid.NewGuid().ToString().ToUpper();
            //if (MtList == null)
            //{
            //    MtList=new List<string>();
            //}
            //MtList.Add(guid);

            //string[] SysId =new string[]{};
            //ViewData["sysid"] = guid;
            
            return Json(new {result=1,sysid=guid},JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetGUIDExists(string guid)
        //{
        //    return Content(MtList.Contains(guid).ToString());
        //}

    }
}
