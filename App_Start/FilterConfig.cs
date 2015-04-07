using System.Web;
using System.Web.Mvc;

namespace UDS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new UDS.Models.LoginActionFilterAttribute());
        }
    }
}