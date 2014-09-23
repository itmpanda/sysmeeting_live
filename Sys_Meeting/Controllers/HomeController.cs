using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Script.Serialization;
using System.Text;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Sys_Meeting.generalHandler;

namespace Sys_Meeting.Controllers
{

    /// <summary>
    /// http://blog.csdn.net/lishehe/article/details/16010271
    /// </summary>
    public class DataTableConvertJson
    {

        #region dataTable转换成Json格式
        /// <summary>    
        /// dataTable转换成Json格式    
        /// </summary>    
        /// <param name="dt"></param>    
        /// <returns></returns>    
        public static string DataTable2Json(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            //jsonBuilder.Append("{\"");
            //jsonBuilder.Append(dt.TableName);
            //jsonBuilder.Append("\":[");
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            //jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        #endregion dataTable转换成Json格式
        #region DataSet转换成Json格式
        /// <summary>    
        /// DataSet转换成Json格式    
        /// </summary>    
        /// <param name="ds">DataSet</param>   
        /// <returns></returns>    
        public static string Dataset2Json(DataSet ds)
        {
            StringBuilder json = new StringBuilder();

            foreach (DataTable dt in ds.Tables)
            {
                //json.Append("{\"");
                //json.Append(dt.TableName);
                //json.Append("\":");
                json.Append(DataTable2Json(dt));
                //json.Append("}");
            } return json.ToString();
        }
        #endregion

        /// <summary>  
        /// Msdn  
        /// </summary>  
        /// <param name="jsonName"></param>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTableToJson(string jsonName, DataTable dt)
        {
            StringBuilder Json = new StringBuilder();
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString() + "\"");
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
    }

    /// <summary>
    /// http://www.cnblogs.com/kissdodog/archive/2012/12/31/2841201.html
    /// 写成DataTable的扩展方法是这样
    /// </summary>
    public static class JsonTableHelper
    {
        /// <summary> 
        /// 返回对象序列化 
        /// </summary> 
        /// <param name="obj">源对象</param> 
        /// <returns>json数据</returns> 
        public static string ToJson(object obj)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            return serialize.Serialize(obj);
        }

        /// <summary> 
        /// 控制深度 
        /// </summary> 
        /// <param name="obj">源对象</param> 
        /// <param name="recursionDepth">深度</param> 
        /// <returns>json数据</returns> 
        public static string ToJson(object obj, int recursionDepth)
        {
            
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            serialize.RecursionLimit = recursionDepth;
            return serialize.Serialize(obj);
        }

        /// <summary> 
        /// DataTable转为json 
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns>json数据</returns> 
        public static string ToJson(this DataTable dt)
        {
            List<object> dic = new List<object>();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();

                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                dic.Add(result);
            }
            return ToJson(dic);
        }
    }

    public class HomeController : Controller
    {
        //
        // GET: /Home/





        public ActionResult Index()
        {
            ViewData["key"] = "this is viewdata value";
            return View();
        }
        public ActionResult Admin()
        {

            return View("admin");
        }

        public ActionResult menu()
        {
            return View("menu");
        }

        public ActionResult lanmu_Menu()
        {
            return View("lanmu_Menu");
        }

        public ActionResult ajaxHandler()
        {
            return View("ajaxHandler");
        }

        public ActionResult grid(string id, string name)
        {
            //Response.Write("id:" + id + ",name:" + name);
            return View("grid");
        }

        public ActionResult TimeLine()
        {
            return View("TimeLine");
        }


        public ActionResult GetJson(int page,int rows)
        {
            //string connString=SqlHelper.ReportCentreConnectionString;
            ////int page=1,int rows=1
            //string sql = "";
            //sql="select * from(";
            //sql+="select top (@pagenum * @pagesize) ROW_NUMBER() OVER (ORDER BY wor_id) AS rownum, wor_id id,ful_name name,age FROM empmas WHERE wor_num like 'ab%'";
            //sql+=") as tb";
            //sql += " where rownum between ( @pagenum - 1 )* @pagesize + 1 AND (@pagenum*@pagesize) order by rownum";

            ////string sql = "select top 1000 wor_id as id,FUL_NAME as [name],age from empmas";
            ////int page = 1;
            ////int rows = 10;

            //DataSet ds = SqlHelper.ExecuteDataset(connString, CommandType.Text, sql,new SqlParameter("@pagesize",rows),new SqlParameter("@pagenum",page));
            ////SqlDataReader dr = UGSC.ReportCentre.Services.SqlHelper.ExecuteReader(connString, CommandType.Text, "select top 1 * from empmas");
            //int dscount = ds.Tables[0].Rows.Count;
            ////string json = JsonConvert.SerializeObject(ds, Formatting.None);

            //string json = DataTableConvertJson.Dataset2Json(ds);

            //DataTable dt = new DataTable();
            //dt.Columns.Add("id");
            //dt.Columns.Add("name");
            //dt.Columns.Add("age");

            //dt.Rows.Add("1", "zs", 20);
            //dt.Rows.Add("2", "ls", 21);
            //dt.Rows.Add("3", "ws", 22);
            //dt.Rows.Add("4", "fs", 23);
            string json = "";
            //json = dt.ToJson();// Json(dt).ToString();
            json = "{\"total\":30,\"rows\":" + json + "}";
            return Content(json);
        }

        ///<summary>
        ///方法1    这个方法其实就是DataTable拼接成字符串而已，没什么出奇的
        ///</summary>
        ///<param name="dt"></param>
        ///<returns></returns>
        public static string DataTableToJson1(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return "";
            }

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }
    }


}
