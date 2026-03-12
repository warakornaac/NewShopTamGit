using NewShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace NewShop.Controllers
{
    public class ManagementCustomerportalUserController : Controller
    {
        //
        // GET: /ManageCustomerportalUser/

        public ActionResult Index()
        {
            string message = string.Empty;

            List<CUS> CUSList = new List<CUS>();

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {

                SqlCommand cmd = new SqlCommand("select * from v_CUSPROV order by SLMCOD", Connection);
                SqlDataReader rev_CUSPROV = cmd.ExecuteReader();
                while (rev_CUSPROV.Read())
                {
                    CUSList.Add(new CUS()
                    {
                        CUSCOD = rev_CUSPROV["CUSCOD"].ToString(),
                        CUSNAM = rev_CUSPROV["CUSNAM"].ToString(),
                        PRO = rev_CUSPROV["PRO"].ToString(),
                        ADDR_01 = rev_CUSPROV["ADDR_01"].ToString(),
                        CUSTYP = rev_CUSPROV["CUSTYP"].ToString(),
                        TAMCRLINE = rev_CUSPROV["TAMCRLINE"].ToString(),
                        TAMBAL = rev_CUSPROV["TAMBAL"].ToString(),
                        VELOXCRLINE = rev_CUSPROV["VELOXCRLINE"].ToString(),
                        VELOXBAL = rev_CUSPROV["VELOXBAL"].ToString(),
                    });
                }
                ViewBag.GetCustomer = CUSList;
                rev_CUSPROV.Close();
                rev_CUSPROV.Dispose();
                cmd.Dispose();

                Connection.Close();
                message = "Y";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return View();
        }

        public JsonResult GetUsername()
        {
            string message = string.Empty;
            List<customerPortalUser> getData = new List<customerPortalUser>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_GetUser_CustomerPortal", Connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    getData.Add(new customerPortalUser()
                    {
                        Username = Reader["Username"] != DBNull.Value ? Reader["Username"].ToString() : "",
                        Cuscode = Reader["CusCode"] != DBNull.Value ? Reader["CusCode"].ToString() : "",
                        CusName = Reader["CusName"] != DBNull.Value ? Reader["CusName"].ToString() : ""
                    });
                }
                Reader.Close();
                command.Dispose();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { message = message, getData }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ChangeUserCus(string CUSCOD, string username, string NewCUSCOD)
        {
            string message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_ChangeCuscod_CustomerPortal", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCuscod", CUSCOD.Trim());
                command.Parameters.AddWithValue("@inUsername", username.Trim());
                command.Parameters.AddWithValue("@inNewCuscod", NewCUSCOD.Trim());
                SqlParameter p = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                p.Direction = ParameterDirection.Output;
                command.Parameters.Add(p);
                command.ExecuteNonQuery();
                message = command.Parameters["@outGenstatus"].Value.ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }



    }
}
