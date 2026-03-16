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


namespace NewShopTAM.Controllers
{
    public class ChangePasswordController : Controller
    {
        //
        // GET: /ChangePassword/

        public ActionResult Index()
        {//this.Session["UserType"] = "";
            //this.Session["UserType"] = "";
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");

            }

            return View();
          
        }
        public JsonResult Changepassword(string userlogin, string currentpass, string newpass, string cfpass)
        {
            string exerror = string.Empty;
            string returnmessage = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {

                var command = new SqlCommand("P_ChangepasswordCustomer", Connection);
                // var command = new SqlCommand("P_ShoppingCart_list", Connection);
                
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userlogin", userlogin);
                command.Parameters.AddWithValue("@currentpass", currentpass);
                command.Parameters.AddWithValue("@newpass", newpass);
                command.Parameters.AddWithValue("@cfpass", cfpass);
              
               

                SqlParameter returnValue = new SqlParameter("@outResult", SqlDbType.NVarChar, 500);
                returnValue.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnValue);
                command.ExecuteNonQuery();
                returnmessage = returnValue.Value.ToString();
            }
            catch (Exception ex)
            {

                exerror = ex.Message + '/' + ex.Source + '/' + ex.HelpLink + '/' + ex.HResult;

            }
            Connection.Close();

            return Json(new { returnmessage, exerror }, JsonRequestBehavior.AllowGet);
        }
    }
}
