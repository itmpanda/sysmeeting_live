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
    }

    public class MeetMaintenanceModels
    {
        //會議編號
        [Display(Name = "會議編號")]
        [Required]
        public string MtId { get; set; }

        //會議日期
        [Display(Name = "會議日期")]
        [Required]
        public string MtDte { get; set; }

        //會議時間
        [Display(Name = "會議時間")]
        [Required]
        public string MtTime { get; set; }

        //會議地點
        [Display(Name = "會議地點")]
        [Required]
        public string MtAddr { get; set; }

        //主席
        [Display(Name = "主席")]
        [Required]
        public string MtMasters { get; set; }

        //出席會議人員
        [Display(Name = "出席")]
        [Required]
        public string MtJoins { get; set; }

        //缺席人員
        [Display(Name = "缺席人員")]
        public string MtUnJoins { get; set; }

        //會議名稱
        [Display(Name = "會議名稱")]
        [Required]
        public string MtTitle { get; set; }
    }

}