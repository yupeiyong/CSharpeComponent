using System.Collections.Generic;


namespace CSharpeComponents.Auth
{

    public interface IAuthProvider
    {
        IAuthFilter GetAuthFilter();

        void SetAuthFilter(IAuthFilter filter);

        /// <summary>
        ///     无需登录就可以访问的地址
        /// </summary>
        /// <returns></returns>
        List<string> GetFreeAccessUrls();


        /// <summary>
        ///     登录后即可访问的地址
        /// </summary>
        /// <returns></returns>
        List<string> GetDefaultAccessUrls();


        /// <summary>
        ///     用户信息关键字
        /// </summary>
        /// <returns></returns>
        string GetUserInfoKey();


        /// <summary>
        ///     登录页面
        /// </summary>
        /// <returns></returns>
        string GetLoginPageUrl();


        /// <summary>
        ///     没有权限时，跳转的页面
        /// </summary>
        /// <returns></returns>
        string GetAccessDenyPageUrl();


        List<IAuth> GetUserAuths(object userId);

        List<IRoleAuth> GetUserRoles(object userId);

    }

}