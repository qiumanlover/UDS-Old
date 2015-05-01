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
    public class DetailController : Controller
    {
        //
        // GET: /Detail/

        public ActionResult DetailContainer()
        {
            int id = Convert.ToInt32(Request["id"]);
            int show = int.Parse(Request["show"]);
            int canSign = Convert.ToInt32(Request["canSign"]);
            int backid = Convert.ToInt32(Request["backid"]);
            DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", id));
            int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
            string pagename = dtPreMain.Rows[0]["pagename"].ToString();
            ViewBag.pageName = pagename;
            ViewBag.innerId = innerid;
            ViewBag.show = show;
            DataTable dtBaseInfo = SQLHelper.ProcDataTable("usp_BaseInfo", new SqlParameter("@id", id));
            BaseInfo baseinfo = new BaseInfo();
            ViewBag.BaseInfo = baseinfo.DBToObject(dtBaseInfo);
            DataTable dtSignInfo = SQLHelper.ProcDataTable("usp_ExamInfo", new SqlParameter("@id", id));
            ViewBag.SignInfoList = SignInfo.DBToObject(dtSignInfo);
            ViewBag.Id = id;
            ViewBag.Old = Request["isOld"];
            ViewBag.canSign = canSign;
            switch (backid)
            {
                case 1:
                    ViewBag.ActionName = "NextSignList";
                    ViewBag.ControllerName = "List";
                    break;
                case 2:
                    ViewBag.ActionName = "OwnApplyList";
                    ViewBag.ControllerName = "List";
                    break;
                case 3:
                    ViewBag.ActionName = "RecordSignList";
                    ViewBag.ControllerName = "List";
                    break;
                case 4:
                    ViewBag.ActionName = "AgentSign";
                    ViewBag.ControllerName = "Agent";
                    break;
                default:
                    ViewBag.ActionName = "OwnApplyList";
                    ViewBag.ControllerName = "List";
                    break;
            }
            ViewBag.BackId = backid;
            return PartialView();
        }

        public ActionResult WriteContainer(int flowid)
        {
            DataTable dt = SQLHelper.ProcDataTable("usp_WriteDetail", new SqlParameter("@id", flowid));
            string pagename = dt.Rows[0]["pagename"].ToString();
            ViewBag.pageName = pagename;
            ViewBag.Id = flowid;
            return PartialView();
        }

        public ActionResult DraftContainer(int show, int isOld, int formflowid)
        {
            DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
            int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
            string pagename = dtPreMain.Rows[0]["pagename"].ToString();
            ViewBag.PageName = pagename;
            ViewBag.InnerId = innerid;
            ViewBag.Show = show;
            ViewBag.isOld = isOld;
            ViewBag.Id = formflowid;
            return PartialView();
        }

        public ActionResult SignDeal()
        {
            int formflowid = Convert.ToInt32(Request["id"]);
            int backid = Convert.ToInt32(Request["backid"]);
            int eid = (Session["user"] as User).Eid;
            string reason = Request["reason"];
            if (Request["agree"] != null && Request["disagree"] == null)
            {
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string nextid = preSign.Rows[0]["nextstep"].ToString();
                List<string> signlist = new List<string>(preSign.Rows[0]["signposlist"].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                int index = signlist.IndexOf(nextid) + 1;
                nextid = signlist.ElementAt(index);
                string lastid = signlist.ElementAt(signlist.Count - 1);
                if (nextid.Equals(lastid))
                {
                    SQLHelper.ProcNoQuery("usp_SignSuccess", new SqlParameter("@nextstep", nextid), new SqlParameter("@id", formflowid));
                }
                else
                {
                    SQLHelper.ProcNoQuery("usp_SignNext", new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                }
                SQLHelper.ProcNoQuery("usp_SignAgreeInfo", new SqlParameter("formflowid", formflowid), new SqlParameter("@eid", eid), new SqlParameter("time", DateTime.Now), new SqlParameter("reason", reason));
            }
            else if (Request["agree"] == null && Request["disagree"] != null)
            {
                SQLHelper.ProcNoQuery("usp_SignFail", new SqlParameter("id", formflowid));
                SQLHelper.ProcNoQuery("usp_SignRefuseInfo", new SqlParameter("formflowid", formflowid), new SqlParameter("@eid", eid), new SqlParameter("time", DateTime.Now), new SqlParameter("reason", reason));
            }
            if (backid == 4)
            {
                return RedirectToAction("AgentSign", "Agent", new { pageindex = 1 });
            }
            else
            {
                return RedirectToAction("NextSignList", "List", new { pageindex = 1 });
            }
        }

        private static int AddForm(int innerid, int formid, int eid, DateTime datetime, string signlist)
        {
            SqlParameter[] parameters = { new SqlParameter("@innerid", SqlDbType.Int), new SqlParameter("@formid", SqlDbType.Int), new SqlParameter("@eid", SqlDbType.Int), new SqlParameter("@writetime", SqlDbType.DateTime), new SqlParameter("@signlist", SqlDbType.VarChar), new SqlParameter("@id", SqlDbType.Int) };
            parameters[0].Value = innerid;
            parameters[1].Value = formid;
            parameters[2].Value = eid;
            parameters[3].Value = datetime;
            parameters[4].Value = signlist;
            parameters[5].Direction = ParameterDirection.Output;
            SQLHelper.ProcNoQuery("usp_WriteSave", parameters);
            return Convert.ToInt32(parameters[5].Value);
        }

        private static int UpdateForm(int formflowid)
        {
            return SQLHelper.ProcNoQuery("usp_WriteModify", new SqlParameter("@id", formflowid), new SqlParameter("time", DateTime.Now));
        }

        private static int UpdateForm(int formflowid, string signlist)
        {
            return SQLHelper.ProcNoQuery("usp_WriteModifySignList", new SqlParameter("@id", formflowid), new SqlParameter("@time", DateTime.Now), new SqlParameter("@signlist", signlist));
        }

        private static List<string> CalcSignList(int flowid, int eid)
        {
            List<string> signlist = new List<string>();
            DataTable dt = SQLHelper.ProcDataTable("usp_WriteDetail", new SqlParameter("@id", flowid));
            string flow = dt.Rows[0]["flow"].ToString();
            string[] flowlist = flow.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            int temp;
            dt = SQLHelper.ProcDataTable("usp_PosId", new SqlParameter("@id", eid));
            string posid = dt.Rows[0]["positionid"].ToString();
            for (int i = 0; i < flowlist.Length - 1; i++)
            {
                if (int.TryParse(flowlist[i], out temp))
                {
                    if (!signlist.Contains(flowlist[i]))
                    {
                        signlist.Add(flowlist[i]);
                    }
                    continue;
                }
                if (flowlist[i].Equals("U"))
                {
                    dt = SQLHelper.ProcDataTable("usp_SuperiorPosId", new SqlParameter("@id", eid));
                    signlist.Add(dt.Rows[0]["superiorposid"].ToString());
                    continue;
                }

                if (flowlist[i].Equals("T"))
                {
                    signlist.Add(posid);
                    continue;
                }
                if (flowlist[i].Equals("D"))
                {
                    dt = SQLHelper.ProcDataTable("usp_DepPosId", new SqlParameter("@id", eid));
                    string depposid = dt.Rows[0]["directorposid"].ToString();
                    if (posid.Equals("1") || posid.Equals("2") || posid.Equals(depposid))
                    {
                        continue;
                    }
                    else
                    {
                        List<string> tempSuper = new List<string>();
                        int supereid = eid;
                        string superposid = string.Empty;
                        do
                        {
                            dt = SQLHelper.ProcDataTable("usp_SuperiorPosId", new SqlParameter("@id", supereid));
                            superposid = dt.Rows[0]["superiorposid"].ToString();
                            if (!superposid.Equals(depposid))
                            {
                                tempSuper.Add(superposid);
                            }
                            dt = SQLHelper.ProcDataTable("usp_SuperEid", new SqlParameter("@id", supereid));
                            supereid = Convert.ToInt32(dt.Rows[0]["employeeid"]);
                        }
                        while (!superposid.Equals(depposid));
                        foreach (string item in tempSuper)
                        {
                            if (!signlist.Contains(item))
                            {
                                signlist.Add(item);
                            }
                        }
                        if (!signlist.Contains(depposid))
                        {
                            signlist.Add(depposid);
                        }
                        continue;
                    }
                }
            }
            signlist.Add(flowlist[flowlist.Length - 1]);
            return signlist;
        }

        public ActionResult JBInfo(Dictionary<string, int> pars, JBInfo jbinfo)
        {
            //初始化下拉列表的数据信息            
            ViewData["typelist"] = UDS.Models.JBInfo.GetTypeList();
            ViewBag.Before = -2;            //开始日期的选择范围限制
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.JBInfo.UpdateInfo(jbinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.JBInfo.AddInfo(jbinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.JBInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        public ActionResult QJInfo(Dictionary<string, int> pars, QJInfo qjinfo)
        {
            //初始化下拉列表的数据信息            
            ViewData["typelist"] = UDS.Models.QJInfo.GetTypeList();
            //开始日期的选择范围限制，该表单的参数
            ViewBag.Before = -2;
            ViewBag.HourPreDay = 8;
            ViewBag.EndWorkHour = 17;
            ViewBag.EndWorkMin = 0;
            ViewBag.BeginWorkHour = 9;
            ViewBag.BeginWorkMin = 0;
            int HoursCount = 16;
            string endPosId = "2";

            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.QJInfo.UpdateInfo(qjinfo, innerid);
                    //修改总表单中的签核步奏
                    int flowid = Convert.ToInt32(dtPreMain.Rows[0]["formid"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    if (qjinfo.TotalTime <= HoursCount)
                        if (signposlist.Contains(endPosId))
                        {
                            int startIndex = signposlist.IndexOf(endPosId);
                            int endIndex = signposlist.Count - 2;
                            signposlist.RemoveRange(startIndex, endIndex);
                        }
                    string signlist = string.Join("|", signposlist.ToArray());
                    UpdateForm(formflowid, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.QJInfo.AddInfo(qjinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    if (qjinfo.TotalTime <= HoursCount)
                        if (signposlist.Contains(endPosId))
                        {
                            int startIndex = signposlist.IndexOf(endPosId);
                            int endIndex = signposlist.Count - 2;
                            signposlist.RemoveRange(startIndex, endIndex);
                        }
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.QJInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        public ActionResult GCInfo(Dictionary<string, int> pars, GCInfo gcinfo)
        {
            //开始日期的选择范围限制
            ViewBag.Before = -2;
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.GCInfo.UpdateInfo(gcinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.GCInfo.AddInfo(gcinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.GCInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult FYBXInfo(Dictionary<string, int> pars, FYBXInfo fybxinfo)
        {
            //初始化暂支信息            

            //开始日期的选择范围限制，该表单的参数
            decimal MoneyLimit = 1000;
            string endPosId = "1";

            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.FYBXInfo.UpdateInfo(fybxinfo, innerid);
                    //修改总表单中的签核步奏
                    int flowid = Convert.ToInt32(dtPreMain.Rows[0]["formid"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    if (fybxinfo.Money <= MoneyLimit)
                        if (signposlist.Contains(endPosId))
                        {
                            int startIndex = signposlist.IndexOf(endPosId);
                            int endIndex = signposlist.Count - 2;
                            signposlist.RemoveRange(startIndex, endIndex);
                        }
                    string signlist = string.Join("|", signposlist.ToArray());
                    UpdateForm(formflowid, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    fybxinfo.AttachContent = fybxinfo.AttachContent ?? string.Empty;
                    int innerid = UDS.Models.FYBXInfo.AddInfo(fybxinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    if (fybxinfo.Money <= MoneyLimit)
                        if (signposlist.Contains(endPosId))
                        {
                            int startIndex = signposlist.IndexOf(endPosId);
                            int endIndex = signposlist.Count - 2;
                            signposlist.RemoveRange(startIndex, endIndex);
                        }
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.FYBXInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult SYQMKHInfo(Dictionary<string, int> pars, SYQMKHInfo syqmkhinfo)
        {
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.SYQMKHInfo.UpdateInfo(syqmkhinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.SYQMKHInfo.AddInfo(syqmkhinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.SYQMKHInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult HYJLInfo(Dictionary<string, int> pars, HYJLInfo hyjlinfo)
        {
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.HYJLInfo.UpdateInfo(hyjlinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    hyjlinfo.AttachContent = hyjlinfo.AttachContent ?? string.Empty;
                    int innerid = UDS.Models.HYJLInfo.AddInfo(hyjlinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.HYJLInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult CommonModelInfo(Dictionary<string, int> pars, CommonModelInfo commonmodelinfo)
        {
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.CommonModelInfo.UpdateInfo(commonmodelinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.CommonModelInfo.AddInfo(commonmodelinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.CommonModelInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult QTJLInfo(Dictionary<string, int> pars, QTJLInfo qtjlinfo)
        {
            //初始化下拉列表的数据信息
            ViewData["typelist"] = UDS.Models.QTJLInfo.GetTypeList();
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.QTJLInfo.UpdateInfo(qtjlinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.QTJLInfo.AddInfo(qtjlinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.QTJLInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult XZJXInfo(Dictionary<string, int> pars, XZJXInfo xzjxinfo)
        {
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.XZJXInfo.UpdateInfo(xzjxinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.XZJXInfo.AddInfo(xzjxinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.XZJXInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult PXInfo(Dictionary<string, int> pars, PXInfo pxinfo)
        {
            //初始化下拉列表的数据信息            
            ViewData["typelist"] = UDS.Models.PXInfo.GetTypeList();
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.PXInfo.UpdateInfo(pxinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.PXInfo.AddInfo(pxinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.PXInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        public ActionResult GDZCInfo(Dictionary<string, int> pars, GDZCInfo gdzcinfo)
        {
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.GDZCInfo.UpdateInfo(gdzcinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.GDZCInfo.AddInfo(gdzcinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.GDZCInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }

        public ActionResult ZZInfo(Dictionary<string, int> pars, ZZInfo zzinfo)
        {
            if (pars.ContainsKey("isNew"))
            {//新建表单时的空页面显示
                ViewBag.Display = 1;
                ViewBag.Old = 0;
                ViewBag.Id = pars["id"];
            }
            else if (Request["save"] != null)
            {
                string isOld = Request["isOld"];
                if (isOld.Equals("1"))
                {//保存修改的表单的处理逻辑
                    int formflowid = Convert.ToInt32(Request["id"]);
                    DataTable dtPreMain = SQLHelper.ProcDataTable("usp_PreMainInfo", new SqlParameter("@id", formflowid));
                    int innerid = Convert.ToInt32(dtPreMain.Rows[0]["forminnerid"]);
                    UDS.Models.ZZInfo.UpdateInfo(zzinfo, innerid);
                    UpdateForm(formflowid);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    return RedirectToAction("DraftContainer", "Detail", new { show = 1, isOld = 1, formflowid = formflowid });
                }
                else if (isOld.Equals("0"))
                {//新建表单时的保存处理逻辑
                    int innerid = UDS.Models.ZZInfo.AddInfo(zzinfo);
                    int flowid = int.Parse(Request["id"]);
                    int eid = (Session["user"] as User).Eid;
                    List<string> signposlist = CalcSignList(flowid, eid);
                    signposlist.RemoveAt(0);
                    string signlist = string.Join("|", signposlist.ToArray());
                    int formflowid = AddForm(innerid, flowid, eid, DateTime.Now, signlist);
                    ViewBag.Old = 1;
                    ViewBag.Id = formflowid;
                    ViewBag.Display = 1;
                    return PartialView();
                }
            }
            else if (Request["send"] != null)
            {//发送表单的处理逻辑
                int formflowid = Convert.ToInt32(Request["id"]);
                DataTable preSign = SQLHelper.ProcDataTable("usp_PreSign", new SqlParameter("@id", formflowid));
                string signlist = preSign.Rows[0]["signposlist"].ToString();
                int nextid = int.Parse(signlist.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SQLHelper.ProcNoQuery("usp_Send", new SqlParameter("@sendtime", DateTime.Now), new SqlParameter("@nextid", nextid), new SqlParameter("@id", formflowid));
                ViewBag.Display = 1;
                return RedirectToAction("OwnApplyList", "List", new { pageindex = 1 });
            }
            else if (Request["save"] == null && Request["send"] == null)
            {//用于显示表单详细信息的处理逻辑
                int innerid = pars["innerid"];
                int show = pars["show"];
                ViewData.Model = UDS.Models.ZZInfo.GetInfoById(innerid);
                ViewBag.Display = show;
                return PartialView();
            }
            return PartialView();
        }



    }
}
