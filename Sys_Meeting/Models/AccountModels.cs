using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Sys_Meeting.Models
{
    public class AccountModels
    {
        [Required(ErrorMessage = "請輸入 {0}.")]
        [Display(Name = "用戶名")]
        public  string UserId { get; set; }

        [Required(ErrorMessage = "請輸入 {0}.")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string PassWord { get; set; }

        //全稱
        [Display(Name = "全稱")]
        public string FulName { get; set; }

        public int CK { get; set; }
    }

    public class AccountEditModels
    {
        [Remote("ValidateUserId", "Account", HttpMethod = "POST", ErrorMessage = "用戶名不存在，不能修改密碼.")]
        [Required(ErrorMessage = "請輸入 {0}.")]
        [Display(Name = "用戶名")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "請輸入 {0}.")]
        [DataType(DataType.Password)]
        [Display(Name = "舊密碼")]
        public string Password { get; set; }

        [Required(ErrorMessage = "請輸入 {0}.")]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string Newpwd { get; set; }

        [Compare("Newpwd", ErrorMessage = "兩次輸入密碼不一致，請重新輸入.")]
        [Required(ErrorMessage = "請輸入 {0}.")]
        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        public string ConfirmPassword { get; set; }

        //全稱
        [Display(Name = "全稱")]
        public string FulName { get; set; }
    }
}