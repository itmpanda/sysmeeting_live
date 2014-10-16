using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Sys_Meeting.Models;
using DBCommon;

namespace Sys_Meeting.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        public ActionResult Index()
        {
            return Content("");
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AccountModels accountModels ,string returnUrl)
        {
            DBCommon.SqlHelper.ConntionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;

            if (ModelState.IsValid)
            {
                string sql = "select wor_num,pwd from tb_user where wor_num=@wor_num";
                string pwd = "";
                SqlDataReader dr = DBCommon.SqlHelper.ExecuteReader(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql,
                    new SqlParameter("@wor_num", accountModels.UserId));
                
                while (dr.Read())
                {
                    pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(accountModels.PassWord, "SHA1");
                    if (dr["pwd"].ToString() == pwd)
                    {

                        string userData = FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "SHA1");// Request.UserHostAddress.ToString();
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, accountModels.UserId, DateTime.Now, DateTime.Now.AddMinutes(30), false, userData);

                        string hashTicket = FormsAuthentication.Encrypt(ticket);
                        HttpCookie hc = new HttpCookie(FormsAuthentication.FormsCookieName, hashTicket);
                        Response.Cookies.Add(hc);

                        Session["userid"] = accountModels.UserId;
                        //if(returnUrl.Equals("") || (returnUrl.Equals("/")) )
                        if (string.IsNullOrEmpty(returnUrl)||(returnUrl.Equals("/")))
                        {
                            return RedirectToAction("admin", "Home");
                        }
                        else
                        {
                            return Redirect(returnUrl);
                        }
                    }
                }
                ModelState.AddModelError("","登陸失敗用戶名或密碼不正確！");
                return View(accountModels);
            }
            else
            {
                ModelState.AddModelError("", "請輸入用戶名或密碼！");
                return View(accountModels);
            }
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string id)
        {
            string sql = "";
            sql = DBCommon.SqlHelper.GetPageSql("tb_user", "wor_num ,ful_name", "wor_num");
            DataSet ds = DBCommon.SqlHelper.ExecuteDataset(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql
                , new SqlParameter("@pagenum", 1)
                , new SqlParameter("@pagesize", SqlHelper.GetPageSize));

            List<AccountModels> listRows = new List<AccountModels>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                listRows.Add(new AccountModels()
                {
                    CK = 0,
                    UserId = dr["wor_num"].ToString(),
                    FulName = dr["ful_name"].ToString()
                });
            }

            return Json(new { total = 30, rows = listRows });
            //return View();
        }

        //搜索帳戶
        public ActionResult SearchAccount()
        {
            //return View();
            string sql = "";
            sql = SqlHelper.GetPageSql("tb_user", "wor_num,ful_name", "wor_num");
            DataSet ds = SqlHelper.ExecuteDataset(SqlHelper.ConntionString, CommandType.Text, sql
                , new SqlParameter("@pagenum", 1)
                , new SqlParameter("@pagesize", SqlHelper.GetPageSize));

            string json = DataTableConvertJson.Dataset2Json(ds);
            json = "{\"total\":30,\"rows\":" + json + "}";
            return Content(json);
        }

        [HttpGet]
        public ActionResult Chgpwd()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewData["userid"] = Session["userid"].ToString();
            return View();
        }

        [HttpPost]
        public JsonResult Chgpwd(AccountEditModels accountModels)
        {
            if (Session["userid"] == null)
            {
                return Json(new {result = "0", errmsg = "登陸超時請重新登陸！"});
            }
            bool ret = false;
            string errmsg = "";
            if (ModelState.IsValid)
            {

                if (ValidOldpwd(accountModels))
                {
                    ret = UpdatePwd(accountModels);
                }                
            }

            return Json(new {result = ret, errmsg = ret ? "修改成功" : "修改密碼失敗"});
        }

        [HttpPost]
        public JsonResult ValidateUserId(string userId)
        {
            string sql = "select * from tb_user where wor_num=@wor_num";
            SqlDataReader dr = DBCommon.SqlHelper.ExecuteReader(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql
                , new SqlParameter("@wor_num", userId));

            bool isValid = false;

            while (dr.Read())
            {
                isValid = dr["wor_num"].ToString() == userId;
            }

            return Json(isValid);
        }

        public bool ValidOldpwd(AccountEditModels accountEditModels)
        {
            string sql = "select * from tb_user where wor_num=@wor_num";
            SqlDataReader dr = DBCommon.SqlHelper.ExecuteReader(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql
                , new SqlParameter("@wor_num", accountEditModels.UserId));
            bool isValid = false;
            while (dr.Read())
            {
                string oldPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(accountEditModels.Password,"SHA1");
                isValid = oldPwd == dr["pwd"].ToString();
            }
            return isValid;
        }

        public bool UpdatePwd(AccountEditModels accountEditModels)
        {
            string pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(accountEditModels.Newpwd, "SHA1");
            string sql = "update tb_user set pwd=@pwd where wor_num=@wor_num";
            bool ret = DBCommon.SqlHelper.ExecuteNonQuery(DBCommon.SqlHelper.ConntionString, CommandType.Text, sql
                , new SqlParameter("@wor_num", accountEditModels.UserId)
                , new SqlParameter("pwd", pwd))>=1;
            return ret;
        }
    }
}
