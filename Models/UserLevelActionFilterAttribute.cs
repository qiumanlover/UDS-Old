using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UDS.Models
{
    public class UserLevelActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            User user = HttpContext.Current.Session["user"] as User;
            if (user.Userlevel == 1)
            {

            }
        }
    }
}