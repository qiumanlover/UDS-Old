using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UDS.Models
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class LoginActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            User user = HttpContext.Current.Session["user"] as User;
            if (user == null)
            {
                HttpContext.Current.Response.Redirect("/Login/Index");
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            User user = HttpContext.Current.Session["user"] as User;
            if (user == null)
            {
                HttpContext.Current.Response.Redirect("/Login/Index");
            }
        }
    }
}