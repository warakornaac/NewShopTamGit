using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShopTAM.Controllers;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using NewShopTAM.Models;

namespace NewShopTAM.Controllers
{
    public class SeleScrCustomerController : Controller
    {
        //
        // GET: /SeleScrCustomer/

        public ActionResult Index()
        {
            //this.Session["UserType"] = "";
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");

            }
           
            return View();
        }
        public ActionResult dashboard()
        {
            //this.Session["UserType"] = "";
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");

            }
            

            return View();
        }
        public JsonResult Saveip(string ipno)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string messagereturn = string.Empty;
            SqlTransaction trans = null;
            try
            {
                SqlCommand cmd = new SqlCommand("p_SaveIP", Connection);
                cmd.Connection = Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IP", ipno);
               
                SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                returnValue.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(returnValue);
                cmd.ExecuteNonQuery();
                messagereturn = returnValue.Value.ToString();

            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                //return -1;
            }
            //return null;
            return Json(messagereturn, JsonRequestBehavior.AllowGet);
        }
    }
}
