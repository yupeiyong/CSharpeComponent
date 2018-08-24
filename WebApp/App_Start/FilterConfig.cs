using System.Web;
using System.Web.Mvc;
using CSharpeComponents.Auth;
using Service;


namespace WebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthFilter(new AuthProvider()));
            filters.Add(new HandleErrorAttribute());
        }
    }
}
