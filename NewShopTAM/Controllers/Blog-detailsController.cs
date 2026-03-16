using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewShopTAM.Controllers
{
    public class Blog_detailsController : Controller
    {
        //
        // GET: /Blog-details/

        public ActionResult Index()
        {
            //this.Session["UserType"] = "";
            if (this.Session["UserType"] == null)
            {
                this.Session["UserType"] = "";
            }
            return View();
        }
       
        public JsonResult GetAlotOfUsers()
        {
            var employees = new List<object>();

            employees.Add(new { Name = "Name 1", Surname = "Surname 1" });
            employees.Add(new { Name = "Name 2", Surname = "Surname 2" });
            employees.Add(new { Name = "Name 3", Surname = "Surname 3" });
            employees.Add(new { Name = "Name 4", Surname = "Surname 4" });
            employees.Add(new { Name = "Name 5", Surname = "Surname 5" });
            employees.Add(new { Name = "Name 6", Surname = "Surname 6" });
            employees.Add(new { Name = "Name 7", Surname = "Surname 7" });
            employees.Add(new { Name = "Name 8", Surname = "Surname 8" });
            employees.Add(new { Name = "Name 9", Surname = "Surname 9" });
            employees.Add(new { Name = "Name 10", Surname = "Surname 10" });

            return Json(employees, JsonRequestBehavior.AllowGet);
        }
    }
}
