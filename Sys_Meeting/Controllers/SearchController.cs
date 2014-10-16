using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;
using Sys_Meeting.Models;

namespace Sys_Meeting.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        //public List<SearchField> StatusToList()
        //{
        //    List<SearchField> list = new List<SearchField>();
        //    foreach (string statusName in Enum.GetNames(typeof(SearchField)))
        //    {
        //        list.Add((SearchField)statusName);
        //    }
        //    return list;
        //}
        [HttpPost]
        public ActionResult Result(SearchModels searchModels)
        {

            if (Session["userid"] == null)
            {
                return Json(new {total = 0, rows = "", result = false, errmsg = "登陸超時請重新登陸！",module=searchModels.Model});
            }

            string meetsysid = "";
            string mtid = "",title="",date="",time="",addr="";

            switch ((int)searchModels.Field)
            {
                case (int)SearchField.Id:
                    mtid = searchModels.Wd;
                    break;
                case(int)SearchField.Title:
                    title = searchModels.Wd;
                    break;
                case(int)SearchField.Date:
                    date = searchModels.Wd;
                    break;
                case (int)SearchField.Time:
                    time = searchModels.Wd;
                    break;
                case (int)SearchField.Addr:
                    addr = searchModels.Wd;
                    break;
                default:
                    return Json(new { result = "0", errmsg = "非法請求" });
            }

            
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from(");
            sql.Append("select top (@pagenum * @pagesize) ROW_NUMBER() OVER (ORDER BY sys_dte) AS rownum, sys_id,mt_id,");
            sql.Append("title,mt_dte,mt_time,addr from tb_meet WHERE 1=1 and is_del=0 ");
            sql.Append(meetsysid == "" ? "" : " and sys_id='" + meetsysid + "'");
            sql.Append(mtid == "" ? "" : " and mt_id like '%" + mtid + "%'");
            sql.Append(title == "" ? "" : " and title like '%" + title + "%'");
            sql.Append(date == "" ? "" : " and mt_dte like '%" + date + "%'");
            sql.Append(time == "" ? "" : " and mt_time like '%" + time + "%'");
            sql.Append(addr == "" ? "" : " and addr like '%" + addr + "%'");

            sql.Append(") as tb");
            sql.Append(" where rownum between ( @pagenum - 1 )* @pagesize + 1 AND (@pagenum*@pagesize) order by rownum");

            int page = 1;
            int rows = 10;

            DataSet ds = DBCommon.SqlHelper.ExecuteDataset(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql.ToString()
                , new SqlParameter("@pagesize", rows)
                , new SqlParameter("@pagenum", page));

            List<MeetMaintenanceModels> listRows = new List<MeetMaintenanceModels>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                listRows.Add(new MeetMaintenanceModels()
                {
                    sysid = dr["sys_id"].ToString(),
                    id = dr["mt_id"].ToString(),
                    date = dr["mt_dte"].ToString(),
                    time = dr["mt_time"].ToString(),
                    addr = dr["addr"].ToString(),
                    name = dr["title"].ToString()//,
                    //listitems = listItems,
                    //listmasters = listMasters,
                    //listjoins = listJoins,
                    //listunjoins = listUnJoins,
                    //listsharelists = listShares
                });
            }

            return Json(new { total = ds.Tables[0].Rows.Count, rows = listRows, result = true, errmsg = "", module = searchModels.Model });
            //return Json(new {result = "1", errmsg = ""});
        }

    }
}
