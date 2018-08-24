using System.Web.Mvc;


namespace WebApp.Controllers
{

    public class BaseController : Controller
    {
        public class Result
        {
            // Properties
            public object Data { get; set; }

            public string msg { get; set; }

            public int status { get; set; }

            public bool success { get; set; }
        }
        //protected override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    //不能应用在子方法上
        //    if (filterContext.IsChildAction)
        //        return;


        //    if (CurrentManager == null)
        //    {
        //        if (WebHelper.IsAjax())
        //        {
        //            Result result = new Result();
        //            result.msg = "登录超时,请重新登录！";
        //            result.success = false;
        //            filterContext.Result = Json(result);
        //            return;
        //        }
        //        else
        //        {
        //            var result = RedirectToAction("", "Login", new {area = "admin"});
        //            filterContext.Result = result;
        //            return;

        //            //跳转到登录页
        //        }
        //    }
        //    var actionFilter = filterContext.ActionDescriptor.GetCustomAttributes(typeof (UnAuthorize), false);
        //    if (actionFilter.Length == 1)
        //        return;
        //    var controllerName = filterContext.RouteData.Values["controller"].ToString().ToLower();
        //    var actionName = filterContext.RouteData.Values["action"].ToString().ToLower();
        //    if (CurrentManager.AdminPrivileges == null || CurrentManager.AdminPrivileges.Count == 0 || !AdminPermission.CheckPermissions(CurrentManager.AdminPrivileges, controllerName, actionName))
        //    {
        //        if (WebHelper.IsAjax())
        //        {
        //            Result result = new Result
        //            {
        //                msg = "你没有访问的权限！",
        //                success = false
        //            };
        //            filterContext.Result = Json(result);
        //        }
        //        else
        //        {
        //            //跳转到错误页
        //            var result = new ViewResult {ViewName = "NoAccess"};
        //            result.TempData.Add("Message", "你没有权限访问此页面");
        //            result.TempData.Add("Title", "你没有权限访问此页面！");
        //            filterContext.Result = result;
        //        }
        //    }
        //}

    }

}