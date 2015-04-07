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
                    return View();
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
    }
}
