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
    public class MonitorPackingController : Controller
    {
        //
        // GET: /MonitorPacking/

        public ActionResult Index()
        {
            string Message = string.Empty;
            List<SLM> SlmList = new List<SLM>();
            List<Lisrsta> Lisrsta = new List<Lisrsta>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();

            var command = new SqlCommand("P_Search_SLM_ALL", Connection);

            command.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                SlmList.Add(new SLM()
                {
                    SLMCOD = dr["SLMCOD"].ToString(),
                    SLMNAM = dr["SLMNAM"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();

            ViewBag.SlmList = SlmList;
            SqlCommand cmd = new SqlCommand("select * from v_StatusTrackingOrder ", Connection);

            SqlDataReader rev_ = cmd.ExecuteReader();
            while (rev_.Read())
            {
                Lisrsta.Add(new Lisrsta()
                {
                    ID = rev_["ID"].ToString(),
                    Des = rev_["Status"].ToString(),

                });
            }
            ViewBag.Lisrsta = Lisrsta;
            rev_.Close();
            rev_.Dispose();
            cmd.Dispose();

            Connection.Close();
            return View();
        }
        public JsonResult Getdatabyslm(string SLXX, string SLMNAM)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<CUS> CUSList = new List<CUS>();
            SqlCommand cmd = new SqlCommand("select * from V_CUSPROV where SLMCOD =N'" + SLXX + "' order by SLMCOD", Connection);

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
                    AACCrlimit = rev_CUSPROV["AACCRLINE"].ToString(),
                    AACBalance = rev_CUSPROV["AACBAL"].ToString(),
                    TACCrlimit = rev_CUSPROV["TACCRLINE"].ToString(),
                    TACBalance = rev_CUSPROV["TACBAL"].ToString()
                });
            }
            //rev_CUSPROV.Dispose();
            //S20161016
            rev_CUSPROV.Close();
            rev_CUSPROV.Dispose();
            cmd.Dispose();
            //E20161016
            Connection.Close();
            return Json(CUSList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult Getlocationbyadmin(string ST_SDate,string ST_EDate,string CarID, string Status, string Packingno, string Cuscod, string Salesorder, string Slmcod)
        {

            string Message = string.Empty;

            List<Listloc> Listloc = new List<Listloc>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {

                //SqlCommand cmd = new SqlCommand("P_Get_CusProvMapSales", Connection);
                SqlCommand cmd = new SqlCommand("P_Get_MoniterTrackingOrderDrive", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCarID", CarID);
                cmd.Parameters.AddWithValue("@inStatus", Status);
                cmd.Parameters.AddWithValue("@inPackingNo", Packingno);
                cmd.Parameters.AddWithValue("@inCuscod", Cuscod);
                cmd.Parameters.AddWithValue("@inSalesorder", Salesorder);
                cmd.Parameters.AddWithValue("@inSlmcod", Slmcod);
                cmd.Parameters.AddWithValue("@inST_SDate", ST_SDate);
                cmd.Parameters.AddWithValue("@inST_EDate", ST_EDate);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Listloc.Add(new Listloc()
                    {

                        //packing_no = dr["packing_no"].ToString(),
                        //sales_order = dr["sales_order"].ToString(),
                        //customer = dr["customer"].ToString(),
                        //customer_name = dr["customer_name"].ToString(),
                        //dlvr_code = dr["dlvr_code"].ToString(),
                        Latitude = dr["Latitude"].ToString(),
                        Longitude = dr["Longitude"].ToString(),
                        //Address = dr["Address"].ToString(),
                        //Contact = dr["Contact"].ToString(),
                        //Contact_Phone = dr["Contact_Phone"].ToString(),
                        Status = dr["Status"].ToString(),
                        //StatusStr = dr["StrStatus"].ToString(),
                        CarID = dr["Car-ID"].ToString(),
                        packing_no = dr["packinglist"].ToString(),
                        sales_order = dr["salesorder"].ToString(),
                        // NoOfBox = dr["NoOfBox"].ToString(),

                    });
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }


            Connection.Close();
            // return Json(CUSList, JsonRequestBehavior.AllowGet);
            return Json(new { Listloc, Message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getlocationdatailbyadmin(string ST_SDate, string CarID, string Status, string Packingno, string Cuscod, string Salesorder, string Slmcod)
        {

            string Message = string.Empty;

            List<Listloc> Listloc = new List<Listloc>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {

                //SqlCommand cmd = new SqlCommand("P_Get_CusProvMapSales", Connection);
                SqlCommand cmd = new SqlCommand("P_Get_MoniterTrackingOrder", Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCarID", CarID);
                cmd.Parameters.AddWithValue("@inStatus", Status);
                cmd.Parameters.AddWithValue("@inPackingNo", Packingno);
                cmd.Parameters.AddWithValue("@inCuscod", Cuscod);
                cmd.Parameters.AddWithValue("@inSalesorder", Salesorder);
                cmd.Parameters.AddWithValue("@inSlmcod", Slmcod);
                cmd.Parameters.AddWithValue("@inST_SDate", ST_SDate);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Listloc.Add(new Listloc()
                    {

                        packing_no = dr["packing_no"].ToString(),
                        sales_order = dr["sales_order"].ToString(),
                        customer = dr["customer"].ToString(),
                        customer_name = dr["customer_name"].ToString(),
                        dlvr_code = dr["dlvr_code"].ToString(),
                        Latitude = dr["Latitude"].ToString(),
                        Longitude = dr["Longitude"].ToString(),
                        Address = dr["Address"].ToString(),
                        Contact = dr["Contact"].ToString(),
                        Contact_Phone = dr["Contact_Phone"].ToString(),
                        Status = dr["Status"].ToString(),
                        StatusStr = dr["StrStatus"].ToString(),
                        CarID = dr["Car-ID"].ToString(),
                        StartWork = dr["StartWork"].ToString(),
                        EndWork = dr["EndWork"].ToString(),
                        Exptoarrive = dr["Exptoarrive"].ToString(),
                        //NoOfBox = dr["NoOfBox"].ToString(),

                    });
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }


            Connection.Close();
            // return Json(CUSList, JsonRequestBehavior.AllowGet);
            return Json(new { Listloc, Message }, JsonRequestBehavior.AllowGet);
        }



        public class Lisrsta
        {

            public string ID { get; set; }
            public string Des { get; set; }
        }
        public class Listloc
        {
            public string packing_no { get; set; }
            public string sales_order { get; set; }
            public string customer { get; set; }
            public string customer_name { get; set; }
            public string dlvr_code { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Address { get; set; }
            public string Contact { get; set; }
            public string Contact_Phone { get; set; }
            public string Status { get; set; }
            public string StatusStr { get; set; }
            public string NoOfBox { get; set; }
            public string CarID { get; set; }
            public string StartWork{ get; set; }
            public string EndWork{ get; set; }
	         public string Exptoarrive{ get; set; }
        }
        public class SLM
        {

            public string SLMCOD { get; set; }
            public string SLMNAM { get; set; }
        }
        public class CUS
        {
            public string CUSCOD { get; set; }
            public string CUSNAM { get; set; }
            public string PRO { get; set; }
            public string ADDR_01 { get; set; }
            public string ADDR_02 { get; set; }
            public string CUSTYP { get; set; }
            public string AACCrlimit { get; set; }
            public string AACBalance { get; set; }
            public string TACCrlimit { get; set; }
            public string TACBalance { get; set; }
            public string SLMCOD { get; set; }
            public string INACTIVE { get; set; }
            public string BLOCKED { get; set; }
            public string AACPAYTRM { get; set; }
            public string TACPAYTRM { get; set; }
            public string TELNUM { get; set; }
        }
    }
}
