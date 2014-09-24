using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Sys_Meeting.generalHandler;
using Sys_Meeting.Models;

namespace Sys_Meeting.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
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
            if (ModelState.IsValid)
            {
                string sql = "select wor_num,pwd from tb_user where wor_num=@wor_num";
                string pwd = "";
                SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.ReportCentreConnectionString, CommandType.Text, sql,
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

        public ActionResult Search()
        {
            return View();
        }

        //搜索帳戶
        public ActionResult SearchAccount()
        {
            //return View();
            string sql = "";
            sql = DbCommon.GetPageSql("tb_user", "wor_num,ful_name","wor_num");
            DataSet ds = SqlHelper.ExecuteDataset(DbCommon.GConnectionString, CommandType.Text, sql
                , new SqlParameter("@pagenum", 1)
                , new SqlParameter("@pagesize", DbCommon.GetPageSize));

            string json = DataTableConvertJson.Dataset2Json(ds);
            json = "{\"total\":30,\"rows\":" + json + "}";
            return Content(json);
        }

    }
}
