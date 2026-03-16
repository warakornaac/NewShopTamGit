using NewShopTAM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewShopTAM.Controllers
{
    public class StockCheckingController : Controller
    {
        //
        // GET: /StockChecking/

        public ActionResult Index()
        {
            if (this.Session["UserType"] == "" || this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            return View();
        }
        public JsonResult GetCheckingStock(string Company, string Stkcod)
        {
            string Usr = "Thiraphon.pra";
            string message = string.Empty;
            List<StkCanSale> Getdata = new List<StkCanSale>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("P_Search_Stock_CanSales", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@inCompany", Company);
                cmd.Parameters.AddWithValue("@inSTKCOD", Stkcod);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Getdata.Add(new StkCanSale()
                    {
                        WH = reader["WH"] != DBNull.Value ? reader["WH"].ToString() : string.Empty,
                        Company = reader["Company"] != DBNull.Value ? reader["Company"].ToString() : string.Empty,
                        STKCOD = reader["STKCOD"] != DBNull.Value ? reader["STKCOD"].ToString() : string.Empty,
                        STKDES = reader["STKDES"] != DBNull.Value ? reader["STKDES"].ToString() : string.Empty,
                        STKGRP = reader["STKGRP"] != DBNull.Value ? reader["STKGRP"].ToString() : string.Empty,
                        UOM = reader["UOM"] != DBNull.Value ? reader["UOM"].ToString() : string.Empty,
                        ItemQty = reader["ItemQty"] != DBNull.Value ? reader["ItemQty"].ToString() : string.Empty,
                        ReadyQty = reader["ReadyQty"] != DBNull.Value ? Convert.ToDecimal(reader["ReadyQty"]).ToString("0") : string.Empty,
                        MO_Qty = reader["Mobile_Qty"] != DBNull.Value ? Convert.ToDecimal(reader["Mobile_Qty"]).ToString("0") : string.Empty,
                        SO_Qty = reader["SO_Qty"] != DBNull.Value ? reader["SO_Qty"].ToString() : string.Empty,
                        BuffQty = reader["Buff_Qty"] != DBNull.Value ? Convert.ToDecimal(reader["Buff_Qty"]).ToString("0") : string.Empty,
                        RevQty = reader["RevQty"] != DBNull.Value ? Convert.ToDecimal(reader["RevQty"]).ToString("0") : string.Empty,
                        InStock = reader["InStock"] != DBNull.Value ? reader["InStock"].ToString() : string.Empty,
                        BckDue = reader["BckDue"] != DBNull.Value ? Convert.ToDecimal(reader["BckDue"]).ToString("0") : string.Empty,


                    });
                }
                message = "Y";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { message = message, Getdata }, JsonRequestBehavior.AllowGet);
        }

    }
}
