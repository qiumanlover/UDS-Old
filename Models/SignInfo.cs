using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class SignInfo
    {
        public string SignName { get; set; }
        public string SignTime { get; set; }
        public string Result { get; set; }
        public string Reason { get; set; }

        public static List<SignInfo> DBToObject(System.Data.DataTable dt)
        {
            List<SignInfo> signInfoList = new List<SignInfo>();
            foreach (System.Data.DataRow row in dt.Rows)
            {
                SignInfo signinfo = new SignInfo();
                signinfo.SignName = row["name"].ToString();
                signinfo.SignTime = Convert.ToDateTime(row["signtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                signinfo.Result = row["result"].ToString();
                signinfo.Reason = row["reason"].ToString();
                signInfoList.Add(signinfo);
            }
            return signInfoList;
        }
    }
}