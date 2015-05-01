using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class StatisticsList
    {
        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public string Type { get; set; }

        public double TimeCount { get; set; }

        public StatisticsList() { }

        internal static StatisticsList DBDataToOStatisticsList(DataRow row, DateTime begintime, DateTime endtime)
        {
            StatisticsList stalist = new StatisticsList();
            stalist.BeginTime = begintime.ToString("yyyy-MM-dd HH:mm");
            stalist.EndTime = endtime.ToString("yyyy-MM-dd HH:mm");
            stalist.Type = row["typename"].ToString();
            stalist.TimeCount = Convert.ToDouble(row["timesum"]);
            return stalist;
        }
    }
}