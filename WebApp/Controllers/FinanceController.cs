using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class FinanceController : Controller
    {
        // GET: Finance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewRpt()
        {
            return View();
        }
    }
}