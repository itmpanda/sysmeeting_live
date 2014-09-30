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
    public class ListColumns
    {
        public string field { get; set; }
        public string title { get; set; }
        public int width { get; set; }
        public bool checkbox { get; set; }
        public bool hidden { get; set; }
    }

    public class AccountColumns
    {
        public string field { get; set; }
        public string title { get; set; }
        public int width { get; set; }
        public bool checkbox { get; set; }
        public bool hidden { get; set; }
    }

    public class MeetColumns
    {
        public string field { get; set; }
        public string title { get; set; }
        public int width { get; set; }
        public bool checkbox { get; set; }
        public bool hidden { get; set; }
    }

    public class DGridTitleController : Controller
    {
        //
        // GET: /DGridTitle/

        public ActionResult List()
        {
            List<ListColumns> cols = new List<ListColumns>();
            cols.Add(new ListColumns() { field = "ck", title = "", width = 50, checkbox = true, hidden = false });
            cols.Add(new ListColumns() { field = "ListSysId", title = "ListSysId", width = 1, checkbox = false, hidden = true });
            cols.Add(new ListColumns() { field = "ListId", title = "事項編號", width = 100, checkbox = false, hidden = false });
            cols.Add(new ListColumns() { field = "ListName", title = "事項標題", width = 200, checkbox = false, hidden = false });

            return Json(new {columns = cols});
        }

        public ActionResult Account()
        {
            List<AccountColumns> cols = new List<AccountColumns>();
            cols.Add(new AccountColumns() { field = "ck", title = "", width = 50, checkbox = true, hidden = false });
            cols.Add(new AccountColumns() { field = "UserId", title = "用戶編號", width = 200, checkbox = false, hidden = false });
            cols.Add(new AccountColumns() { field = "FulName", title = "名稱", width = 200, checkbox = false, hidden = false });

            return Json(new { columns = cols });
        }

        public ActionResult Meet()
        {
            List<MeetColumns> cols = new List<MeetColumns>();
            cols.Add(new MeetColumns() { field = "ck", title = "", width = 50, checkbox = true, hidden = false });
            cols.Add(new MeetColumns() { field = "sysid", title = "系統編號", width = 100, checkbox = false, hidden = true });
            cols.Add(new MeetColumns() { field = "id", title = "會議編號", width = 100, checkbox = false, hidden = false });
            cols.Add(new MeetColumns() { field = "date", title = "會議日期", width = 100, checkbox = false, hidden = false });
            cols.Add(new MeetColumns() { field = "time", title = "會議時間", width = 100, checkbox = false, hidden = false });
            cols.Add(new MeetColumns() { field = "addr", title = "會議地點", width = 100, checkbox = false, hidden = false });

            return Json(new { columns = cols });
        }

    }
}
