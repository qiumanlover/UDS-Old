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
    [LoginActionFilter]
    public class ListController : Controller
    {
        //
        // GET: /List/

        public ActionResult NextSignList()
        {
            int eid = (Session["user"] as User).Eid;
            int pagecount;
            int pageindex = int.Parse(Request["pageindex"]);
            if (pageindex < 1) pageindex = 1;
            List<InfoList> data = GetInfoListFromDB("usp_NextSign", eid, pageindex, 20, out pagecount);
            if (pageindex > pagecount)
            {
                pageindex = pagecount;
                data = GetInfoListFromDB("usp_NextSign", eid, pageindex, 20, out pagecount);
            }
            ViewData.Model = data;
            ViewBag.pageCount = pagecount;
            ViewBag.pageIndex = pageindex;
            return PartialView();
        }

        public ActionResult RecordSignList()
        {
            int eid = (Session["user"] as User).Eid;
            int pagecount;
            int pageindex = int.Parse(Request["pageindex"]);
            if (pageindex < 1) pageindex = 1;
            List<InfoList> data = GetInfoListFromDB("usp_RecordSign", eid, pageindex, 20, out pagecount);
            if (pageindex > pagecount)
            {
                pageindex = pagecount;
                data = GetInfoListFromDB("usp_RecordSign", eid, pageindex, 20, out pagecount);
            }
            ViewData.Model = data;
            ViewBag.pageCount = pagecount;
            ViewBag.pageIndex = pageindex;
            return PartialView();
        }

        public ActionResult OwnApplyList()
        {
            int eid = (Session["user"] as User).Eid;
            int pagecount;
            int pageindex = int.Parse(Request["pageindex"]);
            if (pageindex < 1) pageindex = 1;
            List<InfoList> data = GetInfoListFromDB("usp_OwnApply", eid, pageindex, 20, out pagecount);
            if (pageindex > pagecount)
            {
                pageindex = pagecount;
                data = GetInfoListFromDB("usp_OwnApply", eid, pageindex, 20, out pagecount);
            }
            ViewData.Model = data;
            ViewBag.pageCount = pagecount;
            ViewBag.pageIndex = pageindex;
            return PartialView();
        }

        public ActionResult OwnDraftList()
        {
            int eid = (Session["user"] as User).Eid;
            int pagecount;
            int pageindex = int.Parse(Request["pageindex"]);
            if (pageindex < 1) pageindex = 1;
            List<InfoList> data = GetInfoListFromDB("usp_OwnDraft", eid, pageindex, 20, out pagecount);
            if (pageindex > pagecount)
            {
                pageindex = pagecount;
                data = GetInfoListFromDB("usp_OwnDraft", eid, pageindex, 20, out pagecount);
            }
            ViewData.Model = data;
            ViewBag.pageCount = pagecount;
            ViewBag.pageIndex = pageindex;
            return PartialView();
        }

        public ActionResult WriteList()
        {
            DataTable dt = SQLHelper.ProcDataTable("usp_WriteList", null);
            Dictionary<int, string> flowList = new Dictionary<int, string>();
            foreach (DataRow row in dt.Rows)
            {
                flowList.Add(Convert.ToInt32(row["id"]), row["formname"].ToString());
            }
            ViewBag.flowList = flowList;
            return PartialView();
        }

        private static List<InfoList> GetInfoListFromDB(string procname, int eid, int pageindex, int pagesize, out int pagecount)
        {
            SqlParameter[] parameters = { new SqlParameter("@pageIndex", SqlDbType.Int), new SqlParameter("@pageSize", SqlDbType.Int), new SqlParameter("@pageCount", SqlDbType.Int), new SqlParameter("@eid", SqlDbType.Int) };
            parameters[0].Value = pageindex;
            parameters[1].Value = pagesize;
            parameters[2].Direction = ParameterDirection.Output;
            parameters[3].Value = eid;
            DataTable dt = SQLHelper.ProcDataTable(procname, parameters);
            pagecount = Convert.ToInt32(parameters[2].Value);
            List<InfoList> data = new List<InfoList>();
            DataColumnCollection columns = dt.Columns;
            foreach (DataRow row in dt.Rows)
            {
                InfoList info = new InfoList();
                info = info.DBDataToInfo(row, columns);
                data.Add(info);
            }
            return data;
        }
    }
}
