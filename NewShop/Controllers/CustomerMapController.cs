using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewShop.Controllers;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using NewShop.Models;


namespace NewShop.Controllers
{
    public class CustomerMapController : Controller
    {
        // GET: /CustomerMap/
        public ActionResult Index()
        {
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            return View();
        }
        public JsonResult Getlocationbysales(string SLM)
        {
            string Message = string.Empty;
            List<Listloc> Listloc = new List<Listloc>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("P_Get_CusProvMapSales", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inSLM", SLM);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Listloc.Add(new Listloc()
                    {
                        CUSCOD = dr["CUSCOD"].ToString(),
                        Latitude = dr["Latitude"].ToString(),
                        Longitude = dr["Longitude"].ToString(),
                        Remark = dr["Remark"].ToString(),
                        ShipCode = dr["ShipCode"].ToString(),
                        ShipStatus = dr["ShipStatus"].ToString(),
                        CUSNAM = dr["Cusnam"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            Connection.Close();
            return Json(new { Listloc, Message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getlocation(string CUSID, string SHIPCODE)
        {
            string lat = string.Empty;
            string log = string.Empty;
            string re = string.Empty;
            string Message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<Listloc> Listloc = new List<Listloc>();
            try
            {
                 SqlCommand cmd = new SqlCommand("P_Get_CusProvMap", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCUSCOD", CUSID);
                cmd.Parameters.AddWithValue("@inSHIPCODE", SHIPCODE);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Listloc.Add(new Listloc()
                    {
                        CUSCOD = dr["CUSCOD"].ToString(),
                        Latitude = dr["Latitude"].ToString(),
                        Longitude = dr["Longitude"].ToString(),
                        Remark = dr["Remark"].ToString(),
                        ShipCode = dr["ShipCode"].ToString(),
                        ShipStatus = dr["ShipStatus"].ToString(),
                        CUSNAM = dr["Cusnam"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            Connection.Close();
            return Json(new { Listloc, Message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Insertdatacuspromap(string usrRemark, string CUSID, string SLMID, string Latitude, string Longitude, string Usrlogin, string SHIPCODE)
        {
            string Message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                SqlCommand cmd = new SqlCommand("P_Save_CusProvMap", Connection);
                cmd.Connection = Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCUSCOD", CUSID);
                cmd.Parameters.AddWithValue("@inSLMCODE", SLMID);
                cmd.Parameters.AddWithValue("@inLatitude", Latitude);
                cmd.Parameters.AddWithValue("@inLongitude", Longitude);
                cmd.Parameters.AddWithValue("@inInserted_By", Usrlogin);
                cmd.Parameters.AddWithValue("@inRemark", usrRemark);
                cmd.Parameters.AddWithValue("@inSHIPCODE", SHIPCODE);
                cmd.ExecuteNonQuery();

                cmd.Dispose();

                Message = "true";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            Connection.Close();
            Connection.Dispose();
            SqlConnection.ClearPool(Connection);
            return Json(Message, JsonRequestBehavior.AllowGet);
        }
    }
}
