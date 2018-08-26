using System.Web;
using System.Web.Mvc;
using CSharpeComponents.Auth;
using Service;
using WebApp.Base;
using Autofac;

namespace WebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            IContainer Container = ContainerFactory.GetContainer();

            var AuthProvider = Container.Resolve<AuthProvider>();
            filters.Add(new AuthFilter(AuthProvider));
            filters.Add(new HandleErrorAttribute());
        }
    }
}
