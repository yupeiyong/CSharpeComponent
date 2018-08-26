using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace CSharpeComponents.Auth
{
    /// <summary>
    /// 1、加载系统所有权限
    /// 2、加载系统所有角色，每一个角色包含了此角色的所有权限，构造数据时，按角色权限标志确定角色树结构
    /// 3、用户权限，包含用户的权限和用户的角色下的所有权限
    /// </summary>
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
            this._authProvider = provider;
            loadAuths();
            loginPageUrl = _authProvider.GetLoginPageUrl();
            userInfoKey = _authProvider.GetUserInfoKey();
            denyPageUrl = _authProvider.GetAccessDenyPageUrl();
        }
        public bool CanAccess(object userId, string url)
        {
            // 1. 检查url 是否在DefaultAccessUrls 中
            foreach (var p in defaultAccessUrls)
            {
                if (p.IsMatch(url))
                {
                    return true;
                }
            }

            // 2. 执行权限检查
            List<AuthInfo> authInfoList = GetUserAuths(userId);
            if (authInfoList != null)
            {
                foreach (AuthInfo ai in authInfoList)
                {
                    if (ai.pattern.IsMatch(url))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // 查询指定用户的权限信息
        private List<AuthInfo> GetUserAuths(object userId)
        {
            List<AuthInfo> authInfoList = null;
            // 1. 先从缓存中查找
            if (userAuths.ContainsKey(userId))
            {
                authInfoList = userAuths[userId];
            }
            if (authInfoList == null)
            {   // 2. 缓冲中没有，再从AuthProvider 接口获取
                // 2.1 创建用户的权限集合
                var authSet = new HashSet<AuthInfo>();

                // 2.2 将用户的直接权限加入集合
                var userAuthList = _authProvider.GetUserAuths(userId);
                if (userAuthList != null)
                {
                    foreach (object authId in userAuthList)
                    {
                        AuthInfo ai = auths[authId];
                        if (ai != null)
                        {
                            authSet.Add(ai);
                        }
                    }
                }

                // 2.3 将用户角色包含的权限加入集合
                var userRoleList = _authProvider.GetUserRoles(userId);
                if (userRoleList != null)
                {
                    foreach (var roleId in userRoleList)
                    {
                        List<AuthInfo> aiList = roles[roleId];
                        foreach (var authInfo in aiList)
                        {
                            authSet.Add(authInfo);
                        }
                    }
                }

                // 2.4 生成权限列表，放入Map中
                authInfoList = new List<AuthInfo>(authSet);
                userAuths.Add(userId, authInfoList);
            }

            // 3. 返回用户的权限列表
            return authInfoList;
        }

        public void ReloadRoleAuth()
        {
            roles = loadRoleAuths();
        }
        // 权限信息
        public class AuthInfo
        {
            public object authId;          // 权限标识
            public Regex pattern;    // url正则表达式解析后的模式

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
            if (httpContext == null)
                return;

            string urlStr = null;
            if (httpContext != null)
            {
                var uri = httpContext.Request.Url;
                urlStr = uri?.AbsolutePath;
            }
            object userId = null;
            var session = httpContext.Session;
            var userInfo = session[_authProvider.GetUserInfoKey()] as IUserInfo;

            // 3. 判断是否为FreeAccessUrl
            if (isFreeAccessUrl(urlStr))
            {
                if (userId != null && userInfo == null)
                {   // 用户Logout
                    userAuths.Remove(userId);   // 清除用户的权限数据

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
                        var result = (ActionResult)new RedirectResult(loginPageUrl);
                        filterContext.Result = result;
                        return;
                        //跳转到登录页
                    }
                }
                else if (userId == null && userInfo != null)
                {   // 用户Login
                    userId = userInfo.GetUserId();
                    GetUserAuths(userId);       // 加载用户的权限数据
                }
                return;
            }

            if (CanAccess(userId, urlStr))
                return;

            if (IsAjax())
            {
                JsonResult jsonResult = new JsonResult();
                Result result = new Result
                {
                    msg = "你没有访问的权限！",
                    success = false
                };
                jsonResult.Data = result;
                filterContext.Result = jsonResult;
                return;
            }
            else
            {
                //跳转到错误页
                var result = (ActionResult)new RedirectResult(denyPageUrl);
                filterContext.Result = result;
                return;
            }
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
                    freeAccessUrls.Add(new Regex(url, RegexOptions.Compiled));
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
            List<IAuth> authList = _authProvider.GetAuths();
            if (authList == null || authList.Count < 1)
            {
                throw new Exception("系统权限为空！");
            }
            foreach (IAuth auth in authList)
            {
                var authId = auth.GetAuthId();
                string url = auth.GetUrl();
                AuthInfo ai = new AuthInfo(authId, url);
                auths.Add(authId, ai);
            }

            // 4. 获取系统的所有角色，解析角色包含的所有权限
            ReloadRoleAuth();
        }


        private Dictionary<object, List<AuthInfo>> loadRoleAuths()
        {
            Dictionary<object, List<AuthInfo>> roleAuthMap = new Dictionary<object, List<AuthInfo>>();
            // 1. 从AuthProvider 接口查询系统的所有角色权限关系
            List<IRoleAuth> roleAuths = _authProvider.GetRoleAuths();
            if (roleAuths != null)
            {
                // 2. 将每个角色包含的权限Id解析出来
                Dictionary<object, HashSet<object>> roleMap = ParseRoleAuths(roleAuths);

                // 3. 根据AuthId查找AuthInfo，组成链表，放入roles中
                foreach (var roleId in roleMap.Keys)
                {
                    HashSet<object> authInfoList = roleMap[roleId];
                    foreach (var authId in authInfoList)
                    {
                        authInfoList.Add(auths[authId]);
                    }
                }
            }

            return roleAuthMap;
        }

        // 解析IRoleAuth链表，转换为Map，其中RoleId为Key，Set<AuthId>是Value
        public Dictionary<object, HashSet<object>> ParseRoleAuths(List<IRoleAuth> roleAuths)
        {
            var roleMap = new Dictionary<object, HashSet<object>>();
            foreach (IRoleAuth roleAuth in roleAuths)
            {
                var roleId = roleAuth.GetRoleId();

                if (!roleMap.ContainsKey(roleId))
                {
                    var authIdSet = new HashSet<object>();
                    getRoleAuths(roleId, roleAuths, authIdSet);

                    roleMap.Add(roleId, authIdSet);
                }
            }

            return roleMap;
        }

        // 将RoleId包含的权限id放入authIdSet中，
        // 对包含的角色逐级解析出权限，也放入authIdSet中
        private void getRoleAuths(object roleId, List<IRoleAuth> roleAuths, HashSet<object> authIdSet)
        {
            foreach (IRoleAuth roleAuth in roleAuths)
            {
                if (roleAuth.GetRoleId() == roleId)
                {
                    if (roleAuth.RoleAuthFlag == (int)RoleAuthFlagEnum.Auth)
                    {
                        authIdSet.Add(roleAuth.GetAuthId());
                    }
                    else
                    {
                        getRoleAuths(roleAuth.GetAuthId(), roleAuths, authIdSet);
                    }
                }
            }
        }


        private bool isFreeAccessUrl(string url)
        {
            if (isPredefinedUrl(url))
            {
                return true;
            }

            foreach (Regex p in freeAccessUrls)
            {
                if (p.IsMatch(url))
                {
                    return true;
                }
            }

            return false;
        }

        private bool isPredefinedUrl(string url)
        {
            if (url.Equals("/") || url.StartsWith("/index.")
                    || url.StartsWith("/welcome.")
                    || url.Equals(loginPageUrl) || url.Equals(denyPageUrl))
            {
                return true;
            }
            return false;
        }
    }

}