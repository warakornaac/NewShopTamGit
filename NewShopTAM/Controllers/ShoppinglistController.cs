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
    public class ShoppinglistController : Controller
    {
        // GET: /Shoppinglist/

        public ActionResult Index()
        {
            if (this.Session["UserType"] == "")
            {
                return RedirectToAction("LogIn", "Account");
            }
            else
            {
                if (this.Session["UserType"] == null)
                {
                    return RedirectToAction("LogIn", "Account");
                }
                else
                {
                    string Docdisplay = string.Empty;
                    string CUSCOD = string.Empty;
                    Docdisplay = Request.QueryString["numcuber"];
                    if (Docdisplay != null)
                    {
                        byte[] data = System.Convert.FromBase64String(Docdisplay);
                        CUSCOD = System.Text.ASCIIEncoding.ASCII.GetString(data);
                    }
                    ViewBag.Nodisplay = CUSCOD;
                }
            }
            return View();
        }
        public JsonResult GetdateStockCode(string Name, string Xval, string Prod, string STKGR, string Xcus, string XvalCompany)
        {
            List<string> StockCode = new List<string>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_Item", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inCUSCOD", Xcus);
            command.Parameters.AddWithValue("@inSearch", Name);
            command.Parameters.AddWithValue("@inProd", "");
            command.Parameters.AddWithValue("@inSTKGRP", "");
            command.Parameters.AddWithValue("@inFix ", Xval);
            command.Parameters.AddWithValue("@Company", XvalCompany);
            Connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                StockCode.Add(dr.GetString(1));
            } 
            dr.Dispose();
            command.Dispose();
            Connection.Dispose();
            Connection.Close();
            return Json(StockCode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Gettemfoc(string strstockCode, string Company)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            PricelistpageingSearch Model = null;
            List<ListPagedList> Getdata = new List<ListPagedList>();
            var command = new SqlCommand("P_Search_Item_Foc", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inSTKCOD ", strstockCode);
            command.Parameters.AddWithValue("@Company ", Company);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                Model = new PricelistpageingSearch();
                Model.company = dr["company"].ToString();
                Model.STKCOD = dr["STKCOD"].ToString();
                Model.STKGRP_PRC = dr["STKGRP"].ToString();
                Model.STKDES = dr["STKDES"].ToString();
                Model.UOM = dr["UOM"].ToString();
                Model.TOTBAL = dr["TOTBAL"].ToString();
                Model.SPackUOM = dr["SPackUOM"].ToString();
                Getdata.Add(new ListPagedList { val = Model });
            }
            dr.Dispose();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataPricelistbytable(string SaleCode, string strcustome, string strstockCode, string User, string Company)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            PricelistpageingSearch Model = null;
            List<ListPagedList> Getdata = new List<ListPagedList>();
            string substkgrp = string.Empty;
            try
            {
                var command = new SqlCommand("P_Search_Pricelist", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SaleCode", SaleCode);
                command.Parameters.AddWithValue("@User", User);
                command.Parameters.AddWithValue("@Customer", strcustome);
                command.Parameters.AddWithValue("@Prod", "(ALL)");
                command.Parameters.AddWithValue("@StockGroup", "(ALL)");
                command.Parameters.AddWithValue("@StockCode", strstockCode);
                command.Parameters.AddWithValue("@Company", Company);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Model = new PricelistpageingSearch();
                    Model.PRCLST_NO = dr["PRCLST_NO"].ToString();
                    Model.PEOPLE = dr["PEOPLE"].ToString();
                    Model.CUSNAM = dr["CUSNAM"].ToString();
                    Model.SLMCOD = dr["SLMCOD"].ToString();
                    Model.STKCOD = dr["STKCOD"].ToString();
                    Model.STKDES = dr["STKDES"].ToString();
                    Model.Brand = dr["Brand"].ToString();
                    substkgrp = dr["STKGRP_PRC"].ToString();
                    Model.STKGRP_PRC = substkgrp.Substring(0, 2);
                    Model.minord = dr["minord"].ToString();
                    Model.Promotion = dr["PromoPrice"].ToString();
                    Model.LastInvUnitPric = dr["LastInvUnitPrice"].ToString();
                    Model.LastInvDisc = dr["LastInvDisc"].ToString();
                    Model.LastInvPrice = dr["LastInvPrice"].ToString();
                    string Ldate = dr["LastInvdate"].ToString();
                    if (Ldate != "")
                    {
                        DateTime dateLastInvdate = Convert.ToDateTime(dr["LastInvdate"].ToString());
                        string formattedLastInvdate = dateLastInvdate.ToString("dd/MM/yyyy");
                        Model.LastInvdate = formattedLastInvdate;
                    }
                    else
                    {
                        Model.LastInvdate = "-";
                    }
                    Model.TOTBAL = dr["TOTBAL"].ToString();
                    Model.Rcw = dr["RCW"].ToString();
                    Model.Totbck = dr["TotalBackOrder"].ToString();
                    Model.BackOrder = dr["BackOrder"].ToString();
                    Model.UOM = dr["UOM"].ToString();
                    Model.PCDES = dr["PCDES"].ToString();
                    Model.Price = dr["Price"].ToString();
                    Model.Price0 = dr["Price0"].ToString();
                    Model.SalePrice = dr["SalePrice"].ToString();
                    Model.Special_Price = dr["Special_Price"].ToString();
                    Model.spc_moq = dr["spc_moq"].ToString();
                    string spc_start_date = dr["spc_start_date"].ToString();
                    if (spc_start_date != "")
                    {
                        DateTime spc_s_date = Convert.ToDateTime(dr["spc_start_date"].ToString());
                        string formatspc_s_date = spc_s_date.ToString("dd/MM/yyyy");
                        Model.spc_start_date = formatspc_s_date;
                    }
                    else
                    {
                        Model.spc_start_date = dr["spc_start_date"].ToString();
                    }
                    string Ldate_end_dat = dr["spc_end_date"].ToString();
                    if (Ldate_end_dat != "")
                    {
                        DateTime dateend_dat = Convert.ToDateTime(dr["spc_end_date"].ToString());
                        string formatted_dateend_dat = dateend_dat.ToString("dd/MM/yyyy");
                        Model.spc_end_date = formatted_dateend_dat;

                        if (Convert.ToDateTime(Ldate_end_dat) > DateTime.Now)
                        {
                            Model.CCheck_date = "T";
                        }
                        else
                        {
                            Model.CCheck_date = "F";
                        }
                    }
                    else
                    {
                        Model.spc_end_date = "-";
                        Model.CCheck_date = "T";
                    }
                    Model.spc_remark = dr["spc_remark"].ToString();
                    Model.spc_PRODAPP = dr["spc_PRODAPP"].ToString();
                    Model.PRODNAM = dr["PRODNAM"].ToString();
                    Model.company = dr["company"].ToString();
                    Model.Special_Price = dr["Special_Price"].ToString();
                    Model.spc_moq = dr["spc_moq"].ToString();	
                    Model.spc_remark = dr["spc_remark"].ToString();
                    Model.PromotionCode = dr["PromotionCode"].ToString();
                    Model.PromoDesc = dr["PromoDesc"].ToString();
                    Model.PromoPrice = dr["PromoPrice"].ToString();
                    Model.PromoMOQ = dr["PromoMOQ"].ToString();
                    Model.PF = dr["PF"].ToString();
                    Model.Rack = dr["Rack"].ToString();
                    Model.SPackUOM = dr["SPackUOM"].ToString();
                    Model.Expected_Receipt_Date = dr["Expected Receipt Date"].ToString();
                    Model.Clearance = dr["Clearance"].ToString();
                    Model.Intransit = dr["Intransit"].ToString();
                    Model.ReworkCanSales = dr["ReworkCanSales"].ToString();
                    Model.ReworkClearance = dr["ReworkClearance"].ToString();
                    Getdata.Add(new ListPagedList { val = Model });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataShopping(string CUSCOD, string Usrlogin, string Qshow)
        {
            int sumQty = 0;
            int sumrow = 0;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_Count_Order", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Customer", CUSCOD);
            command.Parameters.AddWithValue("@usrlogin", Usrlogin);
            command.Parameters.AddWithValue("@StrQnuum", Qshow);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                sumQty = Convert.ToInt32(dr["SumQty"].ToString());
                sumrow = Convert.ToInt32(dr["CountStk"].ToString());
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(new { sumQty, sumrow }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Confirmationdata(string DataSend, string DataSendPro, string Cus, string User)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<ItemListshop> _ItemList = new JavaScriptSerializer().Deserialize<List<ItemListshop>>(DataSend);
            List<ItemFoc> _ItemListFoc = new JavaScriptSerializer().Deserialize<List<ItemFoc>>(DataSendPro);
            string messagereturn = string.Empty;
            SqlTransaction trans = null;
            try
            {
                if (_ItemList.Count > 0)
                {
                    for (int i = 0; i < _ItemList.Count; i++)
                    {
                        SqlCommand cmd = new SqlCommand("p_SaveOrderCart", Connection);
                        cmd.Connection = Connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Customer", _ItemList[i].CUSCOD);
                        cmd.Parameters.AddWithValue("@STKCOD", _ItemList[i].STKCOD);
                        cmd.Parameters.AddWithValue("@Company", _ItemList[i].company);
                        cmd.Parameters.AddWithValue("@Price", _ItemList[i].Price0);
                        cmd.Parameters.AddWithValue("@SPrice", _ItemList[i].SalePrice);
                        cmd.Parameters.AddWithValue("@Expect_Price", _ItemList[i].ExpectPrice);
                        cmd.Parameters.AddWithValue("@Qty", _ItemList[i].Qty);
                        cmd.Parameters.AddWithValue("@Bckorder", _ItemList[i].Qtybo);
                        cmd.Parameters.AddWithValue("@InsertedBy", _ItemList[i].User);
                        cmd.Parameters.AddWithValue("@LineNote", _ItemList[i].LineNote);
                        cmd.Parameters.AddWithValue("@FOC", "0");
                        cmd.Parameters.AddWithValue("@ProCode", _ItemList[i].PromotionCode);
                        cmd.Parameters.AddWithValue("@minord", _ItemList[i].Moq);
                        cmd.Parameters.AddWithValue("@prclstno", _ItemList[i].PRCLST_NO);
                        cmd.Parameters.AddWithValue("@specprice", _ItemList[i].Special_Price);
                        cmd.Parameters.AddWithValue("@spcRemark", _ItemList[i].Spc_Remark);
                        cmd.Parameters.AddWithValue("@promoprice", _ItemList[i].PromoPrice);
                        cmd.Parameters.AddWithValue("@promodesc", _ItemList[i].PromoDesc);
                        cmd.Parameters.AddWithValue("@lastinvprice", _ItemList[i].LastInvPrice);
                        cmd.Parameters.AddWithValue("@lastinvdate", _ItemList[i].LastInvdate);
                        cmd.Parameters.AddWithValue("@sop", _ItemList[i].Sop);
                        SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                        returnValue.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(returnValue);
                        cmd.ExecuteNonQuery();
                        messagereturn = returnValue.Value.ToString();
                    }
                }

                if (_ItemListFoc.Count > 0)
                {
                    for (int i = 0; i < _ItemListFoc.Count; i++)
                    {
                        SqlCommand cmd = new SqlCommand("p_SaveOrderCart_FOC", Connection);
                        cmd.Connection = Connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Customer", _ItemListFoc[i].VCUSCOD);
                        cmd.Parameters.AddWithValue("@STKCOD", _ItemListFoc[i].VSTKCOD);
                        cmd.Parameters.AddWithValue("@Company", _ItemListFoc[i].VCompany);
                        cmd.Parameters.AddWithValue("@Bckorder", _ItemListFoc[i].Backorderfoc);
                        cmd.Parameters.AddWithValue("@InsertedBy", User);
                        cmd.Parameters.AddWithValue("@LineNote", _ItemListFoc[i].VLineNote);
                        cmd.Parameters.AddWithValue("@FOC", _ItemListFoc[i].VQty);
                        SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                        returnValue.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(returnValue);
                        cmd.ExecuteNonQuery();
                        messagereturn = returnValue.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
            }
            return Json(messagereturn, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSpcRemark()
        {
            string message = string.Empty;
            string respone = "Y";
            var list = new List<object>();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("P_Get_SpcRemark", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new
                                {
                                    ID = reader["Lookup ID"] != DBNull.Value ? reader["Lookup ID"].ToString() : string.Empty,
                                    VAL = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty
                                };

                                list.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                respone = "N";
            }
            return Json(new { message = message, respone = respone, result = list }, JsonRequestBehavior.AllowGet);
        }
    }
}
