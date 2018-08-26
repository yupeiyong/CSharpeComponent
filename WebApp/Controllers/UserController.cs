
using Service;
using System;
using System.Web.Mvc;

namespace TelemarketingManagement.Controllers
{
    public class UserController : Controller
    {
        public UserService UserService { get; set; }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }



        //登录
        public JsonResult LoginSubmitJson(string loginName, string password)
        {
            try
            {
                var userInfo = UserService.Login(loginName, password);
                if (userInfo == null)
                    throw new Exception("错误：用户登录失败，注册数据为空，请联系系统管理员！");

                var result = new
                {
                    Title = "用户登录",
                    Message = "登录成功！",
                    Success = true,
                    RedirectUrl = "/SystemManage/Home"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    Title = "用户登录",
                    Message = $"登录失败！{ex.Message}",
                    Success = false,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public ActionResult OutLogin()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}