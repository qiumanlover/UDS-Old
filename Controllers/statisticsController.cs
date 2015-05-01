using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UDS.Models;

namespace UDS.Controllers
{
    public class StatisticsController : Controller
    {
        //
        // GET: /statistics/

        public ActionResult QJSta()
        {
            ViewBag.IsBack = 0;
            if (Request["submit"] != null)
            {
                int eid = (Session["user"] as User).Eid;
                DateTime begintime = Convert.ToDateTime(Request["sbegintime"]);
                DateTime endtime = Convert.ToDateTime(Request["sendtime"]);
                DataTable dt = SQLHelper.ProcDataTable("usp_StaQJ", new SqlParameter("@eid", eid), new SqlParameter("@begintime", begintime), new SqlParameter("@endtime", endtime));
                List<StatisticsList> stalist = new List<StatisticsList>();
                foreach (DataRow row in dt.Rows)
                {
                    stalist.Add(StatisticsList.DBDataToOStatisticsList(row, begintime, endtime));
                }
                ViewBag.BeginTime = begintime.ToString("yyyy-MM-dd HH:mm");
                ViewBag.EndTime = endtime.ToString("yyyy-MM-dd HH:mm");
                ViewData.Model = stalist;
                ViewBag.IsBack = 1;
            }
            return PartialView();
        }

        public ActionResult JBSta()
        {
            ViewBag.IsBack = 0;
            if (Request["submit"] != null)
            {
                int eid = (Session["user"] as User).Eid;
                DateTime begintime = Convert.ToDateTime(Request["sbegintime"]);
                DateTime endtime = Convert.ToDateTime(Request["sendtime"]);
                DataTable dt = SQLHelper.ProcDataTable("usp_StaJB", new SqlParameter("@eid", eid), new SqlParameter("@begintime", begintime), new SqlParameter("@endtime", endtime));
                List<StatisticsList> stalist = new List<StatisticsList>();
                foreach (DataRow row in dt.Rows)
                {
                    stalist.Add(StatisticsList.DBDataToOStatisticsList(row, begintime, endtime));
                }
                ViewBag.BeginTime = begintime.ToString("yyyy-MM-dd HH:mm");
                ViewBag.EndTime = endtime.ToString("yyyy-MM-dd HH:mm");
                ViewData.Model = stalist;
                ViewBag.IsBack = 1;
            }
            return PartialView();
        }

    }
}
