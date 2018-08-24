using System;
using System.Web.Mvc;


namespace CSharpeComponents.Auth
{

    public class AuthFilter : IAuthFilter, IAuthorizationFilter
    {

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


        public void OnAuthorization(AuthorizationContext filterContext)
        {
            throw new NotImplementedException();
        }

    }

}