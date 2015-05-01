using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class BaseInfo
    {
        public string Formname { get; set; }
        public string Applier { get; set; }
        public string Department { get; set; }
        public string Posotion { get; set; }
        public string Writetime { get; set; }
        public string State { get; set; }
        public string Nextname { get; set; }

        public BaseInfo DBToObject(DataTable dtBaseInfo)
        {
            this.Formname = dtBaseInfo.Rows[0]["formname"].ToString();
            this.Applier = dtBaseInfo.Rows[0]["name"].ToString();
            this.Department = dtBaseInfo.Rows[0]["department"].ToString();
            this.Posotion = dtBaseInfo.Rows[0]["position"].ToString();
            this.Writetime = Convert.ToDateTime(dtBaseInfo.Rows[0]["writetime"]).ToString("yyyy-MM-dd HH:mm:ss");
            this.State = dtBaseInfo.Rows[0]["state"].ToString();
            this.Nextname = dtBaseInfo.Rows[0]["nextname"].ToString();

            return this;
        }
    }
}