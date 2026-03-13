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
    public class BackOrderAllController : Controller
    {
        // GET: /BackOrderAll/
        public ActionResult Index()
        {
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            return View();
        }
        public ActionResult Filter(string CUSCOD, string SLMID, string Show_Flag, string Stock_Flag, string stkcod, string DocumentNo, string COM, string UserType)
        {
            int intFlag = Convert.ToInt32(Show_Flag);
            int intstock = Convert.ToInt32(Stock_Flag);
            string SaleOrder_No = string.Empty;
            string CUS = string.Empty;
            string STKCOD = string.Empty;
            string SLM = string.Empty;
            int inqty = 0;
            int inconfirm = 0;
            string ConfirmOrderQty = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            if (DocumentNo == null) { DocumentNo = ""; }
            List<BackOrderbyItem> GetdatabyItem = new List<BackOrderbyItem>();
            BackOrderbyItem _model = new BackOrderbyItem();
            List<BackOrderbyDoc> GetdatabyDoc = new List<BackOrderbyDoc>();
            BackOrderbyDoc _modelDoc = new BackOrderbyDoc();
            var command = new SqlCommand("P_Search_BackOrder", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SaleCode", SLMID);
            command.Parameters.AddWithValue("@Show_Flag", intFlag);
            command.Parameters.AddWithValue("@Customer", CUSCOD);
            command.Parameters.AddWithValue("@StockCode", stkcod);
            command.Parameters.AddWithValue("@DocumentNo", DocumentNo);
            command.Parameters.AddWithValue("@Stock_Flag ", intstock);
            command.Parameters.AddWithValue("@Com", COM);
            command.Parameters.AddWithValue("@UsrTyp", UserType);
            command.ExecuteNonQuery();
            SqlDataReader drb = command.ExecuteReader();
            while (drb.Read())
            {
                if (Show_Flag == "1")
                {
                    inconfirm = Convert.ToInt32(drb["Qty"].ToString());
                    inqty = Convert.ToInt32(drb["ConfirmOrderQty"].ToString());
                    if (inqty < inconfirm)
                    {
                        ConfirmOrderQty = Convert.ToString(inqty);
                    }
                    else
                    {
                        ConfirmOrderQty = Convert.ToString(inconfirm);
                    }
                    GetdatabyDoc.Add(new BackOrderbyDoc()
                    {
                        ckrow = drb["Row"].ToString(),
                        SaleOrder_No = drb["SaleOrder_No"].ToString(),
                        SaleOrder_Date = Convert.ToDateTime(drb["SaleOrder_Date"]).ToString("dd/MM/yyyy"),
                        Amt = drb["Amt"].ToString(),
                        Inventory = drb["Inventory"].ToString(),
                        Cuscod = drb["CUSCOD"].ToString(),
                        Slm = drb["SLMCOD"].ToString(),
                        Qty = drb["Qty"].ToString(),
                        ConfirmOrderQty = Convert.ToString(inconfirm)
                    });
                }
                else if (Show_Flag == "2")
                {
                    inconfirm = Convert.ToInt32(drb["Qty"].ToString());
                    inqty = Convert.ToInt32(drb["ConfirmOrderQty"].ToString());
                    if (inqty < inconfirm)
                    {
                        ConfirmOrderQty = Convert.ToString(inqty);
                    }
                    else
                    {
                        ConfirmOrderQty = Convert.ToString(inconfirm);
                    }
                    string Pro = drb["PromotionCode"].ToString();
                    if (Pro != "") { Pro = Pro.Trim(); } else { Pro = ""; }
                    string display = string.Empty;
                    if (Pro != "") { display = "display: none;"; } else { display = ""; }
                    GetdatabyItem.Add(new BackOrderbyItem()
                    {
                        ckrow = "cKrow" + drb["Row"].ToString(),
                        Qtybackrow = "Qtybackrow" + drb["Row"].ToString(),
                        Qtyclearrow = "Qtyclearrow" + drb["Row"].ToString(),
                        Qtyleftoverkrow = "Qtyleftoverkrow" + drb["Row"].ToString(),
                        SaleOrder_No = drb["SaleOrder_No"].ToString(),
                        SaleOrder_Date = Convert.ToDateTime(drb["SaleOrder_Date"]).ToString("dd/MM/yyyy"),
                        STKCOD = drb["STKCOD"].ToString(),
                        STKDES = drb["STKDES"].ToString(),
                        STKGRP = drb["STKGRP"].ToString(),
                        Price = drb["Price"].ToString(),
                        SalePrice = drb["SalePrice"].ToString(),
                        Discount = drb["Discount"].ToString(),
                        CUSCOD = drb["CUSCOD"].ToString(),
                        CUSNAM = drb["CUSNAM"].ToString(),
                        SLMCOD = drb["SLMCOD"].ToString(),
                        SLMNAM = drb["SLMNAM"].ToString(),
                        Qty = drb["Qty"].ToString(),
                        ORDQTY = drb["ORDQTY"].ToString(),
                        TOTRES = drb["TOTRES"].ToString(),
                        SeqNum = drb["SeqNum"].ToString(),
                        ConfirmOrderQty = ConfirmOrderQty,
                        Amt = drb["Amt"].ToString(),
                        LineNote = drb["LineNote"].ToString(),
                        TOTBAL = drb["Inventory"].ToString(),
                        PromotionCode = drb["PromotionCode"].ToString(),
                        display = display,
                        Allocated = drb["Allocated"].ToString(),
                        REVQTY = drb["REVQTY"].ToString(),
                        InStock = drb["InStock"].ToString(),
                        DLVDAT = drb["DLVDAT"].ToString(),
                        COM = drb["COM"].ToString(),
                        Comment = drb["Comment"].ToString(),
                        Promo = drb["PromotionCode"].ToString(),
                        Totbck = drb["Totbck"].ToString(),
                        OrdTyp = drb["OrdTyp"].ToString(),
                        inactive = drb["inactive"].ToString(),
                        Blocked = drb["Blocked"].ToString(),
                        KDCQty = drb["KDCQty"].ToString(),
                        PDCQty = drb["PDCQty"].ToString(),
                    });
                }
            }
            drb.Close();
            drb.Dispose();
            command.Dispose();
            Connection.Close();
            if (Show_Flag == "2")
            {
                return PartialView("_PartialPage1", GetdatabyItem);
            }
            else
            {
                return PartialView("_PartialPagebyDoc", GetdatabyDoc);
            }
        }

        public JsonResult getbackorder(string CUSCOD, string SLMID, string Show_Flag, string Stock_Flag, string stkcod, string DocumentNo, string COM, string UserType)
        {
            int intFlag = Convert.ToInt32(Show_Flag);
            int intstock = Convert.ToInt32(Stock_Flag);
            string SaleOrder_No = string.Empty;
            string CUS = string.Empty;
            string STKCOD = string.Empty;
            string SLM = string.Empty;
            int inqty = 0;
            int inconfirm = 0;
            string ConfirmOrderQty = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            if (DocumentNo == null) { DocumentNo = ""; }
            List<BackOrderbyItem> GetdatabyItem = new List<BackOrderbyItem>();
            BackOrderbyItem _model = new BackOrderbyItem();
            List<BackOrderbyDoc> GetdatabyDoc = new List<BackOrderbyDoc>();
            BackOrderbyDoc _modelDoc = new BackOrderbyDoc();
            var command = new SqlCommand("P_Search_BackOrder", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SaleCode", SLMID);
            command.Parameters.AddWithValue("@Show_Flag", intFlag);
            command.Parameters.AddWithValue("@Customer", CUSCOD);
            command.Parameters.AddWithValue("@StockCode", stkcod);
            command.Parameters.AddWithValue("@DocumentNo", DocumentNo);
            command.Parameters.AddWithValue("@Stock_Flag ", intstock);
            command.Parameters.AddWithValue("@Com", COM);
            command.Parameters.AddWithValue("@UsrTyp", UserType);
            command.ExecuteNonQuery();
            SqlDataReader drb = command.ExecuteReader();
            while (drb.Read())
            {
                if (Show_Flag == "1")
                {
                    inconfirm = Convert.ToInt32(drb["Qty"].ToString());
                    inqty = Convert.ToInt32(drb["ConfirmOrderQty"].ToString());
                    if (inqty < inconfirm)
                    {
                        ConfirmOrderQty = Convert.ToString(inqty);
                    }
                    else
                    {
                        ConfirmOrderQty = Convert.ToString(inconfirm);
                    }
                    GetdatabyDoc.Add(new BackOrderbyDoc()
                    {
                        ckrow = drb["Row"].ToString(),
                        SaleOrder_No = drb["SaleOrder_No"].ToString(),
                        SaleOrder_Date = Convert.ToDateTime(drb["SaleOrder_Date"]).ToString("dd/MM/yyyy"),
                        Amt = drb["Amt"].ToString(),
                        Inventory = drb["Inventory"].ToString(),
                        Cuscod = drb["CUSCOD"].ToString(),
                        Slm = drb["SLMCOD"].ToString(),
                        Qty = drb["Qty"].ToString(),
                        ConfirmOrderQty = Convert.ToString(inconfirm)
                    });
                }
                else if (Show_Flag == "2")
                {
                    inconfirm = Convert.ToInt32(drb["Qty"].ToString());
                    inqty = Convert.ToInt32(drb["ConfirmOrderQty"].ToString());
                    if (inqty < inconfirm)
                    {
                        ConfirmOrderQty = Convert.ToString(inqty);
                    }
                    else
                    {
                        ConfirmOrderQty = Convert.ToString(inconfirm);
                    }
                    string Pro = drb["PromotionCode"].ToString();
                    if (Pro != "") { Pro = Pro.Trim(); } else { Pro = ""; }
                    string display = string.Empty;
                    if (Pro != "") { display = "display: none;"; } else { display = ""; }
                    GetdatabyItem.Add(new BackOrderbyItem()
                    {
                        ckrow = "cKrow" + drb["Row"].ToString(),
                        Qtybackrow = "Qtybackrow" + drb["Row"].ToString(),
                        Qtyclearrow = "Qtyclearrow" + drb["Row"].ToString(),
                        Qtyleftoverkrow = "Qtyleftoverkrow" + drb["Row"].ToString(),
                        SaleOrder_No = drb["SaleOrder_No"].ToString(),
                        SaleOrder_Date = Convert.ToDateTime(drb["SaleOrder_Date"]).ToString("dd/MM/yyyy"),
                        STKCOD = drb["STKCOD"].ToString(),
                        STKDES = drb["STKDES"].ToString(),
                        STKGRP = drb["STKGRP"].ToString(),
                        Price = drb["Price"].ToString(),
                        SalePrice = drb["SalePrice"].ToString(),
                        Discount = drb["Discount"].ToString(),
                        CUSCOD = drb["CUSCOD"].ToString(),
                        CUSNAM = drb["CUSNAM"].ToString(),
                        SLMCOD = drb["SLMCOD"].ToString(),
                        SLMNAM = drb["SLMNAM"].ToString(),
                        Qty = drb["Qty"].ToString(),
                        ORDQTY = drb["ORDQTY"].ToString(),
                        TOTRES = drb["TOTRES"].ToString(),
                        SeqNum = drb["SeqNum"].ToString(),
                        ConfirmOrderQty = ConfirmOrderQty,
                        Amt = drb["Amt"].ToString(),
                        LineNote = drb["LineNote"].ToString(),
                        TOTBAL = drb["Inventory"].ToString(),
                        PromotionCode = drb["PromotionCode"].ToString(),
                        display = display,
                        Allocated = drb["Allocated"].ToString(),
                        REVQTY = drb["REVQTY"].ToString(),
                        InStock = drb["InStock"].ToString(),
                        DLVDAT = drb["DLVDAT"].ToString(),
                        COM = drb["COM"].ToString(),
                        Comment = drb["Comment"].ToString(),
                        Promo = drb["PromotionCode"].ToString(),
                        Totbck = drb["Totbck"].ToString(),
                        shipto = drb["ShpTo"].ToString(),
                        CUSPO = drb["CUSPO"].ToString(),
                        Servicepart = drb["service Part"].ToString(),
                        OrdTyp = drb["OrdTyp"].ToString(),
                        inactive = drb["inactive"].ToString(),
                        Blocked = drb["Blocked"].ToString(),
                        KDCQty = drb["KDCQty"].ToString(),
                        PDCQty = drb["PDCQty"].ToString(),
                        Rating = drb["Rating"].ToString(),
                        PrdPstGrp = drb["PrdPstGrp"].ToString(),
                    });
                }
            }
            drb.Close();
            drb.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(GetdatabyItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertdataBackorder(string Cuspo, string User, string CUSCOD, string DocNo, string SeqNo, string VSTKCOD, string MyIndexValueQty, string MyIndexValueQtyclear, string MyIndexValueQtyLeftover, string company, string CComment)
        {
            string message = "false";
            string docgen = string.Empty;
            int intdoc = 0;
            string ValueBo = string.Empty;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("P_Save_BackOrder_catalog", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                    command.Parameters.AddWithValue("@inDocNo", DocNo);
                    command.Parameters.AddWithValue("@inSeqNo", SeqNo);
                    command.Parameters.AddWithValue("@inSTKCOD", VSTKCOD);
                    command.Parameters.AddWithValue("@inqty", MyIndexValueQty);
                    command.Parameters.AddWithValue("@inClearqty", MyIndexValueQtyclear);
                    command.Parameters.AddWithValue("@inRemqty", MyIndexValueQtyLeftover);
                    command.Parameters.AddWithValue("@Com", company);
                    command.Parameters.AddWithValue("@User ", User);
                    command.Parameters.AddWithValue("@Cuspo", Cuspo);
                    command.Parameters.AddWithValue("@inComment", CComment);
                    SqlParameter returnValue = new SqlParameter("@outResult", SqlDbType.NVarChar, 100);
                    returnValue.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(returnValue);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    message = returnValue.Value.ToString();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message, intdoc, ValueBo }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult genBackorder()
        {
            string message = "false";
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_BckOrder", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
                command.Dispose();
                message = "true";
            }
            catch (Exception ex)
            {
                message = "false" + ex;
            }
            Connection.Close();
            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}