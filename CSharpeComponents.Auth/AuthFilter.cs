using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace CSharpeComponents.Auth
{

    public class AuthFilter : IAuthFilter, IAuthorizationFilter
    {
        //private String contextPath;		// WebApp的虚拟路径
        private string userInfoKey;     // Session 中存放UserInfo对象的Key
        private string loginPageUrl;    // 应用程序的登录页面地址
        private string denyPageUrl;     // 拒绝访问的页面地址

        // 可以自由访问的URL
        private List<Regex> freeAccessUrls = new List<Regex>();

        // 缺省权限，登录后可以访问的URL,不需要在权限中进行配置
        private List<Regex> defaultAccessUrls = new List<Regex>();

        // 系统中所有的权限，authId 作为Key, AuthInfo作为Value
        private Dictionary<object, AuthInfo> auths = new Dictionary<object, AuthInfo>();

        // 系统中所有的角色，roleId 作为key，List<AuthInfo>作为Value
        private Dictionary<object, List<AuthInfo>> roles = null;

        // 用户权限，userId作为Key，List中的对象为AuthInfo
        private Dictionary<object, List<AuthInfo>> userAuths = new Dictionary<object, List<AuthInfo>>();

        public class Result
        {
            // Properties
            public object Data { get; set; }

            public string msg { get; set; }

            public int status { get; set; }

            public bool success { get; set; }
        }

        private IAuthProvider _authProvider;

        public AuthFilter(IAuthProvider provider)
        {

        }
        public bool CanAccess(object userId, string url)
        {
            throw new NotImplementedException();
        }


        public void ReloadRoleAuth()
        {
            throw new NotImplementedException();
        }
        // 权限信息
        public class AuthInfo
        {
            Object authId;          // 权限标识
            Regex pattern;    // url正则表达式解析后的模式

            public AuthInfo(Object authId, String url)
            {
                this.authId = authId;
                pattern = new Regex(url, RegexOptions.Compiled);
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //不能应用在子方法上
            if (filterContext.IsChildAction)
                return;

            var httpContext = filterContext.HttpContext;
            string urlStr = null;
            if (httpContext != null)
            {
                var uri = httpContext.Request.Url;
                urlStr = uri?.AbsolutePath;
            }
            object userId = null;
            var session = httpContext.Session;

            var userInfo = session[_authProvider.GetUserInfoKey()] as IUserInfo;

            if (userInfo == null)
            {
                if (IsAjax())
                {
                    JsonResult jsonResult = new JsonResult();
                    Result result = new Result
                    {
                        msg = "登录超时,请重新登录！",
                        success = false
                    };
                    jsonResult.Data = result;
                    filterContext.Result = jsonResult;
                    return;
                }
                else
                {
                    var result = (ActionResult)new RedirectResult("");
                    filterContext.Result = result;
                    return;
                    //跳转到登录页
                }

            }
            //object[] actionFilter = filterContext.ActionDescriptor.GetCustomAttributes(typeof(UnAuthorize), false);
            //if (actionFilter.Length == 1)
            //    return;
            //var controllerName = filterContext.RouteData.Values["controller"].ToString().ToLower();
            //var actionName = filterContext.RouteData.Values["action"].ToString().ToLower();
            //if (CurrentManager.AdminPrivileges == null || CurrentManager.AdminPrivileges.Count == 0 || !AdminPermission.CheckPermissions(CurrentManager.AdminPrivileges, controllerName, actionName))
            //{
            //    if (IsAjax())
            //    {
            //        Result result = new Result();
            //        result.msg = "你没有访问的权限！";
            //        result.success = false;
            //        filterContext.Result = Json(result);
            //        return;
            //    }
            //    else
            //    {
            //        //跳转到错误页
            //        var result = new ViewResult() { ViewName = "NoAccess" };
            //        result.TempData.Add("Message", "你没有权限访问此页面");
            //        result.TempData.Add("Title", "你没有权限访问此页面！");
            //        filterContext.Result = result;
            //        return;
            //    }
            //}
        }


        private bool IsAjax()
        {
            return HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        // 加载系统的权限数据
        private void loadAuths()
        {
            // 1. 获取Free Access Urls
            freeAccessUrls.Clear();
            List<string> freeUrlList = _authProvider.GetFreeAccessUrls();
            if (freeUrlList != null)
            {
                foreach (string url in freeUrlList)
                {
                    freeAccessUrls.Add(new Regex(url,RegexOptions.Compiled));
                }
            }

            // 2. 获取Default Access Urls
            defaultAccessUrls.Clear();
            List<String> defaultUrlList = _authProvider.GetDefaultAccessUrls();
            if (defaultUrlList != null)
            {
                foreach (string url in defaultUrlList)
                {
                    defaultAccessUrls.Add(new Regex(url, RegexOptions.Compiled));
                }
            }

            // 3. 获取系统的所有权限,放入auths Map中
            auths.Clear();
            List <IAuth > authList = _authProvider.GetAuths();
            if (authList == null || authList.Count < 1)
            {
                throw new Exception("系统权限为空！");
            }
            foreach (IAuth auth in authList)
            {
                var authId = auth.GetAuthId();
                String url = auth.GetUrl();
                AuthInfo ai = new AuthInfo(authId, url);
                auths.Add(authId, ai);
            }

            // 4. 获取系统的所有角色，解析角色包含的所有权限
            reloadRoleAuths();
        }

        public void reloadRoleAuths()
        {
            roles = loadRoleAuths();
        }

        private Dictionary<object, List<AuthInfo>> loadRoleAuths()
        {
            Dictionary<object, List<AuthInfo>> roleAuthMap = new Dictionary<object, List<AuthInfo>>();
            // 1. 从AuthProvider 接口查询系统的所有角色权限关系
            List <IRoleAuth > roleAuths = _authProvider.GetRoleAuths();
            if (roleAuths != null)
            {
                // 2. 将每个角色包含的权限Id解析出来
                Dictionary<object, List<AuthInfo>> roleMap = ParseRoleAuths(roleAuths);

                //// 3. 根据AuthId查找AuthInfo，组成链表，放入roles中
                //Set<Map.Entry<object, Set>> entrySet = roleMap.entrySet();
                //for (Map.Entry<object, Set> entry : entrySet)
                //{
                //    Set authIdSet = entry.getValue();
                //    List<AuthInfo> authInfoList = new ArrayList<AuthInfo>();
                //    for (Object authId : authIdSet)
                //    {
                //        authInfoList.add(auths.get(authId));
                //    }

                //    Object roleId = entry.getKey();
                //    roleAuthMap.put(roleId, authInfoList);
                //}
            }

            return roleAuthMap;
        }

        // 解析IRoleAuth链表，转换为Map，其中RoleId为Key，Set<AuthId>是Value
        public Dictionary<object, List<AuthInfo>> ParseRoleAuths(List<IRoleAuth> roleAuths)
        {
            Dictionary<object, List<AuthInfo>> roleMap = new Dictionary<object, List<AuthInfo>>();
            foreach (IRoleAuth roleAuth in roleAuths)
            {
                var roleId = roleAuth.GetRoleId();

                //Set authIdSet = roleMap.get(roleId);
                //if (authIdSet == null)
                //{
                //    authIdSet = new HashSet();
                //    getRoleAuths(roleId, roleAuths, authIdSet);

                //    roleMap.put(roleId, authIdSet);
                //}
            }

            return roleMap;
        }
    }

}