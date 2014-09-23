using System;
using System.Collections.Generic;
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
    }
}