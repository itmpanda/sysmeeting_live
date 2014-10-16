using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sys_Meeting.Models;

namespace Sys_Meeting.Controllers
{
    public class MeetFinishController : Controller
    {
        //
        // GET: /MeetFinish/

        public ActionResult Index()
        {
            ViewData["title"] = "會議總結維護";
            return View();
        }

        [HttpPost]
        public ActionResult Save(MeetFinishModels meetFinishModels)
        {
            string errmsg = "";
            string ret = "0";

            //判斷是否登陸
            if (Session["userid"] == null)
            {
                return Json(new { result = "0", errmsg = "登陸超時請重新登陸！" });
            }

            string meetsysid = meetFinishModels.Meetsysid;

            StringBuilder sb=new StringBuilder();
            string sql = "";
            string guid = "";
            string[] aActPerson=new string[0];

            SqlConnection cnn=new SqlConnection();
            cnn.ConnectionString = DBCommon.SqlHelper.ConntionString;
            cnn.Open();

            SqlTransaction tran = cnn.BeginTransaction();

            //初始化表格結構
            DataTable dt = new DataTable();
            DataTable dtPersonTable = new DataTable();

            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("sys_id", typeof (string)),
                new DataColumn("mt_id", typeof (string)),
                new DataColumn("list_id", typeof (string)),
                new DataColumn("detail", typeof (string)),
                new DataColumn("rpt_dte", typeof (System.DateTime))
            });

            dtPersonTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("act_id", typeof (string)),
                new DataColumn("wor_num", typeof (string))
            });
            //****************************************************

            try
            {
                List<ListValueModels> listValues = meetFinishModels.Values;
                foreach (var values in listValues)
                {

                    string listsysid = values.Listsysid;

                    //delete tb_list_act
                    sql = "delete from tb_list_act where mt_id=@mt_id and list_id=@list_id";
                    DBCommon.SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql
                        , new SqlParameter("@mt_id", meetsysid)
                        , new SqlParameter("@list_id", listsysid));

                    //delete tb_list_act_person
                    sql = "delete from tb_list_act_person where act_id=@act_id";
                    DBCommon.SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql
                        , new SqlParameter("@act_id", listsysid));

                    if (values.Details != null)
                    {
                        //插入明細數據***
                        List<ListDetailModels> listDetails = values.Details;
                        foreach (var details in listDetails)
                        {                            
                            //insert table values tb_list_act
                            guid = Guid.NewGuid().ToString().ToUpper();
                            DataRow row = dt.NewRow();

                            row["sys_id"] = guid;
                            row["mt_id"] = meetsysid;
                            row["list_id"] = listsysid;
                            row["detail"] = details.Actcontent;
                            row["rpt_dte"] = details.Rptdte;

                            dt.Rows.Add(row);

                            //insert table values tb_list_act_person需要分開負責人
                            Array.Clear(aActPerson,0,aActPerson.Length);
                            aActPerson = details.Actpersonid.Split(',');
                            for (int i = 0; i < aActPerson.Length; i++)
                            {
                                DataRow rowPersonRow = dtPersonTable.NewRow();
                                rowPersonRow["act_id"] = guid;
                                rowPersonRow["wor_num"] = aActPerson[i];

                                dtPersonTable.Rows.Add(rowPersonRow);
                            }
                        }
                        //****************************
                    }
                }

                //更新的數據庫
                DBCommon.SqlHelper.BulkToDb(cnn, tran, dt, "tb_list_act", out errmsg);
                DBCommon.SqlHelper.BulkToDb(cnn, tran, dtPersonTable, "tb_list_act_person", out errmsg);
                tran.Commit();
                //********************

                ret = "1";
            }
            catch (Exception e)
            {
                tran.Rollback();
                errmsg = e.Message;
                //throw;
            }

            return Json(new {result = ret, errmsg = errmsg});
        }

        public ActionResult SearchResult()
        {
            return View();
        }

        //保存會議記錄的行動內容
        public JsonResult SaveActDetail(MeetActDetail meetActDetail)
        {
            bool ret = false;
            string errmsg = "";

            if (Session["userid"] == null)
            {
                return Json(new { result = ret, errmsg = "登陸超時請重新登陸！" });
            }
            string meetsysid = meetActDetail.Meetsysid;
            string listsysid = meetActDetail.Listsysid;

            StringBuilder sb = new StringBuilder();
            string sql = "";
            string guid = "";
            string[] aActPerson = new string[0];

            SqlConnection cnn = new SqlConnection();
            cnn.ConnectionString = DBCommon.SqlHelper.ConntionString;
            cnn.Open();

            SqlTransaction tran = cnn.BeginTransaction();

            //初始化表格結構
            DataTable dt = new DataTable();
            DataTable dtPersonTable = new DataTable();

            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("sys_id", typeof (string)),
                new DataColumn("mt_id", typeof (string)),
                new DataColumn("list_id", typeof (string)),
                new DataColumn("detail", typeof (string)),
                new DataColumn("rpt_dte", typeof (System.DateTime))
            });

            dtPersonTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("act_id", typeof (string)),
                new DataColumn("wor_num", typeof (string))
            });
            //****************************************************

            try
            {
                //delete tb_list_act
                sql = "delete from tb_list_act where mt_id=@mt_id and list_id=@list_id";
                DBCommon.SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql
                    , new SqlParameter("@mt_id", meetsysid)
                    , new SqlParameter("@list_id", listsysid));

                //delete tb_list_act_person
                sql = "delete from tb_list_act_person where act_id=@act_id";
                DBCommon.SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql
                    , new SqlParameter("@act_id", listsysid));

                if (meetActDetail.Values != null)
                {
                    //插入明細數據***
                    List<ListDetailModels> listDetails = meetActDetail.Values;
                    foreach (var details in listDetails)
                    {
                        //insert table values tb_list_act
                        guid = Guid.NewGuid().ToString().ToUpper();
                        DataRow row = dt.NewRow();

                        row["sys_id"] = guid;
                        row["mt_id"] = meetsysid;
                        row["list_id"] = listsysid;
                        row["detail"] = details.Actcontent;
                        row["rpt_dte"] = details.Rptdte;

                        dt.Rows.Add(row);

                        //insert table values tb_list_act_person需要分開負責人
                        Array.Clear(aActPerson, 0, aActPerson.Length);
                        aActPerson = details.Actpersonid.Split(',');
                        for (int i = 0; i < aActPerson.Length; i++)
                        {
                            DataRow rowPersonRow = dtPersonTable.NewRow();
                            rowPersonRow["act_id"] = guid;
                            rowPersonRow["wor_num"] = aActPerson[i];

                            dtPersonTable.Rows.Add(rowPersonRow);
                        }
                    }
                    //****************************
                }
                //}

                //更新的數據庫
                DBCommon.SqlHelper.BulkToDb(cnn, tran, dt, "tb_list_act", out errmsg);
                DBCommon.SqlHelper.BulkToDb(cnn, tran, dtPersonTable, "tb_list_act_person", out errmsg);
                tran.Commit();
                //********************

                ret = true;
            }
            catch (Exception e)
            {
                tran.Rollback();
                errmsg = e.Message;
                //throw;
            }
            return Json(new {result = ret, errmsg = errmsg});
        }
    }
}
