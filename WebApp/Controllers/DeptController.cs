﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class DeptController : Controller
    {
        // GET: Dept
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewRequest()
        {
            return View();
        }

        public ActionResult WriteRequest()
        {
            return View();
        }
    }
}