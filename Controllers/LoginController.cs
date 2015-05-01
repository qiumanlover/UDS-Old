using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UDS.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginCheck()
        {
            string username = Request["username"];
            string inputpass = Request["password"];
            User user = new User();
            int result = user.CheckLogin(username, inputpass);
            if (result == 1)
            {
                Session["user"] = user;
                Guid guid = Guid.NewGuid();
                Session[user.Eid.ToString()] = guid.ToString();
                UDS.Models.GlobeLoad.Load(user.Eid.ToString(), guid.ToString());
                return RedirectToAction("UserPage", "Navigation");
            }
            else
            {
                ViewBag.Username = username;
                ViewBag.Password = "";
                switch (result)
                {
                    case -1:
                        ViewBag.Msg = "该用户不存在";
                        break;
                    case -2:
                        ViewBag.Msg = "该用户已被删除";
                        break;
                    case -3:
                        ViewBag.Msg = "密码错误";
                        break;
                    default:
                        break;
                }
                return View("Index");
            }
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CheckLoad()
        {
            HttpContext.Response.ContentType = "text/html";
            string name = Request["uid"];
            string uniid = Request["guid"];
            if (!string.IsNullOrEmpty(name) && Session != null)
            {
                if (!UDS.Models.GlobeLoad.CheckLoad(name, uniid))
                {
                    //return View();
                    //return Content(@"<script type='text/javascript'>alert('当前账号在其他地方登录, 请重新登录!');self.location.href = '/Login/Index';</script>");
                    return Content("Error");
                }
                else
                {
                    return Content("ok");
                }
            }
            else
            {
                return RedirectToAction("LogOut");
            }
        }

        public ActionResult ModifyPass()
        {
            ViewBag.Msg = Request["msg"] ?? "";
            if (Request["submit"] != null)
            {
                User user = Session["user"] as User;
                string curpass = Request["curpass"].ToString();
                if (user.Password.Equals(curpass))
                {
                    user.UpdatePass(Request["newpass"].ToString());
                    Session.Clear();
                    return Content("<script type='text/javascript'>alert('密码修改成功, 请重新登录!');self.location.href = '/Login/Index';</script>");
                }
                else
                {
                    ViewBag.Msg = "当前密码错误";
                    ViewBag.CurPass = curpass;
                    ViewBag.NewPass = Request["newpass"] ?? "";
                    ViewBag.NewPass2 = Request["newpass2"] ?? "";
                }                
            }
            return PartialView();
        }
    }
}
