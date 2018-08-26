using System;
using System.Collections.Generic;
using CSharpeComponents.Auth;
using DataAccess;
using Models;
using System.Linq;

namespace Service
{

    public class AuthProvider : IAuthProvider
    {
        public DataDbContext DataDbContext { get; set; }

        public IAuthFilter GetAuthFilter()
        {
            throw new NotImplementedException();
        }


        public void SetAuthFilter(IAuthFilter filter)
        {
            throw new NotImplementedException();
        }


        public List<string> GetFreeAccessUrls()
        {
            List<String> list = new List<String>();
            list.Add("/User/Login");
            list.Add("/User/Logout");
            list.Add("/Pages/.*");
            return list;
        }


        public List<string> GetDefaultAccessUrls()
        {
            List<String> list = new List<String>();
            list.Add("/Pages/[^/]*");
            //list.add("/pages/[^/]*\\.html");
            return list;
        }


        public string GetUserInfoKey()
        {
            return "user_info_key";
        }


        public string GetLoginPageUrl()
        {
            return "/User/Login";
        }


        public string GetAccessDenyPageUrl()
        {
            return "/Deny";
        }


        public List<IAuth> GetAuths()
        {
            return DataDbContext.Set<Auth>().OrderBy(a=>a.Id).ToList<IAuth>();
        }


        public List<IRoleAuth> GetRoleAuths()
        {
            return DataDbContext.Set<RoleAuth>().OrderBy(ra=>ra.RoleId).ThenBy(ra=>ra.AuthId).ToList<IRoleAuth>();
        }


        public List<object> GetUserAuths(object userId)
        {
            if (userId == null)
                return new List<object>();

            long id;
            if(!long.TryParse(userId.ToString(),out id))
            {
                throw new Exception("参数：userId不是长整型数！");
            }
            return DataDbContext.Set<UserAuth>().Where(ua => ua.UserId == id && ua.RoleAuthFlag == 0).Select(ua=>(object)ua.AuthId).ToList();
        }


        public List<object> GetUserRoles(object userId)
        {
            if (userId == null)
                return new List<object>();

            long id;
            if (!long.TryParse(userId.ToString(), out id))
            {
                throw new Exception("参数：userId不是长整型数！");
            }
            return DataDbContext.Set<UserAuth>().Where(ua => ua.UserId == id && ua.RoleAuthFlag == 1).Select(ua => (object)ua.AuthId).ToList();
        }

    }

}