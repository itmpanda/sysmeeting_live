using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sys_Meeting.Models
{
    public class MeetModels
    {
        public int Id { get; set; }

        [Required()]
        [Display(Name = "this is name")]
        public string Name { get; set; }

        [Required()]
        [StringLength(10,ErrorMessage = "max 10")]
        [Display(Name = "地址")]
        public string Addr { get; set; }

        [DataType(DataType.Currency)]
        public string IdCard { get; set; }
    }

    public class MeetListModels
    {
        [Required(ErrorMessage = "請輸入事項編號!")]
        [StringLength(20,ErrorMessage = "長度最大為20個字符")]
        [Display(Name = "事項編號")]
        public string ListId { get; set; }

        [Required(ErrorMessage = "請輸入事項標題!")]
        [Display(Name = "事項標題")]
        public  string ListName { get; set; }

        //新增修改删除
        public string ListAction { get; set; }

        public string ListSysId { get; set; }

        public int CK { get; set; }
    }

    [Serializable]
    public class MeetMaintenanceModels
    {
        //系統ID
        [Required]
        public string sysid { get; set; }

        //會議編號
        [Display(Name = "會議編號")]
        [Required]
        public string id { get; set; }

        //會議日期
        [Display(Name = "會議日期")]
        [Required]
        public string date { get; set; }

        //會議時間
        [Display(Name = "會議時間")]
        [Required]
        public string time { get; set; }

        //會議地點
        [Display(Name = "會議地點")]
        [Required]
        public string addr { get; set; }

        //主席
        [Display(Name = "主席")]
        [Required]
        public string master { get; set; }

        //出席會議人員
        [Display(Name = "出席")]
        [Required]
        public string joins { get; set; }

        //缺席人員
        [Display(Name = "缺席人員")]
        public string unjoins { get; set; }

        //會議名稱
        [Display(Name = "會議名稱")]
        [Required]
        public string name { get; set; }

        //共享指定人員
        public string sharelist { get; set; }
        //public List<string> GuidList;

        [Required]
        public string action { get; set; }

        public List<MeetListContent> listitems { get; set; }

        public List<AccountModels> listmasters { get; set; }
        public List<AccountModels> listjoins { get; set; }
        public List<AccountModels> listunjoins { get; set; }
        public List<AccountModels> listsharelists { get; set; }

}

    [Serializable]
    public class MeetListContent
    {
        public string content { get; set; }
        public string listsysid { get; set; }
        public string meetsysid { get; set; }
        public string title { get; set; }
        public List<ListDetailModels>  ActDetails { get; set; }
    }

}