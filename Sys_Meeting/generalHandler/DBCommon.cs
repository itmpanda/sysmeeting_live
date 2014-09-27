using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Sys_Meeting.generalHandler
{
    public class DbCommon
    {
        public static string GConnectionString
        {
            get
            {
                try
                {
                    return SqlHelper.ReportCentreConnectionString;
                }
                catch
                {
                    return "";
                }
            }
        }

        public static int GetPageSize
        {
            get { return 10; }
        }

        /// <summary>
        /// 分頁標準SQL只需要傳入2個參數，得到SQL后，需要自己再把這二個參數加進去：@pagenum，@pagesize
        /// </summary>
        /// <param name="tbname">表名稱</param>
        /// <param name="fields">字段名，前后不包含逗號</param>
        /// <returns>字符串</returns>
        public static string GetPageSql(string tbname,string fields,string orderbyfield="sys_id")
        {
        //    get { return ""; }
            string sql = "";
            sql = "select * from(";
            sql += "select top (@pagenum * @pagesize) ROW_NUMBER() OVER (ORDER BY " + orderbyfield + ") AS rownum, " + fields + " from " + tbname + " WHERE 1=1 and is_del=0 ";
            sql += ") as tb";
            sql += " where rownum between ( @pagenum - 1 )* @pagesize + 1 AND (@pagenum*@pagesize) order by rownum";
            return sql;
        }

        public static bool BulkToDB(SqlConnection sqlConnection, SqlTransaction sqlTransaction, DataTable dt, string tb,
            out string sMsg)
        {
            bool retval = true;
            sMsg = null;
            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, sqlTransaction);
            bulkCopy.DestinationTableName = tb;
            bulkCopy.BatchSize = dt.Rows.Count;

            try
            {
                if (dt != null && dt.Rows.Count != 0)
                    bulkCopy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                retval = false;
                sMsg = ex.Message;
                throw ex;
            }
            finally
            {
                if (bulkCopy != null)
                    bulkCopy.Close();
            }
            return retval;
        }
    }
}