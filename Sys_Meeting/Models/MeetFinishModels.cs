using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sys_Meeting.Models
{
    //保存會議記錄模型
    public class MeetFinishModels
    {
        public string Meetsysid { get; set; }
        public List<ListValueModels>  Values { get; set; }
    }

    //保存會議記錄明細
    public class MeetActDetail
    {
        public string Meetsysid { get; set; }
        public string Listsysid { get; set; }
        public List<ListDetailModels> Values { get; set; }
    }

    public class ListValueModels
    {
        public string Listsysid { get; set; }
        public List<ListDetailModels>  Details { get; set; }
    }

    public class ListDetailModels
    {
        public string Actpersonid { get; set; }
        public string Actpersonname { get; set; }
        public string Actcontent { get; set; }
        public string Rptdte { get; set; }
        public string Listsysid { get; set; }
        public string Meetsysid { get; set; }
    }
}