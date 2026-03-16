using NewShopTAM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace NewShopTAM.Controllers
{
    public class PortalForSalesController : Controller
    {
        //
        // GET: /PortalForSales/

        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return Redirect("https://mst.aac.co.th/MobileCatalog/Account/LoginCus");
            }

            return View();
        }
        public JsonResult GetSLMList(string SLM)
        {
            string message = string.Empty;
            List<object> SlmList = new List<object>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            try
            {
                Connection.Open();
                var cmd = new SqlCommand("P_Check_Sup_By_Slmcode", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InSlmcode", SLM);
                SqlDataReader rev = cmd.ExecuteReader();
                while (rev.Read())
                {
                    SlmList.Add(new { Value = rev["SLMCOD"].ToString(), Text = rev["SLMCOD"].ToString() + "/" + rev["SLMNAM"].ToString() });
                }
                message = "Y";
                rev.Close();
                rev.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Connection.Close();
            }
            return Json(new { message = message, SlmList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetcustomerList(string SLM)
        {
            string message = string.Empty;
            List<CUS> CUSList = new List<CUS>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            try
            {
                Connection.Open();
                var cmd = new SqlCommand("P_Get_CusSale_CustomerPortal", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inSLM", SLM);
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
            return Json(new { message = message, CUSList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomerSelected(string CUSCOD)
        {
            string message = string.Empty;
            if (!string.IsNullOrWhiteSpace(CUSCOD))
            {
                message = "Y";
            }
            else
            {
                message = "N";
            }
            this.Session["CUSCOD"] = CUSCOD;
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }

    }
}
