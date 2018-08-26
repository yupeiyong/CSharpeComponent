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
    }

}