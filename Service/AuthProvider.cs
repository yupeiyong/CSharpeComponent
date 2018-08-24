using System;
using System.Collections.Generic;
using CSharpeComponents.Auth;


namespace Service
{

    public class AuthProvider : IAuthProvider
    {

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
            throw new NotImplementedException();
        }


        public List<string> GetDefaultAccessUrls()
        {
            throw new NotImplementedException();
        }


        public string GetUserInfoKey()
        {
            throw new NotImplementedException();
        }


        public string GetLoginPageUrl()
        {
            throw new NotImplementedException();
        }


        public string GetAccessDenyPageUrl()
        {
            throw new NotImplementedException();
        }


        public List<IAuth> GetAuths()
        {
            throw new NotImplementedException();
        }


        public List<IRoleAuth> GetRoleAuths()
        {
            throw new NotImplementedException();
        }


        public List<IAuth> GetUserAuths(object userId)
        {
            throw new NotImplementedException();
        }


        public List<IRoleAuth> GetUserRoles(object userId)
        {
            throw new NotImplementedException();
        }

    }

}