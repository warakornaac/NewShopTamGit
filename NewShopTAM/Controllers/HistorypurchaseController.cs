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
    public class HistorypurchaseController : Controller
    {
        //
        // GET: /Historypurchase/

        public ActionResult Index()
        {
            //this.Session["UserType"] = "";
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");

            }
            return View();
        }
        public JsonResult UserInsert(string usre)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<SelectListItem> GroupUserInsert = new List<SelectListItem>();
            SqlCommand cmduser = new SqlCommand("select * from  v_UserInsert where InsertBy=N'" + usre + "'", Connection);
            SqlDataReader rev_UserInsert = cmduser.ExecuteReader();
            while (rev_UserInsert.Read())
            {
                GroupUserInsert.Add(new SelectListItem()
                {

                    Value = rev_UserInsert.GetValue(0).ToString(),
                });
            }
            rev_UserInsert.Dispose();
            Connection.Close();
            return Json(new { GroupUserInsert }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrderhistory(string CUSCOD, string STKCOD, string SortBy)
        {
            List<HistoryorderList> Getdata = new List<HistoryorderList>();

            historyorder model = null;
         
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
            {
                connection.Open();


                var command = new SqlCommand("p_Search_Sales_CusItemsRepeat", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@CusCod", CUSCOD);
                command.Parameters.AddWithValue("@inSTKCOD",STKCOD);
                command.Parameters.AddWithValue("@SortBy", SortBy);
                //command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    model = new historyorder();
                    model.Company= dr["Company"].ToString();
                    model.CUSCOD= dr["CUSCOD"].ToString();
                    model.CUSNAM= dr["CUSNAM"].ToString();
                    model.CUSKEY= dr["CUSKEY"].ToString();
                    model.STKCOD= dr["STKCOD"].ToString();
                    model.STKDES= dr["STKDES"].ToString();
                    model.Qty= dr["Qty"].ToString();
                    model.Sp= dr["Sp"].ToString();
                    model.Amt= dr["Amt"].ToString();
                    model.Docdat= dr["Docdat"].ToString();
                    model.NoofOrder= dr["NoofOrder"].ToString();
                    model.TotalQty= dr["TotalQty"].ToString();
                    model.TotalAmt= dr["TotalAmt"].ToString();
                    model.Qty01= dr["Qty01"].ToString();
			        model.Qty02= dr["Qty02"].ToString();
			        model.Qty03= dr["Qty03"].ToString();
			        model.Qty04= dr["Qty04"].ToString();
			        model.Qty05= dr["Qty05"].ToString();
			        model.Qty06= dr["Qty06"].ToString();
			        model.Qty07= dr["Qty07"].ToString();
			        model.Qty08= dr["Qty08"].ToString();
			        model.Qty09= dr["Qty09"].ToString();
			        model.Qty10= dr["Qty10"].ToString();
			        model.Qty11= dr["Qty11"].ToString();
                    model.Qty12 = dr["Qty12"].ToString();
                    Getdata.Add(new HistoryorderList { val = model });

                }

                dr.Close();
                dr.Dispose();
                command.Dispose();
                //E20161016
                connection.Close();
            }
            return Json(new { Getdata}, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOrderDetail(string ORD_ID, string ORD_DocNo, string STKCOD, string DType)
        {
            List<OrderListDetailGetdata> Getdata = new List<OrderListDetailGetdata>();
            SaleOrderDetail model = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("P_Search_SaleOrderDetail", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@inORD_ID", ORD_ID);
                command.Parameters.AddWithValue("@inORD_DocNo", ORD_DocNo);
                command.Parameters.AddWithValue("@inSTKCOD", STKCOD);
                command.Parameters.AddWithValue("@inDType", DType);
                //command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    model = new SaleOrderDetail();

                    model.RowNo = dr["RowNo"].ToString();
                    model.VSTKCOD = dr["ORD_STKCOD"].ToString();
                    model.VSTKDES = dr["STKDES"].ToString();
                    model.VSTKGRP = dr["ORD_STKGRP"].ToString();
                    model.VPrice = dr["ORD_Price"].ToString();
                    model.VSalePrice = dr["ORD_SalePrice"].ToString();

                    model.VORDDAT = dr["ORD_Date"].ToString();
                    model.Item_Type = dr["Item_Type"].ToString();

                    model.VDiscount = dr["ORD_Discount"].ToString();
                    model.AmtQty = dr["ORD_Qty"].ToString();
                    model.TotalAmt = dr["ORD_Amt"].ToString();



                    Getdata.Add(new OrderListDetailGetdata { val = model });

                }
                //S20161016
                dr.Close();
                dr.Dispose();
                command.Dispose();
                connection.Close();
                //E20161016

            }
            return Json(new { Getdata }, JsonRequestBehavior.AllowGet);

        }
    }
}
