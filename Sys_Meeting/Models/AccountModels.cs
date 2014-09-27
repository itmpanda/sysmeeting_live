using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sys_Meeting.Models
{
    public class AccountModels
    {
        [Required]
        [Display(Name = "用戶名")]
        public  string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string PassWord { get; set; }

        //全稱
        public string FulName { get; set; }

        public int CK { get; set; }
    }
}