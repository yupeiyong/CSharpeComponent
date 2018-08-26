
using Autofac;
using DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Base;

namespace UnitTestProject
{
    [TestClass]
    public class BaseUnitTest
    {
        protected IContainer Container = ContainerFactory.GetContainer();

        public BaseUnitTest()
        {
            DataDbContext = Container.Resolve<DataDbContext>();
        }
        ~BaseUnitTest()
        {
            if (DataDbContext != null)
            {
                DataDbContext.Dispose();
            }
        }
        protected TService GetService<TService>()
        {
            return Container.Resolve<TService>();
        }

        protected DataDbContext DataDbContext { get; private set; }
    }
}
