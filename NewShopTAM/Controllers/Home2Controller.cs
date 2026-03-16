using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewShopTAM.Controllers
{
    public class Home2Controller : Controller
    {
        //
        // GET: /Home2/

        public ActionResult Index()
        {
            if (Session["UserType"] == null )
            {
                return RedirectToAction("LogIn", "Account");
            }
            else
            {
                string User = Session["UserID"].ToString();
                string UserType = Session["UserType"].ToString();
                string UsrCode = Session["CUSCOD"].ToString();


                ViewBag.UserId = User;
                ViewBag.UserType = UserType;
                ViewBag.UsrCode = UsrCode;


            }
            return View();
        }
        [HttpPost]
        public JsonResult SetSession(string name)
        {
            string User = Session["UserID"].ToString();
            string UserType = Session["UserType"].ToString();
            string UsrCode = Session["CUSCOD"].ToString();

            return Json(User);
        }

    }
}
