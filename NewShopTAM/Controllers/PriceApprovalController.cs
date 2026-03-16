using NewShopTAM.Controllers;
using NewShopTAM.Filters;
using NewShopTAM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace NewShopTAM.Controllers
{
    public class PriceApprovalController : Controller
    {
        //
        // GET: /PriceApproval/
        // [SystemAuthorize]
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
                    if (this.Session["UserType"] == "2")
                    {
                        return RedirectToAction("dashboard", "SeleScrCustomer");
                    }
                    string usre = Session["UserID"].ToString();
                    List<SLM> SlmList = new List<SLM>();
                    List<SelectListItem> GroupStkGrp = new List<SelectListItem>();
                    List<SelectListItem> PRODList = new List<SelectListItem>();
                    using (SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
                    {
                        Connection.Open();

                        var command = new SqlCommand("P_Price_Approve_Data", Connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@inUsrID", usre);
                        command.Parameters.AddWithValue("@inType", 2);
                        // command.ExecuteNonQuery();
                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            SlmList.Add(new SLM()
                            {
                                SLMCOD = dr["SLMCOD"].ToString(),
                                SLMNAM = dr["SLMNAM"].ToString()
                            });
                        }
                        ViewBag.SlmModel = SlmList;
                        //dr.Dispose();
                        //S20161016
                        dr.Close();
                        dr.Dispose();
                        command.Dispose();
                        //E20161016

                        command = new SqlCommand("P_Price_Approve_Data", Connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@inUsrID", usre);
                        command.Parameters.AddWithValue("@inType", 3);
                        //command.ExecuteNonQuery();
                        SqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            GroupStkGrp.Add(new SelectListItem() { Value = dr2["STKGRP"].ToString(), Text = dr2["STKGRP"].ToString() + "/" + dr2["GRPNAM"].ToString() });

                        }
                        ViewBag.StkGrp = GroupStkGrp;
                        //dr2.Dispose();
                        //S20161016
                        dr2.Close();
                        dr2.Dispose();
                        command.Dispose();
                        //E20161016

                        command = new SqlCommand("P_Price_Approve_Data", Connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@inUsrID", usre);
                        command.Parameters.AddWithValue("@inType", 4);
                        //command.ExecuteNonQuery();
                        SqlDataReader dr3 = command.ExecuteReader();
                        while (dr3.Read())
                        {
                            PRODList.Add(new SelectListItem() { Value = dr3["PROD"].ToString(), Text = dr3["PROD"].ToString() + "/" + dr3["PRODNAM"].ToString() });

                        }
                        ViewBag.PRODList = PRODList;
                        //dr3.Dispose();
                        //S20161016
                        dr3.Close();
                        dr3.Dispose();
                        command.Dispose();
                        //E20161016
                        Connection.Close();
                    }

                }
            }
            return View();
        }


        public JsonResult GetdataCus(string SLM, string SLMNAME)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<CUS> CUSList = new List<CUS>();
            SqlCommand cmd;
            if (SLM != "")
            {
                cmd = new SqlCommand("select * from v_CUSPROV where SLMCOD =N'" + SLM + "'", Connection);
            }
            else
            {
                cmd = new SqlCommand("select * from v_CUSPROV", Connection);
            }
            //else
            //{
            // SqlCommand cmd = new SqlCommand("select * from v_CUSPROV ", Connection);
            //}
            //  this.Session["SLM"] = SLM;
            //  this.Session["SLMCOD"] = SLMNAME;
            SqlDataReader rev_CUSPROV = cmd.ExecuteReader();
            while (rev_CUSPROV.Read())
            {
                CUSList.Add(new CUS()
                {
                    CUSCOD = rev_CUSPROV.GetValue(1).ToString(),
                    CUSNAM = rev_CUSPROV.GetValue(2).ToString()
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
        public JsonResult GetSTKGRP(string ProdMRG, string ProdMRG_NAME)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<STKGRPList> STKGRPList = new List<STKGRPList>();

            SqlCommand cmd = new SqlCommand("P_Price_Approve_Data  @inType=5,@inProd =N'" + ProdMRG + "'", Connection);

            SqlDataReader rev_CUSPROV = cmd.ExecuteReader();
            while (rev_CUSPROV.Read())
            {
                STKGRPList.Add(new STKGRPList()
                {
                    STKGRP = rev_CUSPROV["STKGRP"].ToString(),
                    GRPNAM = rev_CUSPROV["GRPNAM"].ToString()
                });
            }
            //rev_CUSPROV.Dispose();
            //S20161016
            rev_CUSPROV.Close();
            rev_CUSPROV.Dispose();
            cmd.Dispose();
            //E20161016
            Connection.Close();
            return Json(STKGRPList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetPriceApprove_Group(string CartID)
        {
            int sumQty = 0;
            int sumSalePrice = 0;
            //int sumDiscount = 0;
            List<ItemList_PriceApprove> Getdata = new List<ItemList_PriceApprove>();
            ItemPriceApprove model = null;

            string usre = Session["UserID"].ToString();


            using (SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
            {
                Connection.Open();
                var command = new SqlCommand("P_Search_Approval_Group_catalog", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCartID", CartID);

                //command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    model = new ItemPriceApprove();
                    DateTime date = Convert.ToDateTime(dr["ORDDAT"].ToString());
                    string formatted = date.ToString("dd/M/yyyy");
                    string Ldate = dr["LastInvdate"].ToString();
                    if (Ldate != "")
                    {
                        DateTime dateLastInvdate = Convert.ToDateTime(dr["LastInvdate"].ToString());
                        string formattedLastInvdate = dateLastInvdate.ToString("dd/M/yyyy");
                        model.LastInvdate = formattedLastInvdate;
                    }
                    else
                    {
                        model.LastInvdate = "-";

                    }
                    model.RowNo = dr["RowNo"].ToString();

                    model.SODate = dr["SODate"].ToString();
                    model.SONumber = dr["SONumber"].ToString();

                    model.ORDDAT = dr["ORDDAT"].ToString();
                    model.StatusFull = dr["StatusFull"].ToString();
                    model.PrcApproveDate = dr["PrcApproveDate"].ToString();
                    model.ID = dr["ID"].ToString();
                    model.PRCLST_NO = dr["PRCLST_NO"].ToString();
                    //    model.Status = dr["Status"].ToString();
                    model.CUSCOD = dr["CUSNAM"].ToString();
                    model.SLMID = dr["SLMNAM"].ToString();

                    model.ORDDAT = formatted;
                    model.STKCOD = dr["STKCOD"].ToString();
                    model.STKGRP = dr["STKGRP"].ToString();
                    model.STKDES = dr["STKDES"].ToString();
                    model.MINORD = dr["MINORD"].ToString();
                    model.Price = dr["Price"].ToString();
                    model.SalePrice = dr["SalePrice"].ToString();
                    model.SpecialPrice = dr["SpecialPrice"].ToString();
                    model.spcmoq = dr["spc_moq"].ToString();
                    // model.spcstart_date = dr["spc_start_date"].ToString();
                    // model.spcend_date = dr["spc_end_date"].ToString();
                    string spc_start_date = dr["spc_start_date"].ToString();
                    if (spc_start_date != "")
                    {
                        DateTime spc_s_date = Convert.ToDateTime(dr["spc_start_date"].ToString());
                        string formatspc_s_date = spc_s_date.ToString("dd/MM/yyyy");
                        model.spcstart_date = formatspc_s_date;
                    }
                    else
                    {
                        model.spcstart_date = dr["spc_start_date"].ToString();
                    }
                    //Model.spc_end_date = dr["spc_end_date"].ToString(); 	
                    string Ldate_end_dat = dr["spc_end_date"].ToString();
                    if (Ldate_end_dat != "")
                    {
                        DateTime dateend_dat = Convert.ToDateTime(dr["spc_end_date"].ToString());
                        string formatted_dateend_dat = dateend_dat.ToString("dd/MM/yyyy");
                        model.spcend_date = formatted_dateend_dat;

                    }
                    else
                    {
                        model.spcend_date = "-";
                    }
                    model.Qty = dr["Qty"].ToString();
                    model.Amt = dr["Amt"].ToString();
                    model.Discount = dr["Discount"].ToString();
                    model.Status = dr["Status"].ToString();
                    model.ExpectPrice = dr["ExpectPrice"].ToString();
                    //model.MINORD = dr["MINORD"].ToString();
                    model.UOM = dr["UOM"].ToString();
                    model.LastInvPrice = dr["LastInvPrice"].ToString();

                    model.Promotion = dr["Promotion"].ToString();
                    model.PromotionDesc = dr["PromotionDesc"].ToString();
                    // sumQty += Convert.ToInt32(dr.GetValue(15));
                    // sumSalePrice += Convert.ToInt32(dr.GetValue(9));
                    Getdata.Add(new ItemList_PriceApprove { val = model });

                }
                //S20161016
                dr.Close();
                dr.Dispose();
                command.Dispose();
                //E20161016
                Connection.Close();
            }

            return Json(new { Getdata, sumQty, sumSalePrice }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPriceApprove(string CUSCOD, string SLMCODE, string STKGRP, string ProdMRG, string vStatus, string topicType, string Usre)
        {
            int sumQty = 0;
            int sumSalePrice = 0;
            //  int sumDiscount = 0;
            List<ItemList_PriceApprove> Getdata = new List<ItemList_PriceApprove>();
            ItemPriceApprove model = null;

            string usre = Usre;


            if (ProdMRG != "0")
            {
                using (SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
                {
                    Connection.Open();
                    //var command = new SqlCommand("P_Search_Approval", Connection);
                    var command = new SqlCommand("P_Search_Approval_catalog", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inSLMCODE", SLMCODE);
                    command.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                    command.Parameters.AddWithValue("@inSTKGRP", STKGRP);
                    command.Parameters.AddWithValue("@inProd", ProdMRG);
                    command.Parameters.AddWithValue("@inStatus", vStatus);
                    command.Parameters.AddWithValue("@topicType", topicType);
                    command.Parameters.AddWithValue("@inUsrID", usre);
                    //command.ExecuteNonQuery();
                    SqlDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        model = new ItemPriceApprove();
                        DateTime date = Convert.ToDateTime(dr["ORDDAT"].ToString());
                        string formatted = date.ToString("dd/M/yyyy");
                        string Ldate = dr["LastInvdate"].ToString();
                        if (Ldate != "")
                        {
                            DateTime dateLastInvdate = Convert.ToDateTime(dr["LastInvdate"].ToString());
                            string formattedLastInvdate = dateLastInvdate.ToString("dd/M/yyyy");
                            model.LastInvdate = formattedLastInvdate;
                        }
                        else
                        {
                            model.LastInvdate = "-";

                        }
                        model.RowNo = dr["RowNo"].ToString();
                        model.ORDDAT = dr["ORDDAT"].ToString();
                        model.StatusFull = dr["StatusFull"].ToString();
                        model.PrcApproveDate = dr["PrcApproveDate"].ToString();
                        //model.spcstart_date = dr["spc_start_date"].ToString();
                        // model.spcend_date = dr["spc_end_date"].ToString();
                        string spc_start_date = dr["spc_start_date"].ToString();
                        if (spc_start_date != "")
                        {
                            DateTime spc_s_date = Convert.ToDateTime(dr["spc_start_date"].ToString());
                            string formatspc_s_date = spc_s_date.ToString("dd/MM/yyyy");
                            model.spcstart_date = formatspc_s_date;
                            if (vStatus == "N")
                            {
                                model.spcstart_date = dr["PrcApproveDate"].ToString();
                            }
                            else
                            {
                                model.spcstart_date = formatspc_s_date;
                            }
                        }
                        else
                        {
                            model.spcstart_date = dr["spc_start_date"].ToString();
                        }
                        //Model.spc_end_date = dr["spc_end_date"].ToString(); 	
                        string Ldate_end_dat = dr["spc_end_date"].ToString();
                        if (Ldate_end_dat != "")
                        {
                            DateTime dateend_dat = Convert.ToDateTime(dr["spc_end_date"].ToString());
                            string formatted_dateend_dat = dateend_dat.ToString("dd/MM/yyyy");
                            model.spcend_date = formatted_dateend_dat;


                        }
                        else
                        {
                            model.spcend_date = "-";

                        }
                        // model.PrcApproveDate = dr["PrcApproveDate"].ToString();
                        model.ID = dr["ID"].ToString();
                        model.PRCLST_NO = dr["PRCLST_NO"].ToString();
                        //    model.Status = dr["Status"].ToString();
                        model.CUSCOD = dr["CUSNAM"].ToString();
                        model.SLMID = dr["SLMNAM"].ToString();

                        model.ORDDAT = formatted;
                        model.STKCOD = dr["STKCOD"].ToString();
                        model.STKGRP = dr["STKGRP"].ToString();
                        model.STKDES = dr["STKDES"].ToString();
                        model.MINORD = dr["MINORD"].ToString();
                        model.Price = dr["Price"].ToString();
                        model.SalePrice = dr["SalePrice"].ToString();
                        model.SpecialPrice = dr["SpecialPrice"].ToString();
                        model.Spc_Remark = dr["Spc_Remark"] != DBNull.Value ? dr["Spc_Remark"].ToString() : string.Empty;
                        model.spcmoq = dr["spc_moq"].ToString();
                        model.Qty = dr["Qty"].ToString();
                        model.Amt = dr["Amt"].ToString();
                        model.Discount = dr["Discount"].ToString();
                        model.Status = dr["Status"].ToString();
                        model.ExpectPrice = dr["ExpectPrice"].ToString();

                        model.UOM = dr["UOM"].ToString();
                        model.LastInvPrice = dr["LastInvPrice"].ToString();

                        model.Promotion = dr["Promotion"].ToString();
                        model.PromotionDesc = dr["PromotionDesc"].ToString();
                        model.Prcdes = dr["PRCDES"].ToString();
                        model.ID = dr["ID"].ToString();
                        model.InsertedBy = dr["Inserted By"].ToString();
                        model.InsertedDate = dr["Inserted Date"].ToString();
                        model.Item_Type = dr["Item_Type"].ToString();
                        model.Item_Type_Des = dr["Item_Type_Des"].ToString();
                        model.LineNote = dr["LineNote"].ToString();
                        model.UNITCOST = dr["UNITCOST"].ToString();
                        model.readyQty = dr["readyQty"].ToString();
                        model.ItemType = dr["Item_Type"].ToString();
                        model.ItemTypeDes = dr["Item_Type_Des"].ToString();
                        model.ProdCodeHigh = dr["ProdCodeHigh"].ToString();
                        model.PriceApproveId = dr["PriceApproveId"].ToString();
                        model.BckComment = dr["BckComment"].ToString();
                        // sumQty += Convert.ToInt32(dr.GetValue(15));
                        // sumSalePrice += Convert.ToInt32(dr.GetValue(9));
                        Getdata.Add(new ItemList_PriceApprove { val = model });

                    }
                    //S20161016
                    dr.Close();
                    dr.Dispose();
                    command.Dispose();
                    //E20161016
                    //Connection.Dispose();
                    Connection.Close();
                }

            }
            else
            {

                using (SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
                {
                    Connection.Open();
                    //var command = new SqlCommand("P_Search_Approval", Connection);
                    var command = new SqlCommand("P_Search_Approval_catalog", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inSLMCODE", SLMCODE);
                    command.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                    command.Parameters.AddWithValue("@inSTKGRP", STKGRP);
                    command.Parameters.AddWithValue("@inProd", "");
                    command.Parameters.AddWithValue("@inStatus", vStatus);
                    command.Parameters.AddWithValue("@inUsrID", usre);
                    //command.ExecuteNonQuery();
                    SqlDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        model = new ItemPriceApprove();
                        DateTime date = Convert.ToDateTime(dr["ORDDAT"].ToString());
                        string formatted = date.ToString("dd/M/yyyy");
                        string Ldate = dr["LastInvdate"].ToString();
                        if (Ldate != "")
                        {
                            DateTime dateLastInvdate = Convert.ToDateTime(dr["LastInvdate"].ToString());
                            string formattedLastInvdate = dateLastInvdate.ToString("dd/M/yyyy");
                            model.LastInvdate = formattedLastInvdate;
                        }
                        else
                        {
                            model.LastInvdate = "-";

                        }
                        model.RowNo = dr["RowNo"].ToString();
                        model.ORDDAT = dr["ORDDAT"].ToString();
                        model.StatusFull = dr["StatusFull"].ToString();
                        model.PrcApproveDate = dr["PrcApproveDate"].ToString();
                        model.ID = dr["ID"].ToString();
                        model.PRCLST_NO = dr["PRCLST_NO"].ToString();
                        //    model.Status = dr["Status"].ToString();
                        model.CUSCOD = dr["CUSNAM"].ToString();
                        model.SLMID = dr["SLMNAM"].ToString();

                        model.ORDDAT = formatted;
                        model.STKCOD = dr["STKCOD"].ToString();
                        model.STKGRP = dr["STKGRP"].ToString();
                        model.STKDES = dr["STKDES"].ToString();
                        model.MINORD = dr["MINORD"].ToString();
                        model.Price = dr["Price"].ToString();
                        model.SalePrice = dr["SalePrice"].ToString();
                        model.SpecialPrice = dr["SpecialPrice"].ToString();
                        model.MINORD = dr["MINORD"].ToString();
                        model.spcmoq = dr["spc_moq"].ToString();
                        model.spcstart_date = dr["spc_start_date"].ToString();
                        model.spcend_date = dr["spc_end_date"].ToString();
                        string spc_start_date = dr["spc_start_date"].ToString();
                        if (spc_start_date != "")
                        {
                            DateTime spc_s_date = Convert.ToDateTime(dr["spc_start_date"].ToString());
                            string formatspc_s_date = spc_s_date.ToString("dd/MM/yyyy");
                            model.spcstart_date = formatspc_s_date;
                            if (vStatus == "N")
                            {
                                model.spcstart_date = dr["PrcApproveDate"].ToString();
                            }
                            else
                            {
                                model.spcstart_date = formatspc_s_date;
                            }
                        }
                        else
                        {
                            model.spcstart_date = dr["spc_start_date"].ToString();
                        }
                        //Model.spc_end_date = dr["spc_end_date"].ToString(); 	
                        string Ldate_end_dat = dr["spc_end_date"].ToString();
                        if (Ldate_end_dat != "")
                        {
                            DateTime dateend_dat = Convert.ToDateTime(dr["spc_end_date"].ToString());
                            string formatted_dateend_dat = dateend_dat.ToString("dd/MM/yyyy");
                            model.spcend_date = formatted_dateend_dat;


                        }
                        else
                        {
                            model.spcend_date = "-";

                        }
                        model.Qty = dr["Qty"].ToString();
                        model.Amt = dr["Amt"].ToString();
                        model.Discount = dr["Discount"].ToString();
                        model.Status = dr["Status"].ToString();
                        model.ExpectPrice = dr["ExpectPrice"].ToString();

                        model.UOM = dr["UOM"].ToString();
                        model.LastInvPrice = dr["LastInvPrice"].ToString();

                        model.Promotion = dr["Promotion"].ToString();
                        model.PromotionDesc = dr["PromotionDesc"].ToString();
                        model.Prcdes = dr["PRCDES"].ToString();
                        model.ID = dr["ID"].ToString();
                        model.InsertedBy = dr["Inserted By"].ToString();
                        model.InsertedDate = dr["Inserted Date"].ToString();
                        model.LineNote = dr["LineNote"].ToString();
                        model.UNITCOST = dr["UNITCOST"].ToString();
                        model.ItemTypeDes = dr["Item_Type_Des"].ToString();
                        model.ProdCodeHigh = dr["ProdCodeHigh"].ToString();
                        // sumQty += Convert.ToInt32(dr.GetValue(15));
                        // sumSalePrice += Convert.ToInt32(dr.GetValue(9));
                        Getdata.Add(new ItemList_PriceApprove { val = model });

                    }
                    //S20161016
                    dr.Close();
                    dr.Dispose();
                    command.Dispose();
                    //E20161016
                    Connection.Close();
                }

            }

            return Json(new { Getdata, sumQty, sumSalePrice }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDetailPriceLowCost(string cartId)
        {
            string message = "Y";
            string userId = Session["UserID"].ToString();
            string isStep = "";
            string isProdCode = "";
            string isHighLevel = "";
            string isProdCodeHigh = "";
            string isRemarkSelf = "";

            List<listDetailPriceLowCost> detailPriceLowCostList = new List<listDetailPriceLowCost>();

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Get_Detail_PriceLowCost", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCartId", cartId);
                command.Parameters.AddWithValue("@inUserId", userId);
                SqlParameter returnStep = new SqlParameter("@isStep", SqlDbType.NVarChar, 1000);
                SqlParameter returnProdCode = new SqlParameter("@isProdCode", SqlDbType.NVarChar, 1000);
                SqlParameter returnHighLeve = new SqlParameter("@isHighLevel", SqlDbType.NVarChar, 1000);
                SqlParameter returndProdCodeHigh = new SqlParameter("@isProdCodeHigh", SqlDbType.NVarChar, 1000);
                SqlParameter returndRemarkSelf = new SqlParameter("@isRemarkSelf", SqlDbType.NVarChar, 1000);
                returnStep.Direction = ParameterDirection.Output;
                returnProdCode.Direction = ParameterDirection.Output;
                returnHighLeve.Direction = ParameterDirection.Output;
                returndProdCodeHigh.Direction = ParameterDirection.Output;
                returndRemarkSelf.Direction = ParameterDirection.Output;
                command.Parameters.Add(returnStep);
                command.Parameters.Add(returnProdCode);
                command.Parameters.Add(returnHighLeve);
                command.Parameters.Add(returndProdCodeHigh);
                command.Parameters.Add(returndRemarkSelf);
                int outputResult = command.ExecuteNonQuery();
                isStep = command.Parameters["@isStep"].Value.ToString();
                isProdCode = command.Parameters["@isProdCode"].Value.ToString();
                isHighLevel = command.Parameters["@isHighLevel"].Value.ToString();
                isProdCodeHigh = command.Parameters["@isProdCodeHigh"].Value.ToString();
                isRemarkSelf = command.Parameters["@isRemarkSelf"].Value.ToString();

                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    detailPriceLowCostList.Add(new listDetailPriceLowCost()
                    {
                        CartId = dr["PriceApproveId"].ToString(),
                        Cuscode = dr["CUSCOD"].ToString(),
                        CusnameFull = dr["CUSNAM_FULL"].ToString(),

                        Slmcod = dr["SLMCODE"].ToString(),
                        SlmcodFull = dr["SLMNAM_FULL"].ToString(),

                        Stkcod = dr["STKCOD_FULL"].ToString(),

                        Qty = dr["Qty"].ToString(),
                        ExpectPrice = dr["ExpectPrice"].ToString(),
                        Cost = dr["Cost"].ToString(),

                        ProdCode_Step1 = dr["ProdCode_Step1"].ToString(),
                        ApproveDate_Step1 = dr["ApproveDate_Step1"].ToString(),
                        ApproveBy_Step1 = dr["ApproveBy_Step1"].ToString(),
                        Remark_Step1 = dr["Remark_Step1"].ToString(),

                        ProdCode_Step2 = dr["ProdCode_Step2"].ToString(),
                        ApproveDate_Step2 = dr["ApproveDate_Step2"].ToString(),
                        ApproveBy_Step2 = dr["ApproveBy_Step2"].ToString(),
                        Remark_Step2 = dr["Remark_Step2"].ToString(),

                        ProdCode_Step3 = dr["ProdCode_Step3"].ToString(),
                        ApproveDate_Step3 = dr["ApproveDate_Step3"].ToString(),
                        ApproveBy_Step3 = dr["ApproveBy_Step3"].ToString(),
                        Remark_Step3 = dr["Remark_Step3"].ToString()
                    });
                }
                @ViewBag.listDetailPriceLowCost = detailPriceLowCostList;
                @ViewBag.isStep = isStep;
                @ViewBag.isProdCode = isProdCode;
                @ViewBag.isHighLevel = isHighLevel;
                @ViewBag.isProdCodeHigh = isProdCodeHigh;
                @ViewBag.isRemarkSelf = isRemarkSelf;

                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            ViewBag.Message = message;
            return PartialView("_PriceLowCostDetail", new
            {
                @ViewBag.Message,
                @ViewBag.listDetailPriceLowCost,
                @ViewBag.isStep,
                @ViewBag.isProdCode,
                @ViewBag.isHighLevel,
                @ViewBag.isProdCodeHigh,
                @ViewBag.isRemarkSelf,
            });
        }
        public JsonResult SaveApprovePriceLowCost(string cartId, string step, string remark, string approveStatus)
        {
            string message = string.Empty;
            string userId = Session["UserID"].ToString();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            try
            {
                var cmd = new SqlCommand("P_Save_Approve_PriceLowCost", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inCartId", cartId.Trim());
                cmd.Parameters.AddWithValue("@inUserId", userId);
                cmd.Parameters.AddWithValue("@inStep", step);
                cmd.Parameters.AddWithValue("@inApproveStatus", approveStatus);
                cmd.Parameters.AddWithValue("@inRemark", remark);
                SqlParameter p = new SqlParameter("@OutGenstatus", SqlDbType.NVarChar, 100);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);
                cmd.ExecuteNonQuery();
                message = cmd.Parameters["@OutGenstatus"].Value.ToString();
                conn.Close();
                cmd.Dispose();

                if (approveStatus == "R" || approveStatus == "Y")
                {
                    try
                    {
                        if (approveStatus == "Y")
                        {
                            approveStatus = "N";
                        }
                        else
                        {
                            approveStatus = "C";
                        }
                        //var CallSendAPI = SendNotificateLine();
                        GetApprvOrderPM(cartId.Trim(), approveStatus);
                    }
                    catch { }
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                conn.Close();
            }
            return Json(new { message = message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Approvalndata(string data, string User, string vStatus)
        {
            string message = "false";
            string messagereturnsql = string.Empty;
            string messageerror = string.Empty;
            string Subject = string.Empty;
            string Body = string.Empty;
            List<Itemapproval> _ItemList = new JavaScriptSerializer().Deserialize<List<Itemapproval>>(data);
            int cid = 0;
            int Smoq = 0;
            string spcEndDate = string.Empty;
            string strspcend = string.Empty;
            string strSmoq = string.Empty;
            string usre = Session["UserID"].ToString();
            string strend = string.Empty;
            string stremake = string.Empty;
            string messagereturn = string.Empty;
            try
            {
                if (_ItemList.Count > 0)
                {

                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
                    {
                        connection.Open();
                        for (int i = 0; i < _ItemList.Count; i++)
                        {
                            cid = Convert.ToInt32(_ItemList[i].cartid);
                            strSmoq = _ItemList[i].spcmoq;
                            if (strSmoq != "")
                            {
                                Smoq = Convert.ToInt32(_ItemList[i].spcmoq);
                            }
                            strspcend = _ItemList[i].spcend_date;
                            if (strspcend != "")
                            {
                                //SpcEndDate = DateTime.ParseExact(strspcend, "dd/MM/yyyy", null);
                                spcEndDate = Convert.ToDateTime(strspcend.ToString()).ToString("dd/MM/yyyy");
                                //formatspc_s_date = SpcEndDate.ToString("dd/MM/yyyy");

                                //convert.todatetime(_itemlist[i].spcend_date);
                                //spcenddate = _itemlist[i].spcend_date;
                            }
                            stremake = _ItemList[i].remake;
                            //SqlCommand command = new SqlCommand("P_Save_Price_Approve", connection);
                            SqlCommand command = new SqlCommand("P_Save_Price_Approve_catalog", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            if (vStatus != "C")
                            {

                                command.Parameters.AddWithValue("@inORDID", cid);
                                command.Parameters.AddWithValue("@inUser", usre);
                                command.Parameters.AddWithValue("@inStatus", vStatus);
                                command.Parameters.AddWithValue("@inSpcmoq", Smoq);
                                command.Parameters.AddWithValue("@inSpcEnddate", spcEndDate);
                                command.Parameters.AddWithValue("@inRemake", stremake);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@inORDID", cid);
                                command.Parameters.AddWithValue("@inUser", usre);
                                command.Parameters.AddWithValue("@inStatus", vStatus);
                                command.Parameters.AddWithValue("@inSpcmoq", 1);
                                command.Parameters.AddWithValue("@inSpcEnddate", DateTime.Now);
                                command.Parameters.AddWithValue("@inRemake", stremake);
                            }
                            SqlParameter returnValue = new SqlParameter("@outResult", SqlDbType.NVarChar, 100);
                            //  returnValue.Direction = System.Data.ParameterDirection.Output;
                            //  command.Parameters.Add(returnValue); 
                            returnValue.Direction = System.Data.ParameterDirection.Output;
                            command.Parameters.Add(returnValue);


                            command.ExecuteNonQuery();

                            messagereturnsql = returnValue.Value.ToString();
                            //S20161016
                            command.Dispose();
                            //E20161016
                            //if (messagereturnsql == "N")
                            //{


                            //}
                            try
                            {
                                //var CallSendAPI = SendNotificateLine();
                                GetApprvOrderPM(cid.ToString(), vStatus);
                            }
                            catch { }


                        }
                        connection.Close();

                    }
                }
                message = "true";
            }
            catch (Exception ex)
            {
                message = "false";
                messageerror = ex.Message;

            }
            return Json(new { message, messageerror }, JsonRequestBehavior.AllowGet);
            //return Json(new { message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Cancalpriceapproval(string CartID, string User)
        {
            int id = 0;
            bool message = false;
            try
            {
                //id = Convert.ToInt32(CartID);
                //Ordering_Cart Update = db.Ordering_Cart.First(c => c.ID == id);
                //Update.Status = "C";

                //Update.Updated_Date = DateTime.Now;
                //Update.Updated_By = User;
                //db.SaveChanges();

                message = true;
            }
            catch (Exception ex)
            {
                message = false;
            }
            return Json(new { message }, JsonRequestBehavior.AllowGet);

        }
        //public async Task<string> SendNotificateLine(string Uid, string Toppic, string ToppicCo, string stkcod, string stkdes, string Price, string Qty, string cuscod, string cusnam, string apprvby, string apprvdat, string user )
        public async Task<string> SendNotificateLine(ListPMNotificate list)
        {
            var urlAPI = "https://mst.aac.co.th/APIService/Post/PushMessageSale";
            var post = list;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                // var handler = new HttpClientHandler();
                var handler = new HttpClientHandler();
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                var client = new HttpClient(handler);
                string jsonContent = JsonConvert.SerializeObject(post);
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(urlAPI, content).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch (Exception ex) { return ex.Message; }

        }

        public void GetApprvOrderPM(string ordID, string ordSta)
        {
            List<ListPMNotificate> list = new List<ListPMNotificate>();
            string usre = Session["UserID"].ToString();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("P_GetOrderPMApprove", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inORDID", ordID);
                    cmd.Parameters.AddWithValue("@inStatus", ordSta);
                    cmd.Parameters.AddWithValue("@inUser", usre);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new ListPMNotificate()
                        {
                            Uid = reader["LineID"] != DBNull.Value ? reader["LineID"].ToString() : string.Empty,
                            Topiccod = reader["Topiccod"] != DBNull.Value ? reader["Topiccod"].ToString() : string.Empty,
                            Topic = reader["Topic"] != DBNull.Value ? reader["Topic"].ToString() : string.Empty,
                            Cuscod = reader["CUSCOD"] != DBNull.Value ? reader["CUSCOD"].ToString() : string.Empty,
                            Cusname = reader["CUSNAM"] != DBNull.Value ? reader["CUSNAM"].ToString() : string.Empty,
                            Stkcod = reader["STKCOD"] != DBNull.Value ? reader["STKCOD"].ToString() : string.Empty,
                            Stkdes = reader["STKDES"] != DBNull.Value ? reader["STKDES"].ToString() : string.Empty,
                            Qty = reader["QTY"] != DBNull.Value ? reader["QTY"].ToString() : string.Empty,
                            SPrice = reader["SPrice"] != DBNull.Value ? reader["SPrice"].ToString() : string.Empty,
                            ApprvBy = reader["ApprvBy"] != DBNull.Value ? reader["ApprvBy"].ToString() : string.Empty,
                            ApprvDate = reader["ApprvDate"] != DBNull.Value ? reader["ApprvDate"].ToString() : string.Empty,
                            User = usre,
                            Sta = ordSta
                        });
                    }

                    // reader.Close();
                    conn.Close();

                    if (list.Count() > 0)
                    {
                        //foreach (var item in list)
                        //{
                        //    if (item != null)
                        //    {
                        //        var respon = SendNotificateLine(item);
                        //    }
                        //} Big O n^2
                        //var respone = SendNotificateLine(list.First());
                        foreach (var item in list)
                        {
                            SendNotificateLine(item);
                        }
                    }
                }
            }
            catch { }
        }

        public static string AddCommaIfNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Trim();

            var normalized = input.Replace(",", "");


            bool isNumber = decimal.TryParse(
                normalized,
                NumberStyles.AllowLeadingSign |
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal value
            );

            if (!isNumber)
                return input; // ไม่ใช่เลขจริง → ส่งคืนค่าเดิม

            return value.ToString("#,##0.##", CultureInfo.InvariantCulture);
        }
    }
    public class STKGRPList
    {
        public string STKGRP { get; set; }
        public string GRPNAM { get; set; }
    }

    public class Itemapproval
    {
        public string cartid { get; set; }
        public string PRCLST_NO { get; set; }
        public string spcend_date { get; set; }
        public string spcmoq { get; set; }
        public string remake { get; set; }
        public string saleprice { get; set; }
    }
    public class ItemList_PriceApprove
    {
        public ItemPriceApprove val { get; set; }

    }
    public class ItemPriceApprove
    {
        public string SONumber { get; set; }
        public string SODate { get; set; }
        public string RowNo { get; set; }
        public string ID { get; set; }
        public string PRCLST_NO { get; set; }
        public string CUSCOD { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string STKGRP { get; set; }
        public string MINORD { get; set; }
        public string Price { get; set; }
        public string SalePrice { get; set; }
        public string spcmoq { get; set; }
        public string SpecialPrice { get; set; }
        public string Spc_Remark { get; set; }
        public string spcstart_date { get; set; }
        public string spcend_date { get; set; }
        public string ExpectPrice { get; set; }
        public string Qty { get; set; }
        public string User { get; set; }
        public string Amt { get; set; }
        public string ORDDAT { get; set; }
        public string Item_Type { get; set; }
        public string Item_Type_Des { get; set; }
        public string LineNote { get; set; }
        public string Status { get; set; }
        public string StatusFull { get; set; }
        public string Discount { get; set; }
        public int QtyAmt { get; set; }
        public string AmtQty { get; set; }
        public string AmtSalePrices { get; set; }
        public string TotalAmt { get; set; }
        public string amtCredit { get; set; }
        public string AmtDiscount { get; set; }
        public string UOM { get; set; }
        public string SLMID { get; set; }
        public string PromotionDesc { get; set; }
        public string Promotion { get; set; }
        public string LastInvdate { get; set; }
        public string LastInvPrice { get; set; }
        public string PrcApproveDate { get; set; }
        public string InsertedDate { get; set; }
        public string InsertedBy { get; set; }
        public string Prcdes { get; set; }
        public string UNITCOST { get; set; }
        public string readyQty { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeDes { get; set; }
        public string ProdCodeHigh { get; set; }
        public string PriceApproveId { get; set; }
        public string BckComment { get; set; }

    }

    public class listDetailPriceLowCost
    {
        public string CartId { get; set; }
        public string Stkcod { get; set; }
        public string Cusname { get; set; }
        public string Cuscode { get; set; }
        public string CusnameFull { get; set; }
        public string Slmcod { get; set; }
        public string SlmcodFull { get; set; }
        public string Qty { get; set; }
        public string ExpectPrice { get; set; }
        public string Cost { get; set; }
        public string ProdCode_Step1 { get; set; }
        public string ApproveDate_Step1 { get; set; }
        public string ApproveBy_Step1 { get; set; }
        public string Remark_Step1 { get; set; }
        public string ProdCode_Step2 { get; set; }
        public string ApproveDate_Step2 { get; set; }
        public string ApproveBy_Step2 { get; set; }
        public string Remark_Step2 { get; set; }
        public string ProdCode_Step3 { get; set; }
        public string ApproveDate_Step3 { get; set; }
        public string ApproveBy_Step3 { get; set; }
        public string Remark_Step3 { get; set; }
    }
}
