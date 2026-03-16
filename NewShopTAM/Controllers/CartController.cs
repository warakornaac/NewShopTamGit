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
using System.Threading.Tasks;
using Dapper;
using System.Runtime.Caching;

namespace NewShopTAM.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/

        public ActionResult Index()
        {
            //this.Session["UserType"] = "";
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

                        // Doc = Docdisplay;
                        // Docsub = Docdisplay;
                    }

                    var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                    SqlConnection Connection = new SqlConnection(connectionString);
                    Connection.Open();
                    string discount = string.Empty;
                    var command = new SqlCommand("P_Search_CusDiscount", Connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                    command.Parameters.AddWithValue("@UsrTyp", this.Session["UserType"]);
                    //commandCrTerm.ExecuteNonQuery();
                    SqlDataReader cusdis = command.ExecuteReader();
                    while (cusdis.Read())
                    {
                        discount = cusdis["Discount"].ToString();
                    }
                    //CrTerm.Dispose();
                    //S20161016
                    cusdis.Close();
                    cusdis.Dispose();
                    ViewBag.Xcusdiscount = discount;
                    command.Dispose();


                    List<SelectListItem> Code = new List<SelectListItem>();
                    var commandTermDay = new SqlCommand("P_CusCreditTermDay", Connection);
                    commandTermDay.CommandType = CommandType.StoredProcedure;


                    //commandCrTerm.ExecuteNonQuery();
                    SqlDataReader TermDay = commandTermDay.ExecuteReader();
                    while (TermDay.Read())
                    {
                        Code.Add(new SelectListItem()
                        {
                            // ID = rev_CUSTYP.GetValue(0).ToString(),
                            Value = TermDay["Lookup ID"].ToString(),
                            Text = TermDay["Lookup ID"].ToString()
                        });

                    }
                    //CrTerm.Dispose();
                    //S20161016
                    TermDay.Close();
                    TermDay.Dispose();

                    commandTermDay.Dispose();

                    ViewBag.TermDay = Code;


                    string company = string.Empty;
                    var commandCom = new SqlCommand("P_Get_Companny_catalog", Connection);
                    commandCom.CommandType = CommandType.StoredProcedure;
                    commandCom.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                    // commandCom.ExecuteNonQuery();
                    SqlDataReader CrCom = commandCom.ExecuteReader();
                    while (CrCom.Read())
                    {
                        company = CrCom["Company"].ToString();
                    }

                    CrCom.Close();
                    CrCom.Dispose();
                    commandCom.Dispose();
                    Connection.Close();
                    ViewBag.company = company;
                }
            }
            return View();
        }
        public JsonResult GetdataCheckShopping(string CUSCOD, string usrlogin, string Company, string usrtype)
        {
            string exerror = string.Empty;
            List<ItemListGetdata> Getdata = new List<ItemListGetdata>();
            ItemOrdering model = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {

                var command = new SqlCommand("P_CheckOrderCart_Catalog", Connection);

                // var command = new SqlCommand("P_ShoppingCart_list", Connection);
                command.CommandTimeout = 0;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                command.Parameters.AddWithValue("@inStatus", "N,W");
                command.Parameters.AddWithValue("@Company", Company);
                command.Parameters.AddWithValue("@usrlogin", usrlogin);
                command.Parameters.AddWithValue("@usrtype", usrtype);
                command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    model = new ItemOrdering();

                    model.CUSCOD = dr["CUSCOD"].ToString();

                    Getdata.Add(new ItemListGetdata { val = model });

                }
                //dr.Dispose();
                //S20161016
                dr.Close();
                dr.Dispose();
                command.Dispose();
                //E20161016


                //E20161016
            }
            catch (Exception ex)
            {

                exerror = ex.Message + '/' + ex.Source + '/' + ex.HelpLink + '/' + ex.HResult;

            }
            Connection.Close();

            return Json(new { Getdata, exerror }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetdataCheckQuatationShopping(string Quatation)
        {
            string exerror = string.Empty;
            List<ItemListGetdata> Getdata = new List<ItemListGetdata>();
            ItemOrdering model = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {

                var command = new SqlCommand("P_CheckQuatationOrderCart_Catalog", Connection);

                // var command = new SqlCommand("P_ShoppingCart_list", Connection);
                command.CommandTimeout = 0;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@QuatationNo", Quatation);

                command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    model = new ItemOrdering();

                    model.CUSCOD = dr["CUSCOD"].ToString();

                    Getdata.Add(new ItemListGetdata { val = model });

                }
                //dr.Dispose();
                //S20161016
                dr.Close();
                dr.Dispose();
                command.Dispose();
                //E20161016


                //E20161016
            }
            catch (Exception ex)
            {

                exerror = ex.Message + '/' + ex.Source + '/' + ex.HelpLink + '/' + ex.HResult;

            }
            Connection.Close();

            return Json(new { Getdata, exerror }, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetdataShoppingDapper(string CUSCOD, string usrlogin, string Company, string usrtype, string shiptocode)
        {
            int sumQty = 0;
            int sumSalePrice = 0;
            int sumDiscount = 0;
            string exerror = string.Empty;
            string creditterm = string.Empty;
            List<ItemListGetdata> Getdata = new List<ItemListGetdata>();

            try
            {
                var connectionString = ConfigurationManager
                    .ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    // เปิด connection แบบ async
                    await connection.OpenAsync();

                    // --------------------------------------------------
                    // 1) Query Stored Procedure หลัก (Async + Dapper)
                    // --------------------------------------------------
                    var parameters = new DynamicParameters();
                    parameters.Add("@inCUSCOD", CUSCOD);
                    parameters.Add("@inStatus", "N,W");
                    parameters.Add("@Company", Company);
                    parameters.Add("@usrlogin", usrlogin);
                    parameters.Add("@usrtype", usrtype);
                    parameters.Add("@shiptocode", shiptocode);

                    // 🔥 QueryAsync (Async)
                    var rows = (await connection.QueryAsync<dynamic>(
                        "P_ShoppingCart_list_catalog",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    )).ToList();

                    // Manual mapping
                    foreach (var r in rows)
                    {
                        var row = (IDictionary<string, object>)r;

                        var model = new ItemOrdering
                        {
                            CartID = row["ID"]?.ToString(),
                            PRCLST_NO = row["PRCLST_NO"]?.ToString(),
                            CUSCOD = row["CUSCOD"]?.ToString(),
                            ORDDAT = row["ORDDAT"]?.ToString(),
                            STKCOD = row["STKCOD"]?.ToString(),
                            Company = row["Company"]?.ToString(),
                            STKGRP = row["STKGRP"]?.ToString(),
                            STKGRPNam = row["GRPNAM"]?.ToString(),
                            STKDES = row["STKDES"]?.ToString(),
                            MINORD = row["MINORD"]?.ToString(),
                            Price = row["Price"]?.ToString(),
                            SalePrice = row["SalePrice"]?.ToString(),
                            ExpectPrice = row["ExpectPrice"]?.ToString(),
                            Qty = row["Qty"]?.ToString(),
                            TotalPrice = row["TotalPrice"]?.ToString(),
                            TotalDiscount = row["TotalDiscount"]?.ToString(),
                            Amt = row["Amt"]?.ToString(),
                            Discount = row["Discount"]?.ToString(),
                            Status = row["Status"]?.ToString(),
                            LineNote = row["LineNote"]?.ToString(),
                            UOM = row["UOM"]?.ToString(),
                            Promotion = row["Promotion"]?.ToString() ?? "-",
                            PromotionDesc = row["PromotionDesc"]?.ToString(),
                            Item_Type = row["Item_Type"]?.ToString(),
                            InStock = row["InStock"]?.ToString(),
                            PrcApproveBy = row["PrcApproveBy"]?.ToString(),
                            Stock = row["Stock"]?.ToString(),
                            Backorder = row["BackOrder"]?.ToString(),
                            Ready_Status = row["Ready_Status"]?.ToString(),
                            maxord = row["maxord"]?.ToString(),
                            PrcRemark = row["PrcRemark"]?.ToString(),
                            WH_Location = row["WH Location"]?.ToString(),
                            KDC_QTY = row["KDC-QTY"]?.ToString(),
                            PDC_QTY = row["PDC-QTY"]?.ToString(),
                            AccessID = row["AccessID"]?.ToString(),
                            Intransit = row["Intrnsit"]?.ToString(),
                            InsertedBy = row["Inserted By"]?.ToString()
                        };

                        sumQty += Convert.ToInt32(model.Qty ?? "0");

                        Getdata.Add(new ItemListGetdata { val = model });
                    }

                    // --------------------------------------------------
                    // 2) Query credit term (Async)
                    // --------------------------------------------------
                    creditterm = await connection.ExecuteScalarAsync<string>(
                        "P_Search_Credit Term",
                        new { inCUSCOD = CUSCOD },
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
            catch (Exception ex)
            {
                exerror = ex.Message;
            }

            return Json(
                new { Getdata, sumQty, sumSalePrice, sumDiscount, creditterm, exerror },
                JsonRequestBehavior.AllowGet
            );
        }
        public JsonResult GetdataShopping(string CUSCOD, string usrlogin, string Company, string usrtype, string shiptocode/*, string quatation, string status*/)
        {
            int sumQty = 0;
            int sumSalePrice = 0;
            int sumDiscount = 0;
            string stkgrp = string.Empty;
            string exerror = string.Empty;
            string creditterm = string.Empty;
            List<ItemListGetdata> Getdata = new List<ItemListGetdata>();
            ItemOrdering model = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {

                var command = new SqlCommand("P_ShoppingCart_list_catalog", Connection);

                // var command = new SqlCommand("P_ShoppingCart_list", Connection);
                command.CommandTimeout = 0;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                //command.Parameters.AddWithValue("@inStatus", status);
                command.Parameters.AddWithValue("@inStatus", "N,W");
                command.Parameters.AddWithValue("@Company", Company);
                command.Parameters.AddWithValue("@usrlogin", usrlogin);
                command.Parameters.AddWithValue("@usrtype", usrtype);
                command.Parameters.AddWithValue("@shiptocode", shiptocode);
                // command.Parameters.AddWithValue("@QuotationNo", quatation);
                //command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    model = new ItemOrdering();

                    model.CartID = dr["ID"].ToString();
                    model.PRCLST_NO = dr["PRCLST_NO"].ToString();
                    model.CUSCOD = dr["CUSCOD"].ToString();
                    model.ORDDAT = dr["ORDDAT"].ToString();
                    model.STKCOD = dr["STKCOD"].ToString();
                    model.Company = dr["Company"].ToString();
                    model.STKGRP = dr["STKGRP"].ToString();
                    model.STKGRPNam = dr["GRPNAM"].ToString();
                    model.STKDES = dr["STKDES"].ToString();
                    model.MINORD = dr["MINORD"].ToString();
                    model.Price = dr["Price"].ToString();
                    model.SalePrice = dr["SalePrice"].ToString();
                    model.ExpectPrice = dr["ExpectPrice"].ToString();
                    model.Qty = dr["Qty"].ToString();
                    model.TotalPrice = dr["TotalPrice"].ToString();
                    model.TotalDiscount = dr["TotalDiscount"].ToString();
                    model.Amt = dr["Amt"].ToString();
                    model.Discount = dr["Discount"].ToString();
                    model.Status = dr["Status"].ToString();
                    model.LineNote = dr["LineNote"].ToString();
                    model.UOM = dr["UOM"].ToString();
                    model.Promotion = dr["Promotion"].ToString();
                    model.PromotionDesc = dr["PromotionDesc"].ToString();
                    sumQty += Convert.ToInt32(dr["Qty"].ToString());
                    string sum = dr["SalePrice"].ToString();
                    model.Item_Type = dr["Item_Type"].ToString();
                    model.InStock = dr["InStock"].ToString();
                    model.PrcApproveBy = dr["PrcApproveBy"].ToString();
                    model.Stock = dr["Stock"].ToString();
                    model.Backorder = dr["BackOrder"].ToString();
                    model.Ready_Status = dr["Ready_Status"].ToString();
                    model.maxord = dr["maxord"].ToString();
                    model.maxord = dr["maxord"].ToString();
                    model.PrcRemark = dr["PrcRemark"].ToString();
                    model.WH_Location = dr["WH Location"].ToString();
                    model.KDC_QTY = dr["KDC-QTY"].ToString();
                    model.PDC_QTY = dr["PDC-QTY"].ToString();
                    model.AccessID = dr["AccessID"].ToString();
                    model.AccessID = dr["AccessID"].ToString();
                    model.AccessID = dr["AccessID"].ToString();
                    model.Intransit = dr["Intrnsit"].ToString();
                    model.InsertedBy = dr["Inserted By"].ToString();
                    //model.Promotion_Foc = dr["Promotion_Foc"].ToString();
                    //sumSalePrice += Convert.(sum);
                    Getdata.Add(new ItemListGetdata { val = model });

                }
                //dr.Dispose();
                //S20161016
                dr.Close();
                dr.Dispose();
                command.Dispose();
                //E20161016


                var commandCrTerm = new SqlCommand("P_Search_Credit Term", Connection);
                commandCrTerm.CommandType = CommandType.StoredProcedure;
                commandCrTerm.Parameters.AddWithValue("@inCUSCOD", CUSCOD);

                //commandCrTerm.ExecuteNonQuery();
                SqlDataReader CrTerm = commandCrTerm.ExecuteReader();
                while (CrTerm.Read())
                {
                    creditterm = CrTerm["PayTrm"].ToString();
                }
                //CrTerm.Dispose();
                //S20161016
                CrTerm.Close();
                CrTerm.Dispose();

                commandCrTerm.Dispose();
                //E20161016
            }
            catch (Exception ex)
            {

                exerror = ex.Message + '/' + ex.Source + '/' + ex.HelpLink + '/' + ex.HResult;

            }
            Connection.Close();

            return Json(new { Getdata, sumQty, sumSalePrice, sumDiscount, creditterm, exerror }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTransportation(string cucod)
        {
            List<Transport_DenyK99> List = new List<Transport_DenyK99>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_Search_Transport", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inCUSCOD", cucod);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                List.Add(new Transport_DenyK99()
                {
                    Code = dr["Code"].ToString(),
                    Name = dr["Name"].ToString()
                });

            }
            //S20161016
            dr.Close();
            dr.Dispose();
            command.Dispose();
            //E20161016
            Connection.Close();
            return Json(List, JsonRequestBehavior.AllowGet);
            //return Json(new { List }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetQuatationno(string CUSCOD)
        {

            List<ItemListQno> ListQno = new List<ItemListQno>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_Quatation_No", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Customer", CUSCOD);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                ListQno.Add(new ItemListQno()
                {
                    QuatationNo = dr["QuatationNo"].ToString(),
                    Detail = dr["Detail"].ToString(),
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(ListQno, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataCusQuatation(string CUSCOD, string Company, string usrlogin, string _valQuatation, string usrtype, string shiptocode)
        {
            int sumQty = 0;
            int sumSalePrice = 0;
            int sumDiscount = 0;
            string stkgrp = string.Empty;
            string exerror = string.Empty;
            string creditterm = string.Empty;
            List<ItemListGetdata> Getdata = new List<ItemListGetdata>();
            ItemOrdering model = null;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {

                var command = new SqlCommand("P_QuatationCart_list", Connection);
                command.CommandTimeout = 0;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCUSCOD", CUSCOD);
                command.Parameters.AddWithValue("@inStatus", "Q");
                command.Parameters.AddWithValue("@Company", Company);
                command.Parameters.AddWithValue("@usrlogin", usrlogin);
                command.Parameters.AddWithValue("@QuatationNo", _valQuatation);
                command.Parameters.AddWithValue("@usrtype", usrtype);
                command.Parameters.AddWithValue("@shiptocode", shiptocode);
                //command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    model = new ItemOrdering();
                    // DateTime date = Convert.ToDateTime(dr["ORDDAT"].ToString());
                    // string formatted = date.ToString("dd/M/yyyy");
                    // model.CartID = dr.GetValue(0).ToString();
                    // model.PRCLST_NO = dr["PRCLST_NO"].ToString();
                    // model.CUSCOD = dr["CUSCOD"].ToString();
                    // model.ORDDAT = formatted;
                    // model.STKCOD = dr["STKCOD"].ToString();
                    // model.Company = dr["Company"].ToString();
                    // model.STKGRP = dr["STKGRP"].ToString();
                    // stkgrp = model.STKGRP;

                    // model.STKGRPNam = "-";
                    // model.STKDES = dr["STKDES"].ToString();
                    // model.MINORD = dr["MINORD"].ToString();
                    // model.Price = dr["Price"].ToString();
                    // model.SalePrice = dr["SalePrice"].ToString();
                    // model.ExpectPrice = dr["ExpectPrice"].ToString();
                    // model.Qty = dr["Qty"].ToString();
                    // model.TotalPrice = dr["TotalPrice"].ToString();
                    // model.TotalDiscount = dr["TotalDiscount"].ToString();
                    // model.Amt = dr["Amt"].ToString();
                    // model.Discount = dr["Discount"].ToString();
                    // //sumDiscount += Convert.ToInt32(dr["Discount"].ToString());
                    // model.Status = dr["Status"].ToString();
                    // model.LineNote = dr["LineNote"].ToString();
                    // model.UOM = dr["UOM"].ToString();
                    // model.Promotion = dr["Promotion"].ToString();
                    // model.PromotionDesc = dr["PromotionDesc"].ToString();
                    // sumQty += Convert.ToInt32(dr["Qty"].ToString());
                    // string sum = dr["SalePrice"].ToString();
                    // model.Item_Type = dr["Item_Type"].ToString();
                    // model.InStock = dr["InStock"].ToString();
                    // model.PrcApproveBy = dr["PrcApproveBy"].ToString();
                    // model.Stock = dr["Stock"].ToString();
                    // model.Backorder = dr["BackOrder"].ToString();
                    // model.Ready_Status = dr["Ready_Status"].ToString();
                    // model.maxord = dr["maxord"].ToString();
                    // model.maxord = dr["maxord"].ToString();
                    // model.PrcRemark = dr["PrcRemark"].ToString();
                    // model.WH_Location = dr["WH Location"].ToString();


                    //// model.Type_Cal = dr["Type_Cal"].ToString();
                    // //model.Special_Discount = dr["Special_Discount"].ToString();
                    model.DiscountPercent = dr["ORD_DiscountPercent"].ToString();
                    // model.ORDMOD_Type = dr["ORDMOD_Type"].ToString();
                    // model.ORD_Type = dr["ORD_Type"].ToString();
                    // model.GenID = dr["GenID"].ToString();
                    // model.PrcRemark = dr["PrcRemark"].ToString();
                    // model.WH_Location = dr["WH Location"].ToString();
                    //sumSalePrice += Convert.(sum);
                    model.CartID = dr["ID"].ToString();
                    model.PRCLST_NO = dr["PRCLST_NO"].ToString();
                    model.CUSCOD = dr["CUSCOD"].ToString();
                    model.ORDDAT = dr["ORDDAT"].ToString();
                    model.STKCOD = dr["STKCOD"].ToString();
                    model.Company = dr["Company"].ToString();
                    model.STKGRP = dr["STKGRP"].ToString();
                    model.STKGRPNam = dr["GRPNAM"].ToString();
                    model.STKDES = dr["STKDES"].ToString();
                    model.MINORD = dr["MINORD"].ToString();
                    model.Price = dr["Price"].ToString();
                    model.SalePrice = dr["SalePrice"].ToString();
                    model.ExpectPrice = dr["ExpectPrice"].ToString();
                    model.Qty = dr["Qty"].ToString();
                    model.TotalPrice = dr["TotalPrice"].ToString();
                    model.TotalDiscount = dr["TotalDiscount"].ToString();
                    model.Amt = dr["Amt"].ToString();
                    model.Discount = dr["Discount"].ToString();
                    model.Status = dr["Status"].ToString();
                    model.LineNote = dr["LineNote"].ToString();
                    model.UOM = dr["UOM"].ToString();
                    model.Promotion = dr["Promotion"].ToString();
                    model.PromotionDesc = dr["PromotionDesc"].ToString();
                    sumQty += Convert.ToInt32(dr["Qty"].ToString());
                    string sum = dr["SalePrice"].ToString();
                    model.Item_Type = dr["Item_Type"].ToString();
                    model.InStock = dr["InStock"].ToString();
                    model.PrcApproveBy = dr["PrcApproveBy"].ToString();
                    model.Stock = dr["Stock"].ToString();
                    model.Backorder = dr["BackOrder"].ToString();
                    model.Ready_Status = dr["Ready_Status"].ToString();
                    model.maxord = dr["maxord"].ToString();
                    model.maxord = dr["maxord"].ToString();
                    model.PrcRemark = dr["PrcRemark"].ToString();
                    model.WH_Location = dr["WH Location"].ToString();
                    model.KDC_QTY = dr["KDC-QTY"].ToString();
                    model.PDC_QTY = dr["PDC-QTY"].ToString();
                    model.AccessID = dr["AccessID"].ToString();
                    Getdata.Add(new ItemListGetdata { val = model });

                }
                //dr.Dispose();
                //S20161016
                dr.Close();
                dr.Dispose();
                command.Dispose();
                //E20161016


                var commandCrTerm = new SqlCommand("P_Search_Credit Term", Connection);
                commandCrTerm.CommandType = CommandType.StoredProcedure;
                commandCrTerm.Parameters.AddWithValue("@inCUSCOD", CUSCOD);

                //commandCrTerm.ExecuteNonQuery();
                SqlDataReader CrTerm = commandCrTerm.ExecuteReader();
                while (CrTerm.Read())
                {
                    creditterm = CrTerm["PayTrm"].ToString();
                }
                //CrTerm.Dispose();
                //S20161016
                CrTerm.Close();
                CrTerm.Dispose();

                commandCrTerm.Dispose();
                //E20161016all
            }
            catch (Exception ex)
            {

                exerror = ex.Message + '/' + ex.Source + '/' + ex.HelpLink + '/' + ex.HResult;

            }
            Connection.Close();

            return Json(new { Getdata, sumQty, sumSalePrice, sumDiscount, creditterm, exerror }, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> DeliveryModeDapper(string userid, string cuscod)
        {
            //หากเป็นคน Login เดิม จะดึง cache มาใช้
            string cacheKey = $"DeliveryMode_{userid}";

            // เช็คใน MemoryCache ก่อน
            var cache = MemoryCache.Default;
            if (cache.Contains(cacheKey))
            {
                var cachedData = cache.Get(cacheKey) as List<Itemordertype>;
                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }

            List<Itemordertype> ordertypes = new List<Itemordertype>();
            string exerror = string.Empty;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@UsrID", userid);
                    parameters.Add("@CusCod", cuscod);

                    //Dapper + Async read
                    var rows = await connection.QueryAsync<dynamic>(
                        "p_OrdType",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

                    foreach (var r in rows)
                    {
                        ordertypes.Add(new Itemordertype
                        {
                            ORD_Type = r.MOD?.ToString(),
                            ORD_TypeName = r.Description?.ToString()
                        });
                    }
                }
                //Cache 10 นาที (หรือเปลี่ยนได้)
                cache.Add(
                    cacheKey,
                    ordertypes,
                    new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60)
                    }
                );
            }
            catch (Exception ex)
            {
                exerror = ex.Message;
            }

            return Json(ordertypes, JsonRequestBehavior.AllowGet);
        }
        //ดึง DeliveryMode CBI/CDB/MOT
        public JsonResult DeliveryMode(string userid, string cuscod)
        {
            List<Itemordertype> ordertype = new List<Itemordertype>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("p_OrdType", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UsrID", userid);
            command.Parameters.AddWithValue("@CusCod", cuscod);

            //command.ExecuteNonQuery();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                ordertype.Add(new Itemordertype()
                {
                    ORD_Type = dr["MOD"].ToString(),
                    ORD_TypeName = dr["Description"].ToString(),
                });
            }
            //S20161016
            dr.Close();
            dr.Dispose();
            command.Dispose();
            //E20161016
            Connection.Close();
            return Json(ordertype, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrdertype(string type, string mod)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();

            string ORD_Time = "";
            //string DateCaldate = "";
            string DateCal = "";
            // DateTime z ;
            //SqlCommand cmd = new SqlCommand("SELECT ORD_Type, ORD_TypeName, ORD_Day, ORD_Time, CONVERT(varchar, GETDATE() + ORD_Day, 103) AS DateCal FROM   OrderType where ORD_Type =N'" + type + "'", Connection);
            SqlCommand cmd = new SqlCommand("SELECT ORDTYP,Description,ORD_Day,ORD_Time,DateCal,[MOD] FROM [v_ORDTYPE] where ORDTYP =N'" + type + "' and [MOD]  =N'" + mod + "' ", Connection);
            //var data = db.v_OrderType.Where(c => c.ORD_Type == type).ToArray();
            SqlDataReader rev_typ = cmd.ExecuteReader();
            while (rev_typ.Read())
            {
                // ORD_T ype = rev_typ.GetValue(0).ToString();
                // ORD_TypeName = rev_typ.GetValue(1).ToString();
                if (type != "S" && type != "X" && type != "BO" && type != "RS")
                {
                    ORD_Time = rev_typ["ORD_Time"].ToString();
                    // DateTime date = Convert.ToDateTime(rev_typ["DateCal"].ToString());
                    DateCal = rev_typ["DateCal"].ToString(); // date.ToString("yyyy-MM-dd");

                    // z = DateTime.UtcNow;
                }
                else
                {
                    ORD_Time = rev_typ["ORD_Time"].ToString();

                    DateCal = rev_typ["DateCal"].ToString();

                }
            }

            //S20161016
            rev_typ.Close();
            rev_typ.Dispose();
            cmd.Dispose();
            //E20161016
            Connection.Close();

            return Json(new { ORD_Time, DateCal }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getordertypeby(string type)
        {

            List<Itemordertype> ordertype = new List<Itemordertype>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            var command = new SqlCommand("P_List_ModeOfDelivery", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Mode", type);
            command.Parameters.AddWithValue("@StrSql", "");

            //command.ExecuteNonQuery();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {


                ordertype.Add(new Itemordertype()
                {
                    ORD_Type = dr["ORD_Type"].ToString(),
                    ORD_TypeName = dr["ORD_TypeName"].ToString(),
                });
            }
            //S20161016
            dr.Close();
            dr.Dispose();
            command.Dispose();
            //E20161016
            Connection.Close();
            return Json(ordertype, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CusDiscount(string cuscod, string type)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string discount = string.Empty;
            string CashDiscount = string.Empty;
            var command = new SqlCommand("P_Search_CusDiscount", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inCUSCOD", cuscod);
            command.Parameters.AddWithValue("@UsrTyp", type);
            //commandCrTerm.ExecuteNonQuery();
            SqlDataReader cusdis = command.ExecuteReader();
            while (cusdis.Read())
            {
                CashDiscount = cusdis["Cash Discount"].ToString();
                discount = cusdis["Discount"].ToString();
            }
            //CrTerm.Dispose();
            //S20161016
            cusdis.Close();
            cusdis.Dispose();

            command.Dispose();
            Connection.Close();
            return Json(new { discount, CashDiscount }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CusCreditTermDay()
        {

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<string> Code = new List<string>();
            var command = new SqlCommand("P_CusCreditTermDay", Connection);
            command.CommandType = CommandType.StoredProcedure;


            //commandCrTerm.ExecuteNonQuery();
            SqlDataReader cusdis = command.ExecuteReader();
            while (cusdis.Read())
            {

                Code.Add(cusdis["Lookup ID"].ToString());
            }
            //CrTerm.Dispose();
            //S20161016
            cusdis.Close();
            cusdis.Dispose();

            command.Dispose();
            Connection.Close();

            return Json(Code, JsonRequestBehavior.AllowGet);

        }
        public JsonResult CusCreditTerm(string crdterm)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string Creditper = string.Empty;
            var command = new SqlCommand("P_CusCreditTerm", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CreditTerm", crdterm);

            //commandCrTerm.ExecuteNonQuery();
            SqlDataReader cusdis = command.ExecuteReader();
            while (cusdis.Read())
            {
                Creditper = cusdis["Lookup code"].ToString();
            }
            //CrTerm.Dispose();
            //S20161016
            cusdis.Close();
            cusdis.Dispose();

            command.Dispose();
            Connection.Close();

            return Json(Creditper, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetdataCusShipping(string XXcus)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<ItemListshipto> cusshipping = new List<ItemListshipto>();
            shipto model = null;
            // SqlCommand cmd = new SqlCommand("select *    from dbo.v_NVcust_ShiptoAddr  where [customer No_] ='"+XXcus+"' and code not like 'N9%' ", Connection);
            SqlCommand cmd = new SqlCommand("select *    from dbo.v_NVcust_ShiptoAddr_TAM  where [customer No_] ='" + XXcus + "' order by [Code] ", Connection);
            SqlDataReader rev_Mod = cmd.ExecuteReader();
            //var cusshipping = db.v_NVcust_ShiptoAddr.Where(c => c.Customer_No_ == XXcus && c.Code != "N9" || c.Code != "N99").ToArray();
            // var cusshipping = rev_Mod;
            // ID = rev_CUSTYP.GetValue(0).ToString(),
            while (rev_Mod.Read())
            {
                model = new shipto();
                model.customer = rev_Mod["Customer No_"].ToString();
                model.code = rev_Mod["Code"].ToString();
                model.name = rev_Mod["Name"].ToString();
                model.name2 = rev_Mod["Name 2"].ToString();
                model.address = rev_Mod["Address"].ToString();
                model.address2 = rev_Mod["Address 2"].ToString();
                model.city = rev_Mod["City"].ToString();
                model.postcode = rev_Mod["Post code"].ToString();
                cusshipping.Add(new ItemListshipto { val = model });
            }
            //S20161016
            rev_Mod.Close();
            rev_Mod.Dispose();
            cmd.Dispose();
            //E20161016
            Connection.Close();
            return Json(cusshipping, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DelShoppingCart(string DataSend)
        {
            List<ItemConfirm> _ItemList = new JavaScriptSerializer().Deserialize<List<ItemConfirm>>(DataSend);
            bool message = false;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            try
            {

                if (_ItemList.Count > 0)
                {
                    for (int i = 0; i < _ItemList.Count; i++)
                    {
                        SqlCommand cmd = new SqlCommand("P_Del_Ordering_Cart", conn);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@inID", cartid);
                        cmd.Parameters.AddWithValue("@inID", _ItemList[i].Vidorder);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }

                conn.Close();
                message = true;
            }
            catch (Exception ex)
            {
                message = false;
            }
            return Json(message, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ConfirmationdataTemp(string totallabor, string totaltransportcost, string typecke, string CusPo, string CreditTerm, string takeorderby, string Remark, string codetransportation, string CusShipping, string codeShipping, string Moddate, string Modtime, string Modtype, string Xusrlogin, string Xcus, string XSul, string DataSend, string sumvat, string sumpro, string ORD_TotalPrice, string ORD_TotalDiscount, string sumqty, string sumstk, string dateDelivery, string DeliveryTime)
        {

            List<ItemConfirm> _ItemList = new JavaScriptSerializer().Deserialize<List<ItemConfirm>>(DataSend);
            //List<ItemConfirmPro> _ItemListPro = new JavaScriptSerializer().Deserialize<List<ItemConfirmPro>>(DataSendPro);
            string message = "false";
            string Docorder = string.Empty;
            string Err_Flg = string.Empty;
            string Err_Message = string.Empty;
            string IDProc = string.Empty;
            string Prodiscount = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();

            // int seqmax= 0;
            int id = 0;
            int idorder = 0;
            string docgen = string.Empty;
            string DocNo = string.Empty;

            //var commandDoc_Control = new SqlCommand("P_SearchSave_Doc_Control", Connection);
            //commandDoc_Control.CommandType = CommandType.StoredProcedure;
            //commandDoc_Control.Parameters.AddWithValue("@inDocNam", "Order");
            //SqlParameter returnValuedoc = new SqlParameter("@outResult", SqlDbType.NVarChar, 100);
            //returnValuedoc.Direction = System.Data.ParameterDirection.Output;
            //commandDoc_Control.Parameters.Add(returnValuedoc);
            //commandDoc_Control.ExecuteNonQuery();
            //docgen = returnValuedoc.Value.ToString();
            //string[] strSplit = docgen.Split('=');
            //Docorder = strSplit[1];
            //DocNo = Docorder;
            Connection.Close();
            if (_ItemList.Count > 0)
            {
                string CUSID = Xcus;
                string SLMCOD = XSul;
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
                {
                    conn.Open();
                    SqlTransaction trans = null;
                    try
                    {
                        SqlCommand cmd = new SqlCommand("P_Save_OrderConfirmTH_Temp_catalog", conn);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@inORD_DocNo", Docorder);
                        cmd.Parameters.AddWithValue("@inORD_DocNo", "");
                        //cmd.Parameters.AddWithValue("@inORD_Date", _ItemList[0].VORDDAT);
                        cmd.Parameters.AddWithValue("@inORD_Date", DateTime.Now.ToString("dd/MM/yyy"));
                        cmd.Parameters.AddWithValue("@inCUSCOD", CUSID);
                        //cmd.Parameters.AddWithValue("@inCompany", _ItemList[0].VCompany);
                        cmd.Parameters.AddWithValue("@inORD_DiscountAmt", Convert.ToDecimal(_ItemList[0].AmtDiscount));
                        cmd.Parameters.AddWithValue("@inORD_DiscountPercent", Convert.ToDecimal(_ItemList[0].Credit));
                        cmd.Parameters.AddWithValue("@inORD_Status", "N");
                        cmd.Parameters.AddWithValue("@inORD_TotalAmt", Convert.ToDecimal(_ItemList[0].TotalAmt));
                        cmd.Parameters.AddWithValue("@inORD_Type", _ItemList[0].Ordertype);
                        cmd.Parameters.AddWithValue("@inORDMOD_Type", Modtype);
                        cmd.Parameters.AddWithValue("@inORDMOD_Date", Moddate);
                        cmd.Parameters.AddWithValue("@inORDMOD_Time", Modtime);
                        cmd.Parameters.AddWithValue("@inShip_Code", codeShipping);
                        cmd.Parameters.AddWithValue("@inShip_Customer", CusShipping);
                        cmd.Parameters.AddWithValue("@Transport_Code", codetransportation);
                        cmd.Parameters.AddWithValue("@inSLMCODE", SLMCOD);
                        cmd.Parameters.AddWithValue("@inORD_TotalQty", sumqty);
                        cmd.Parameters.AddWithValue("@inORD_TotalItem", sumstk);
                        cmd.Parameters.AddWithValue("@inORD_TotalPrice", Convert.ToDecimal(ORD_TotalPrice));
                        cmd.Parameters.AddWithValue("@inORD_TotalDiscount", Convert.ToDecimal(ORD_TotalDiscount));
                        cmd.Parameters.AddWithValue("@inORD_Vat", sumvat);
                        cmd.Parameters.AddWithValue("@inPro_Discount", sumpro);
                        cmd.Parameters.AddWithValue("@inDeliveryDate", dateDelivery);
                        cmd.Parameters.AddWithValue("@inDeliveryTime", DeliveryTime);
                        cmd.Parameters.AddWithValue("@inRemark", Remark);
                        cmd.Parameters.AddWithValue("@inInsertBy", Xusrlogin);
                        cmd.Parameters.AddWithValue("@inTakeORDBy", takeorderby);
                        // cmd.Parameters.AddWithValue("@inCreditTerm", CreditTerm);
                        cmd.Parameters.AddWithValue("@inCreditTerm", "3");
                        cmd.Parameters.AddWithValue("@inCusPo", CusPo);
                        cmd.Parameters.AddWithValue("@intypecke", Convert.ToInt32(typecke));
                        cmd.Parameters.AddWithValue("@inORD_Totallabor", Convert.ToDecimal(totallabor));
                        cmd.Parameters.AddWithValue("@inORD_Totaltransportcost", Convert.ToDecimal(totaltransportcost));
                        SqlParameter returnValue = new SqlParameter("@outGenID", SqlDbType.Int);
                        returnValue.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(returnValue);
                        cmd.ExecuteNonQuery();
                        //S20161016
                        cmd.Dispose();
                        //E20161016
                        id = Convert.ToInt32(returnValue.Value);
                        int sop = 0;
                        string _str = string.Empty;
                        int intstock = 0;
                        string strstock = string.Empty;
                        string strFoc = string.Empty;
                        for (int i = 0; i < _ItemList.Count; i++)
                        {
                            idorder = Convert.ToInt32(_ItemList[i].Vidorder);
                            strFoc = _ItemList[i].Vtype;
                            _str = strFoc;

                            if (strFoc == "-")
                            {
                                _str = "";
                                sop = 0;
                            }
                            else if (strFoc == "SOP")
                            {
                                _str = strFoc;
                                sop = 1;

                            }
                            else if (strFoc == "FOC")
                            {
                                _str = strFoc;
                                sop = 0;
                            }
                            //Save รายการที่เป็นสินค้าขาย//
                            cmd = new SqlCommand("P_Save_CartConfirmTD_Temp_catalog", conn);
                            // cmd = new SqlCommand("P_Save_CartConfirmTD_Temp_Test", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@inORD_ID", id);
                            cmd.Parameters.AddWithValue("@inORD_STKCOD", _ItemList[i].VSTKCOD);
                            cmd.Parameters.AddWithValue("@inCompany", _ItemList[i].VCompany);
                            cmd.Parameters.AddWithValue("@inORD_STKGRP", _ItemList[i].VSTKGRP);
                            cmd.Parameters.AddWithValue("@inORD_Price", Convert.ToDecimal(_ItemList[i].VPrice));
                            cmd.Parameters.AddWithValue("@inORD_SalePrice", Convert.ToDecimal(_ItemList[i].VSalePrice));
                            cmd.Parameters.AddWithValue("@inORD_Qty", Convert.ToInt32(_ItemList[i].VQty));
                            cmd.Parameters.AddWithValue("@inORD_Amt", Convert.ToInt32(_ItemList[i].VQty) * Convert.ToDecimal(_ItemList[i].VSalePrice));
                            cmd.Parameters.AddWithValue("@inORD_UOM", _ItemList[i].Uom);
                            cmd.Parameters.AddWithValue("@inORD_Discount", _ItemList[i].VDiscount);
                            cmd.Parameters.AddWithValue("@inORD_LineNote", _ItemList[i].VLineNote);
                            cmd.Parameters.AddWithValue("@inORD_Promotion", _ItemList[i].VPromotion);
                            cmd.Parameters.AddWithValue("@inORD_SEQ", i + 1);
                            cmd.Parameters.AddWithValue("@inItem_Type", _str);
                            cmd.Parameters.AddWithValue("@sop", sop);
                            cmd.Parameters.AddWithValue("@inInsertBy", Xusrlogin);
                            cmd.Parameters.AddWithValue("@Backorder", Convert.ToInt32(_ItemList[i].Vbackorder));
                            cmd.Parameters.AddWithValue("@CartID", Convert.ToInt32(_ItemList[i].Vidorder));
                            cmd.Parameters.AddWithValue("@WH_Location", _ItemList[i].VWHLocation);
                            // cmd.Parameters.AddWithValue("@inPRCLST_NO", Convert.ToInt32(_ItemList[i].PRCLST_NO));
                            cmd.Parameters.AddWithValue("@inSO", Docorder);
                            cmd.ExecuteNonQuery();
                            //S20161016
                            cmd.Dispose();
                            //E20161016
                            message = "true";
                        }
                        cmd = new SqlCommand("p_Chk_Promotion", conn);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@inORD_ID", id);
                        SqlParameter returnValueProcheck = new SqlParameter("@outGenID", SqlDbType.Int);
                        returnValueProcheck.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(returnValueProcheck);
                        SqlDataReader rev_ = cmd.ExecuteReader();
                        while (rev_.Read())
                        {
                            Err_Flg = rev_["Err_Flg"].ToString();
                            Err_Message = rev_["err_message"].ToString();
                            Prodiscount = rev_["PM_Qty"].ToString();
                            // IDProc = rev_["ID"].ToString();

                        }
                        //rev_.Dispose();
                        //S20161016
                        rev_.Close();
                        rev_.Dispose();
                        cmd.Dispose();
                        //E20161016
                        Connection.Close();


                    }
                    catch (Exception ex)
                    {
                        message = "false";
                        if (trans != null)
                        {
                            trans.Rollback();
                        }
                        //return -1;
                    }
                    finally
                    {
                        if (conn != null)
                        {
                            conn.Close();
                        }
                    }


                }
            }


            return Json(new { message, DocNo, Err_Flg, Err_Message, IDProc, id, Prodiscount }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConfirmationdatacusQuatation(string typecke, string S_dis, string QNo, string Ts_dis, string CusPo, string CreditTerm, string takeorderby, string Remark, string codetransportation, string CusShipping, string codeShipping, string Moddate, string Modtime, string Modtype, string Xusrlogin, string Xcus, string XSul, string DataSend, string DataSendPro, string sumvat, string sumpro, string ORD_TotalPrice, string ORD_TotalDiscount, string sumqty, string sumstk, string dateDelivery, string DeliveryTime)
        {
            List<ItemConfirm> _ItemList = new JavaScriptSerializer().Deserialize<List<ItemConfirm>>(DataSend);
            List<ItemConfirmPro> _ItemListPro = new JavaScriptSerializer().Deserialize<List<ItemConfirmPro>>(DataSendPro);
            string message = "false";
            string Docorder = string.Empty;
            string Err_Flg = string.Empty;
            string Err_Message = string.Empty;
            string IDProc = string.Empty;
            string Prodiscount = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();

            // int seqmax= 0;
            int id = 0;
            int idorder = 0;
            string docgen = string.Empty;
            string DocNo = string.Empty;
            string GenDoc_Quatation = string.Empty;

            Connection.Close();
            if (_ItemList.Count > 0)
            {
                string CUSID = Xcus;
                string SLMCOD = XSul;
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
                {
                    conn.Open();
                    SqlTransaction trans = null;
                    try
                    {
                        SqlCommand cmd = new SqlCommand("P_Save_OrderConfirmTH_Quatation", conn);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@inORD_DocNo", Docorder);
                        cmd.Parameters.AddWithValue("@inORD_DocNo", "");
                        //cmd.Parameters.AddWithValue("@inORD_Date", _ItemList[0].VORDDAT);
                        cmd.Parameters.AddWithValue("@inORD_Date", DateTime.Now.ToString("dd/MM/yyy"));
                        cmd.Parameters.AddWithValue("@inCUSCOD", CUSID);
                        cmd.Parameters.AddWithValue("@inCompany", _ItemList[0].VCompany);
                        cmd.Parameters.AddWithValue("@inORD_DiscountAmt", Convert.ToDecimal(_ItemList[0].AmtDiscount));
                        cmd.Parameters.AddWithValue("@inORD_DiscountPercent", Convert.ToDecimal(_ItemList[0].Credit));
                        cmd.Parameters.AddWithValue("@inORD_Status", "N");
                        cmd.Parameters.AddWithValue("@inORD_TotalAmt", Convert.ToDecimal(_ItemList[0].TotalAmt));
                        cmd.Parameters.AddWithValue("@inORD_Type", _ItemList[0].Ordertype);
                        cmd.Parameters.AddWithValue("@inORDMOD_Type", Modtype);
                        cmd.Parameters.AddWithValue("@inORDMOD_Date", Moddate);
                        cmd.Parameters.AddWithValue("@inORDMOD_Time", Modtime);
                        cmd.Parameters.AddWithValue("@inShip_Code", codeShipping);
                        cmd.Parameters.AddWithValue("@inShip_Customer", CusShipping);
                        cmd.Parameters.AddWithValue("@Transport_Code", codetransportation);
                        cmd.Parameters.AddWithValue("@inSLMCODE", SLMCOD);
                        cmd.Parameters.AddWithValue("@inORD_TotalQty", Convert.ToInt32(sumqty)); //26002
                        cmd.Parameters.AddWithValue("@inORD_TotalItem", Convert.ToInt32(sumstk)); //12
                        //cmd.Parameters.AddWithValue("@inORD_TotalPrice", Convert.ToDecimal(ORD_TotalPrice)); //26686284.30
                        cmd.Parameters.AddWithValue("@inORD_TotalPrice", Convert.ToDecimal(0)); //26686284.30
                        cmd.Parameters.AddWithValue("@inORD_TotalDiscount", Convert.ToDecimal(ORD_TotalDiscount));
                        //cmd.Parameters.AddWithValue("@inORD_Vat", Convert.ToDecimal(sumvat));   //1868039.90
                        cmd.Parameters.AddWithValue("@inORD_Vat", Convert.ToDecimal(0));   //1868039.90
                        cmd.Parameters.AddWithValue("@inPro_Discount", Convert.ToDecimal(sumpro));
                        cmd.Parameters.AddWithValue("@inDeliveryDate", dateDelivery);
                        cmd.Parameters.AddWithValue("@inDeliveryTime", DeliveryTime);
                        cmd.Parameters.AddWithValue("@inRemark", Remark);
                        cmd.Parameters.AddWithValue("@inInsertBy", Xusrlogin);
                        cmd.Parameters.AddWithValue("@inTakeORDBy", takeorderby);
                        cmd.Parameters.AddWithValue("@inCreditTerm", CreditTerm);
                        cmd.Parameters.AddWithValue("@inCusPo", "");
                        cmd.Parameters.AddWithValue("@inSpecial_Discount", Convert.ToDecimal(S_dis));
                        cmd.Parameters.AddWithValue("@inType_Cal", Ts_dis);
                        cmd.Parameters.AddWithValue("@inQno", QNo);
                        cmd.Parameters.AddWithValue("@intypecke", Convert.ToInt32(typecke));
                        SqlParameter returnValue = new SqlParameter("@outGenID", SqlDbType.Int);
                        returnValue.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(returnValue);
                        SqlParameter returnValue_Quatation = new SqlParameter("@outGenDoc_Quatation", SqlDbType.VarChar, 50);
                        returnValue_Quatation.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(returnValue_Quatation);
                        cmd.ExecuteNonQuery();
                        //S20161016
                        GenDoc_Quatation = Convert.ToString(cmd.Parameters["@outGenDoc_Quatation"].Value);
                        cmd.Dispose();
                        //E20161016
                        id = Convert.ToInt32(returnValue.Value);
                        // GenDoc_Po = Convert.ToString(returnValue_Po);
                        //  Convert.ToString(returnValue_Po);
                        int sop = 0;
                        string _str = string.Empty;
                        // int intstock = 0;
                        string strstock = string.Empty;
                        string strFoc = string.Empty;
                        string _gen = string.Empty;
                        int _genId = 0;
                        for (int i = 0; i < _ItemList.Count; i++)
                        {
                            idorder = Convert.ToInt32(_ItemList[i].Vidorder);
                            strFoc = _ItemList[i].Vtype;
                            _str = strFoc;
                            _gen = _ItemList[i].VGenID;
                            if (_gen != "")
                            {
                                _genId = Convert.ToInt32(_ItemList[i].VGenID);
                            }
                            else
                            {
                                _genId = 0;
                            }
                            if (strFoc == "-")
                            {
                                _str = "";
                                sop = 0;
                            }
                            else if (strFoc == "SOP")
                            {
                                _str = strFoc;
                                sop = 1;

                            }
                            else if (strFoc == "FOC")
                            {
                                _str = strFoc;
                                sop = 0;
                            }
                            //Save รายการที่เป็นสินค้าขาย//
                            cmd = new SqlCommand("P_Save_CartConfirmTD_Quatation", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@inORD_ID", id);
                            cmd.Parameters.AddWithValue("@GenID", _genId);
                            cmd.Parameters.AddWithValue("@inORD_STKCOD", _ItemList[i].VSTKCOD);
                            cmd.Parameters.AddWithValue("@inCompany", _ItemList[i].VCompany);
                            cmd.Parameters.AddWithValue("@inORD_STKGRP", _ItemList[i].VSTKGRP);
                            cmd.Parameters.AddWithValue("@inORD_Price", Convert.ToDecimal(_ItemList[i].VPrice));
                            cmd.Parameters.AddWithValue("@inORD_SalePrice", Convert.ToDecimal(_ItemList[i].VSalePrice));
                            cmd.Parameters.AddWithValue("@inORD_Qty", Convert.ToInt32(_ItemList[i].VQty));
                            cmd.Parameters.AddWithValue("@inORD_Amt", Convert.ToInt32(_ItemList[i].VQty) * Convert.ToDecimal(_ItemList[i].VSalePrice));
                            cmd.Parameters.AddWithValue("@inORD_UOM", _ItemList[i].Uom);
                            cmd.Parameters.AddWithValue("@inORD_Discount", _ItemList[i].VDiscount);
                            cmd.Parameters.AddWithValue("@inORD_LineNote", _ItemList[i].VLineNote);
                            cmd.Parameters.AddWithValue("@inORD_Promotion", _ItemList[i].VPromotion);
                            cmd.Parameters.AddWithValue("@inORD_SEQ", i + 1);
                            cmd.Parameters.AddWithValue("@inItem_Type", _str);
                            cmd.Parameters.AddWithValue("@sop", sop);
                            cmd.Parameters.AddWithValue("@inInsertBy", Xusrlogin);
                            cmd.Parameters.AddWithValue("@Backorder", Convert.ToInt32(_ItemList[i].Vbackorder));
                            // cmd.Parameters.AddWithValue("@inPRCLST_NO", Convert.ToInt32(_ItemList[i].PRCLST_NO));
                            cmd.Parameters.AddWithValue("@inSO", GenDoc_Quatation);
                            cmd.Parameters.AddWithValue("@cartid", Convert.ToInt32(_ItemList[i].Vidorder));
                            cmd.ExecuteNonQuery();
                            //S20161016
                            cmd.Dispose();
                            //E20161016
                            message = "true";
                        }


                        Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        message = "false" + ex.Message;
                        if (trans != null)
                        {
                            trans.Rollback();
                        }
                        //return -1;
                    }
                    finally
                    {
                        if (conn != null)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            return Json(new { message, GenDoc_Quatation, Err_Flg, Err_Message, IDProc, id, Prodiscount }, JsonRequestBehavior.AllowGet);
        }


        // public JsonResult Confirmationdata(string company, string ordidtemp, string takeorderby, string Remark, string codetransportation, string CusShipping, string codeShipping, string Moddate, string Modtime, string Modtype, string Xusrlogin, string Xcus, string XSul, string DataSend, string DataSendPro, string sumvat, string sumpro, string ORD_TotalPrice, string ORD_TotalDiscount, string sumqty, string sumstk, string dateDelivery, string DeliveryTime)
        public JsonResult Confirmationdata(string DataSend, string Xusrlogin, string ordidtemp)
        {

            List<ItemConfirm> _ItemList = new JavaScriptSerializer().Deserialize<List<ItemConfirm>>(DataSend);

            string message = "false";
            string Docorder = string.Empty;


            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // int seqmax= 0;
            int id = 0;
            int idorder = 0;
            string docgen = string.Empty;
            string DocNo = string.Empty;
            string ItemListORD_ID = string.Join(",", _ItemList.Select(item => item.Vidorder));
            //message = ItemListORD_ID;

            if (_ItemList.Count > 0)
            {

                // string CUSID = Xcus;
                //string SLMCOD = XSul;
                string com = _ItemList[0].VCompany;
                //string com = "TAC";
                SqlTransaction trans = null;
                try
                {
                    SqlCommand cmd = new SqlCommand("P_Save_OrderConfirmTH_catalog_All", conn);
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inORD_ID", ordidtemp);
                    cmd.Parameters.AddWithValue("@inCompany", com);
                    cmd.Parameters.AddWithValue("@inUser", Xusrlogin);
                    cmd.Parameters.AddWithValue("@inOrderListID", ItemListORD_ID);
                    SqlParameter returnValue = new SqlParameter("@outGenID", SqlDbType.Int);
                    returnValue.Direction = System.Data.ParameterDirection.Output;
                    SqlParameter p = new SqlParameter("@OutGenstatus", SqlDbType.NVarChar, 100);
                    p.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(returnValue);
                    cmd.Parameters.Add(p);

                    using (SqlDataReader rev_ = cmd.ExecuteReader())
                    {
                        while (rev_.Read())
                        {
                            // ตรวจสอบชื่อคอลัมน์ที่แน่นอน
                            DocNo = rev_["Sale Order"].ToString();

                        }
                    } // ปิด reader อัตโนมัติ
                    message = p.Value.ToString();
                    cmd.Dispose();

                    //Comment
                    /*

                    for (int i = 0; i < _ItemList.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(DocNo))
                        {
                            SqlCommand cmdupdate = new SqlCommand("P_UpdateStatus_Ordering_Cart", conn);
                            cmdupdate.Connection = conn;
                            cmdupdate.CommandType = CommandType.StoredProcedure;
                            cmdupdate.Parameters.AddWithValue("@inCart_ID", _ItemList[i].Vidorder);
                            //cmdupdate.Parameters.AddWithValue("@inCart_ID", null);
                            cmdupdate.Parameters.AddWithValue("@inUser", Xusrlogin);
                            cmdupdate.Parameters.AddWithValue("@inDocNo", DocNo);
                            //cmdupdate.Parameters.Add(returnValue);
                            cmdupdate.ExecuteReader();
                            cmdupdate.Dispose();
                        }
                    }

                    */

                    conn.Close();
                    //}
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    if (trans != null)
                    {
                        trans.Rollback();
                    }
                    //return -1;
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
                //}
            }
            return Json(new { message, DocNo }, JsonRequestBehavior.AllowGet);
        }
        //เช็ค last id LogAccessCart ก่อน confirm
        public JsonResult checkAccessIdConfirm(string accessId, string cuscod)
        {
            string flagCheck = string.Empty;
            string userLast = string.Empty;
            string timeBefore = string.Empty;
            string timeLast = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Get_Access_Id_Last", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@accessId", accessId);
            command.Parameters.AddWithValue("@cuscod", cuscod);
            SqlParameter returnFlag = new SqlParameter("@outResult", SqlDbType.NVarChar, 100);
            returnFlag.Direction = System.Data.ParameterDirection.Output;
            command.Parameters.Add(returnFlag);

            SqlParameter returnUserLast = new SqlParameter("@outUser", SqlDbType.NVarChar, 100);
            returnUserLast.Direction = System.Data.ParameterDirection.Output;
            command.Parameters.Add(returnUserLast);

            SqlParameter returnTimeBefore = new SqlParameter("@outTimeUserBefore", SqlDbType.NVarChar, 100);
            returnTimeBefore.Direction = System.Data.ParameterDirection.Output;
            command.Parameters.Add(returnTimeBefore);

            SqlParameter returnTimeLast = new SqlParameter("@outTimeUserLast", SqlDbType.NVarChar, 100);
            returnTimeLast.Direction = System.Data.ParameterDirection.Output;
            command.Parameters.Add(returnTimeLast);

            Connection.Open();
            command.ExecuteNonQuery();
            flagCheck = returnFlag.Value.ToString();
            userLast = returnUserLast.Value.ToString();
            timeBefore = returnTimeBefore.Value.ToString();
            timeLast = returnTimeLast.Value.ToString();

            command.Dispose();
            Connection.Close();

            return Json(new { flagCheck, userLast, timeBefore, timeLast }, JsonRequestBehavior.AllowGet);
        }
        //เช็ค credit ลูกค้าก่อน confirm
        public JsonResult CheckCustomerCredit(string company, string cuscod, string sumAmt)
        {
            string flagCheck = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("p_CheckCustomerCredit", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@InCompany", company);
            command.Parameters.AddWithValue("@InCuscod", cuscod);
            command.Parameters.AddWithValue("@InOrderAmt", sumAmt);
            SqlParameter returnFlag = new SqlParameter("@outResult", SqlDbType.NVarChar, 100);
            returnFlag.Direction = System.Data.ParameterDirection.Output;
            command.Parameters.Add(returnFlag);
            Connection.Open();
            command.ExecuteNonQuery();
            flagCheck = returnFlag.Value.ToString();

            command.Dispose();
            Connection.Close();

            return Json(new { flagCheck }, JsonRequestBehavior.AllowGet);
        }

        //เช็ค Quota พนักงาน 
        public JsonResult CheckEmployeeQuota(string Cuscod, string Shipto, string Amt)
        {
            string flagCheck = string.Empty;
            string allAmt = string.Empty;
            string quotaEmp = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            try
            {
                var command = new SqlCommand("P_CheckStaffQuota", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@InCuscod", Cuscod);
                command.Parameters.AddWithValue("@InShipto", Shipto);
                command.Parameters.AddWithValue("@BuyAmt", Amt);
                SqlParameter returnFlag = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                returnFlag.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnFlag);

                SqlParameter returnAmt = new SqlParameter("@outAmtAll", SqlDbType.NVarChar, 100);
                returnAmt.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnAmt);

                SqlParameter returnQuota = new SqlParameter("@outQuota", SqlDbType.NVarChar, 100);
                returnQuota.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnQuota);

                Connection.Open();
                command.ExecuteNonQuery();
                flagCheck = returnFlag.Value.ToString();
                allAmt = returnAmt.Value.ToString();
                quotaEmp = returnQuota.Value.ToString();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                flagCheck = ex.Message;
            }


            return Json(new { flagCheck, allAmt, quotaEmp }, JsonRequestBehavior.AllowGet);
        }
    }
}
