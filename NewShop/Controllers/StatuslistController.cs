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
    public class StatuslistController : Controller
    {
        // GET: /Statuslist/
        public ActionResult Index()
        {
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            else
            {
                List<SaleOrder_History> GetaleOrder_History = new List<SaleOrder_History>();
                SaleOrder_History _model = new SaleOrder_History();
                GetaleOrder_History.Add(new SaleOrder_History()
                {
                    CUSCOD = "",
                    CUSNAM = "",
                    STKCOD = "",
                    FullDES = "",
                    QTY = "",
                    SALPRICE = "",
                    AMT = "",
                    DISCOUNT = "",
                    ORDDAT = "",
                    SONUM = "",
                    PINUM = "",
                    INVNUM = ""
                });
                _model.SaleOrder_History_Grid = GetaleOrder_History;
                return View("Index", _model);
            }
            return View();
        }
        public ActionResult Order_History(string cus, string STKCOD, string StartDate, string EndDate, string Slm, string UserCreate, string typeUser)
        {
            List<SaleOrder_History> GetaleOrder_History = new List<SaleOrder_History>();
            SaleOrder_History model = new SaleOrder_History();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_Search_SaleOrder_History_x", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inUserCreate", UserCreate);
            command.Parameters.AddWithValue("@inSTKCOD", STKCOD);
            command.Parameters.AddWithValue("@inCUSCOD", cus);
            command.Parameters.AddWithValue("@in_SDate", StartDate);
            command.Parameters.AddWithValue("@in_EDate", EndDate);
            command.Parameters.AddWithValue("@inSLMCODE", Slm);
            command.Parameters.AddWithValue("@Usertype", typeUser);
            SqlDataReader drb = command.ExecuteReader();
            while (drb.Read())
            {
                GetaleOrder_History.Add(new SaleOrder_History()
                {
                    CUSCOD = drb["CUSCOD"].ToString(),
                    CUSNAM = drb["CUSNAM"].ToString(),
                    STKCOD = drb["STKCOD"].ToString(),
                    FullDES = drb["FullDes"].ToString(),
                    QTY = String.Format("{0:0}", Convert.ToDecimal(drb["Qty"].ToString())),
                    SALPRICE = String.Format("{0:0.00}", Convert.ToDecimal(drb["SALPRICE"].ToString())),
                    AMT = String.Format("{0:0.00}", Convert.ToDecimal(drb["AMT"].ToString())),
                    DISCOUNT = String.Format("{0:0.00}", Convert.ToDecimal(drb["Discount"].ToString())),
                    ORDDAT = drb["ORDDAT"].ToString(),
                    SONUM = drb["SONUM"].ToString(),
                    PINUM = drb["PINUM"].ToString(),
                    INVNUM = drb["INVNUM"].ToString(),
                    SLMCOD = drb["SLMCOD"].ToString(),
                });
            }
            drb.Close();
            drb.Dispose();
            command.Dispose();
            Connection.Close();
            return PartialView("_PartialSaleOrder_History", GetaleOrder_History);
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
        public JsonResult GetOrder(string Usertype, string SOW, string UserCreate, string CUSCOD, string SLMCODE, string SO_SDate, string SO_EDate, string STKCOD, string DType)
        {
            List<OrderListGetdata> Getdata = new List<OrderListGetdata>();
            List<OrderListGetdata> Getdatatop = new List<OrderListGetdata>();
            SaleOrderList model = null;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("P_Search_SaleOrder", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inUserCreate", UserCreate);
                command.Parameters.AddWithValue("@inSLMCODE", SLMCODE);
                command.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                command.Parameters.AddWithValue("@inSTKCOD", STKCOD);
                command.Parameters.AddWithValue("@inSO_SDate", SO_SDate);
                command.Parameters.AddWithValue("@inSO_EDate", SO_EDate);
                command.Parameters.AddWithValue("@inDType", DType);
                command.Parameters.AddWithValue("@inSOW", SOW);
                command.Parameters.AddWithValue("@Usertype", Usertype);
                command.CommandTimeout = 120; //60 sec = 1 min
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    model = new SaleOrderList();
                    model.DType = dr["DType"].ToString();
                    model.RowNo = dr["RowNo"].ToString();
                    model.ORD_ID = dr["ORD_ID"].ToString();
                    model.ORD_DocNo = dr["ORD_DocNo"].ToString();
                    model.ORD_Date = dr["ORD_Date"].ToString();
                    model.ORD_TotalAmt = dr["ORD_TotalAmt"].ToString();
                    model.QTY = dr["ORD_TotalQty"].ToString();
                    model.Picking = dr["Picking"].ToString();
                    model.Picking_Date = dr["Picking_Date"].ToString();
                    model.Invoice = dr["Invoice"].ToString();
                    model.Invoice_Date = dr["Invoice_Date"].ToString();
                    model.InsertDate = dr["InsertDate"].ToString();
                    model.Customer = dr["Customer"].ToString();
                    model.SlmCod = dr["SlmCod"].ToString();
                    model.Prn_ORD = dr["Prn_ORD"].ToString();
                    model.Order_Status = dr["Order_Status"].ToString();
                    model.PrintID = dr["PrintID"].ToString();
                    model.StatusID = dr["StatusID"].ToString();
                    model.Latitude = dr["Latitude"].ToString();
                    model.Longitude = dr["Longitude"].ToString();
                    model.DeliveryDate = dr["DeliveryDate"].ToString();
                    Getdata.Add(new OrderListGetdata { val = model });
                }
                dr.Close();
                dr.Dispose();
                command.Dispose();
                connection.Close();
            }
            return Json(new { Getdata, Getdatatop }, JsonRequestBehavior.AllowGet);
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
                dr.Close();
                dr.Dispose();
                command.Dispose();
                connection.Close();
            }
            return Json(new { Getdata }, JsonRequestBehavior.AllowGet);
        }
    }
}
