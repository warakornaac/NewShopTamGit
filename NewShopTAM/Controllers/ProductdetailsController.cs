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
    public class ProductdetailsController : Controller
    {
        //
        // GET: /Product-details/

        public ActionResult Index()
        {
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            else
            {
                string Docdisplay = string.Empty;
                string Nodisplay = string.Empty;
                string Comdisplay = string.Empty;

                Docdisplay = Request.QueryString["prodenum"];
                if (Docdisplay != null)
                {
                    string[] words = Docdisplay.Split('/');
                    Nodisplay = words[0];
                    byte[] data = System.Convert.FromBase64String(Nodisplay);
                    Nodisplay = System.Text.ASCIIEncoding.ASCII.GetString(data);

                    Comdisplay = words[1];
                    byte[] datasub = System.Convert.FromBase64String(Comdisplay);
                    Comdisplay = System.Text.ASCIIEncoding.ASCII.GetString(datasub);


                    // Doc = Docdisplay;
                    // Docsub = Docdisplay;
                }

                ViewBag.Nodisplay = Nodisplay;
                ViewBag.com = Comdisplay;
            }
            return View();
        }
        public JsonResult Getdataitemdetail(string Nodisplay, string Comdisplay, string strcustome)
        {
            string com = string.Empty;
            string substkgrp = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            // List<SearchitemDetailGetdata> Getdata = new List<SearchitemDetailGetdata>();
            //Searchitem model = null;
            PricelistpageingSearch Model = null;
            List<ListPagedList> Getdata = new List<ListPagedList>();
            //var Getdata = new List<object>();
            try
            {
                //var command = new SqlCommand("P_Search_Item_byVehicle_Detail", Connection);
                //command.CommandType = CommandType.StoredProcedure;
                //command.Parameters.AddWithValue("@inSearch", Nodisplay);
                //command.Parameters.AddWithValue("@Company", Comdisplay);
                //command.ExecuteNonQuery();
                var root = @"..\IMAGE_B\";
                var command = new SqlCommand("P_Search_Pricelist", Connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@SaleCode", "");
                command.Parameters.AddWithValue("@User", "");

                command.Parameters.AddWithValue("@Customer", strcustome);
                command.Parameters.AddWithValue("@Prod", "(ALL)");
                command.Parameters.AddWithValue("@StockGroup", "(ALL)");
                command.Parameters.AddWithValue("@StockCode", Nodisplay);
                command.Parameters.AddWithValue("@Company", Comdisplay);
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
                    //Model.FullDescription = dr.(4).ToString();
                    substkgrp = dr["STKGRP_PRC"].ToString();
                    Model.STKGRP_PRC = substkgrp.Substring(0, 2);
                    Model.minord = dr["minord"].ToString();
                    Model.Promotion = dr["PromoPrice"].ToString();
                    Model.ItemClass = dr["ItemClass"].ToString();

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
                    //string spcp = dr["spc_moq"].ToString();
                    // if (spcp =="0"){ Model.spc_moq = "0.00"}else{ Model.spc_moq =spcp;}
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
                    //Model.spc_end_date = dr["spc_end_date"].ToString(); 	
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
                    // Model.spc_start_date	 = dr["spc_start_date"].ToString(); 	

                    Model.spc_remark = dr["spc_remark"].ToString();
                    Model.PromotionCode = dr["PromotionCode"].ToString();
                    Model.PromoDesc = dr["PromoDesc"].ToString();
                    Model.PromoPrice = dr["PromoPrice"].ToString();
                    Model.PromoMOQ = dr["PromoMOQ"].ToString();
                    Model.PF = dr["PF"].ToString();
                    Model.Rack = dr["Rack"].ToString();
                    Model.SPackUOM = dr["SPackUOM"].ToString();
                    Model.PATH = Path.Combine(root, dr["IMAGE_NAME"].ToString());
                    Model.Expected_Receipt_Date = dr["Expected Receipt Date"].ToString();
                    Model.maxord = dr["maxord"].ToString();
                    //Model.expired = dr["expired"].ToString();
                    // Model.itemblock = dr["itemblock"].ToString();
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
        public JsonResult GetdataitemdetailbyMoq(string Nodisplay, string Comdisplay, string strcustome, string moq)
        {
            string com = string.Empty;
            string substkgrp = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            // List<SearchitemDetailGetdata> Getdata = new List<SearchitemDetailGetdata>();
            //Searchitem model = null;
            PricelistpageingSearch Model = null;
            List<ListPagedList> Getdata = new List<ListPagedList>();
            //var Getdata = new List<object>();
            try
            {
                //var command = new SqlCommand("P_Search_Item_byVehicle_Detail", Connection);
                //command.CommandType = CommandType.StoredProcedure;
                //command.Parameters.AddWithValue("@inSearch", Nodisplay);
                //command.Parameters.AddWithValue("@Company", Comdisplay);
                //command.ExecuteNonQuery();
                var root = @"..\IMAGE_B\";
                var command = new SqlCommand("P_Search_Pricelist_By_Moq", Connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@SaleCode", "");
                command.Parameters.AddWithValue("@User", "");

                command.Parameters.AddWithValue("@Customer", strcustome);
                command.Parameters.AddWithValue("@Prod", "(ALL)");
                command.Parameters.AddWithValue("@StockGroup", "(ALL)");
                command.Parameters.AddWithValue("@StockCode", Nodisplay);
                command.Parameters.AddWithValue("@Company", Comdisplay);
                command.Parameters.AddWithValue("@Moq", moq);
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
                    //Model.FullDescription = dr.(4).ToString();
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
                    //string spcp = dr["spc_moq"].ToString();
                    // if (spcp =="0"){ Model.spc_moq = "0.00"}else{ Model.spc_moq =spcp;}
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
                    //Model.spc_end_date = dr["spc_end_date"].ToString(); 	
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
                    // Model.spc_start_date	 = dr["spc_start_date"].ToString(); 	

                    Model.spc_remark = dr["spc_remark"].ToString();
                    Model.PromotionCode = dr["PromotionCode"].ToString();
                    Model.PromoDesc = dr["PromoDesc"].ToString();
                    Model.PromoPrice = dr["PromoPrice"].ToString();
                    Model.PromoMOQ = dr["PromoMOQ"].ToString();
                    Model.PF = dr["PF"].ToString();
                    Model.Rack = dr["Rack"].ToString();
                    Model.SPackUOM = dr["SPackUOM"].ToString();
                    Model.PATH = Path.Combine(root, dr["IMAGE_NAME"].ToString());
                    Model.Expected_Receipt_Date = dr["Expected Receipt Date"].ToString();
                    //Model.expired = dr["expired"].ToString();
                    // Model.itemblock = dr["itemblock"].ToString();
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
        public JsonResult Confirmationdata(string customer, string stkcod, string company, string price, string sprice, string qty, string bckorder, string expectprice, string linenote, string insertedby, string pomocode, string foc, string minord, string prclstno, string specprice, string promoprice, string promodesc, string lastinvprice, string lastinvdate)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string messagereturn = string.Empty;
            SqlTransaction trans = null;
            try
            {
                SqlCommand cmd = new SqlCommand("p_SaveOrderCart", Connection);
                cmd.Connection = Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Customer", customer);
                cmd.Parameters.AddWithValue("@STKCOD", stkcod);
                cmd.Parameters.AddWithValue("@Company", company);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@SPrice", sprice);
                cmd.Parameters.AddWithValue("@Expect_Price", expectprice);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@Bckorder", bckorder);
                cmd.Parameters.AddWithValue("@InsertedBy", insertedby);
                cmd.Parameters.AddWithValue("@LineNote", linenote);
                cmd.Parameters.AddWithValue("@FOC", foc);
                cmd.Parameters.AddWithValue("@ProCode", pomocode);
                cmd.Parameters.AddWithValue("@minord", minord);
                cmd.Parameters.AddWithValue("@prclstno", prclstno);
                cmd.Parameters.AddWithValue("@specprice", specprice);
                cmd.Parameters.AddWithValue("@promoprice", promoprice);
                cmd.Parameters.AddWithValue("@promodesc", promodesc);
                cmd.Parameters.AddWithValue("@lastinvprice", lastinvprice);
                cmd.Parameters.AddWithValue("@lastinvdate", lastinvdate);
                cmd.Parameters.AddWithValue("@sop", "0");
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
