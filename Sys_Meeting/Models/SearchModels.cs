using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sys_Meeting.Models
{
    public enum SearchType
    {
        Meet=1,
        List
    }

    public enum SearchField:int
    {
        Id=1,
        Title,
        Date,
        Time,
        Addr
    }

    public class SearchModels
    {
        public string Wd { get; set; }//搜索關鍵字
        public int Model { get; set; }//搜索模塊
        public int Field { get; set; }//搜索字段
    }

    public class SearchMeetModels
    {
        [Display(Name = "關鍵字")]
        public string Keywords { get; set; }

        [Display(Name = "會議事項")]
        public string Listfrom { get; set; }

        [Display(Name = "至")]
        public string Listto { get;set; }

        [Display(Name = "會議日期")]
        public string Meetdatefrom { get; set; }

        [Display(Name = "至")]
        public string Meetdateto { get; set; }
    }
}