using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTestAuthProvider:BaseUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var provider = GetService<AuthProvider>();
            var denyUrl=provider.GetAccessDenyPageUrl();

            var auths = provider.GetAuths();

            var roles = provider.GetRoleAuths();

        }
    }
}
