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
using System.DirectoryServices.Protocols;
using System.Web.Services.Description;

namespace NewShopTAM.Controllers
{
    public class PromotionController : Controller
    {

        //
        // GET: /Promotion/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RegisterCustomer()
        {
           // string appEnv = Session["appEnv"].ToString();
            string user = Session["UserID"].ToString();
            string slmCodeDefault = Session["UserID"].ToString();
            string flagSup = string.Empty;

            using (SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
            {
                Connection.Open();
                if (slmCodeDefault != null)
                {
                    SqlCommand cmd1 = new SqlCommand("select TOP 1 * From v_SLMTAB_SM_Userrid where SUP = N'" + slmCodeDefault + "'", Connection);
                    SqlDataReader rev = cmd1.ExecuteReader();
                    while (rev.Read())
                    {
                        flagSup = rev["SUP"].ToString();
                    }
                    rev.Close();
                    rev.Dispose();
                    cmd1.Dispose();
                }
            }

            ViewBag.slmCodeList = GetSalesmanName(user);
            ViewBag.pmCodeList = GetProductName("");
            //ViewBag.slmCode = slmCode == null ? slmCodeDefault : slmCode;
            ViewBag.flagSup = flagSup;

            return View();
        }
        //get name sales
        public List<SelectListItem> GetSalesmanName(string slmCode)
        {
            List<SelectListItem> slmCodeList = new List<SelectListItem>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();

            var command = new SqlCommand("P_Chk_user", Connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UsrID", slmCode);
            command.Parameters.AddWithValue("@Password", "");
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                slmCodeList.Add(new SelectListItem() { Value = dr["SLMCOD"].ToString(), Text = dr["SLMCOD"].ToString() + "/" + dr["SLMNAM"].ToString() });
            }
            dr.Close();
            dr.Dispose();

            Connection.Dispose();
            command.Dispose();
            Connection.Close();

            return slmCodeList;
        }
        public List<SelectListItem> GetProductName(string User)
        {
            List<SelectListItem> productList = new List<SelectListItem>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();

            var command = new SqlCommand("P_Search_Product", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inUser", User);
            command.ExecuteNonQuery();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                productList.Add(new SelectListItem() { Value = dr["PROD"].ToString(), Text = dr["PROD"].ToString() + "/" + dr["PRODNAM"].ToString() });

            }
            dr.Close();
            dr.Dispose();

            Connection.Dispose();
            command.Dispose();
            Connection.Close();

            return productList;
        }
        //get PromotionCode By Pmcode
        public JsonResult GetPromotion(string prodMgr, string year, string company, string period)
        {
            List<listPromotionCode> promotionList = new List<listPromotionCode>();
            SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString);
            Connection.Open();
            var command = new SqlCommand("P_Search_Promotioncode", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inPm", prodMgr);
            command.Parameters.AddWithValue("@inYear", year);
            command.Parameters.AddWithValue("@inCom", company);
            command.Parameters.AddWithValue("@inPeriod", period);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                promotionList.Add(new listPromotionCode()
                {
                    PromotionCode = dr["Promotion_code"].ToString(),
                    PromotionDes = dr["Description"].ToString()
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return Json(promotionList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPromotionDetail(string promotionCode, string slmCode)
        {
            string message = "";
            int countList = 0;
            List<listPromotionDetail> promotionDetailList = new List<listPromotionDetail>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Search_Promotion_Detail", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inSlmCode", slmCode);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ++countList;
                    if (countList == 1) {
                        @ViewBag.RegisterExpireDate = reader["RegisterExpireDate"].ToString();
                    }
                    promotionDetailList.Add(new listPromotionDetail()
                    {
                        Promotion_Code = reader["Promotion_Code"].ToString(),
                        Seq = reader["Seq"].ToString(),
                        Type = reader["Type"].ToString(),
                        DesType = reader["DesType"].ToString(),
                        Description = reader["Description"].ToString(),
                        Condition = reader["Condition"].ToString(),
                        Reward = reader["Reward"].ToString(),
                        Reward_Percent = reader["Reward_Percent"].ToString(),
                        Count_reg = reader["Count_reg"].ToString()
                    });
                }
                //if (Getdata.Any())
                //{
                //    //get data WH, Round, PIDate, StartDelivery, ExptoArrive
                //    foreach (var rowData in Getdata)
                //    {
                //    }
                //}
                @ViewBag.promotionDetailList = promotionDetailList;
                reader.Close();
                command.Dispose();
                Connection.Close();
                message = "Y";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            @ViewBag.messageError = message;
            return PartialView("_DetailPromotionRegister", new
            {
                @ViewBag.promotionDetailList,
                @ViewBag.RegisterExpireDate,
                @ViewBag.messageError
            });
        }
        public ActionResult GetCustomerByPromotion(string slmCode, string promotionCode, string promotionSeq, string textHeader)
        {
            int countCustomer = 0;
            int countCustomerReg = 0;
            string message = "Y";
            string customerMasterList = string.Empty;
            List<listCustomerRegister> customerRegisterList = new List<listCustomerRegister>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Search_Customer_Register", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inSlmCode", slmCode);
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inPromotionSeq", promotionSeq);
                command.Parameters.AddWithValue("@inCuscodeSearch", "ALL");
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ++countCustomer;
                    if (!string.IsNullOrEmpty(reader["FLAG_REG"].ToString()))
                    {
                        ++countCustomerReg;
                    }
                    customerRegisterList.Add(new listCustomerRegister()
                    {
                        Cuscode = reader["CUSCOD"].ToString(),
                        Cusname = reader["CUSNAM"].ToString(),
                        Slmcode = reader["SLMCOD"].ToString(),
                        FlagReg = reader["FLAG_REG"].ToString(),
                        FlagApprove = reader["FLAG_APPROVE"].ToString(),
                        PackQty = reader["PackQty"] != DBNull.Value ? Convert.ToInt32(reader["PackQty"]) : 0,
                        PackAmt = reader["PackAmt"] != DBNull.Value ? Convert.ToInt32(reader["PackAmt"]) : 0,
                        SumPackAmt = reader["SumPackAmt"] != DBNull.Value ? Convert.ToDouble(reader["SumPackAmt"]) : 0.0,
                        PackQtyPedingApprove = reader["PackQtyPedingApprove"].ToString(),

                    });
                }
                @ViewBag.customerRegisterList = customerRegisterList;
                //ดึงรายชื่อร้านค้า
                reader.Close();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            @ViewBag.messageError = message;
            @ViewBag.countCustomer = countCustomer;
            @ViewBag.countCustomerReg = countCustomerReg;
            @ViewBag.textHeader = textHeader;
            @ViewBag.promotionCode = promotionCode;
            @ViewBag.promotionSeq = promotionSeq;
            @ViewBag.customerMasterList = GetCustpmerBySlmcode(slmCode); 
            return PartialView("_ListCustomer", new
            {
                @ViewBag.customerRegisterList,
                @ViewBag.customerMasterList,
                @ViewBag.messageError,
                @ViewBag.textHeader,
                @ViewBag.countCustomer,
                @ViewBag.countCustomerReg,
                @ViewBag.promotionCode,
                @ViewBag.promotionSeq
            });
        }
        public ActionResult GetSubCustomerByPromotion(string slmCode, string promotionCode, string promotionSeq, string cusCodeSearch)
        {
            int countCustomer = 0;
            int countCustomerReg = 0;
            string message = "Y";
            string customerMasterList = string.Empty;
            List<listCustomerRegister> customerRegisterList = new List<listCustomerRegister>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Search_Customer_Register", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inSlmCode", slmCode);
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inPromotionSeq", promotionSeq);
                command.Parameters.AddWithValue("@inCuscodeSearch", cusCodeSearch);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ++countCustomer;
                    if (!string.IsNullOrEmpty(reader["FLAG_REG"].ToString()))
                    {
                        ++countCustomerReg;
                    }
                    customerRegisterList.Add(new listCustomerRegister()
                    {
                        Cuscode = reader["CUSCOD"].ToString(),
                        Cusname = reader["CUSNAM"].ToString(),
                        Slmcode = reader["SLMCOD"].ToString(),
                        FlagReg = reader["FLAG_REG"].ToString(),
                        FlagApprove = reader["FLAG_APPROVE"].ToString(),
                        PackQtyPedingApprove = reader["PackQtyPedingApprove"].ToString(),
                    });
                }
                @ViewBag.customerRegisterList = customerRegisterList;
                //ดึงรายชื่อร้านค้า
                reader.Close();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            @ViewBag.messageError = message;
            @ViewBag.countCustomer = countCustomer;
            @ViewBag.countCustomerReg = countCustomerReg;
            //@ViewBag.textHeader = textHeader;
            @ViewBag.customerMasterList = GetCustpmerBySlmcode(slmCode);
            return PartialView("_SubListCustomer", new
            {
                @ViewBag.customerRegisterList,
                @ViewBag.customerMasterList,
                @ViewBag.messageError,
                //@ViewBag.textHeader,
                @ViewBag.countCustomer,
                @ViewBag.countCustomerReg
            });
        }
        [HttpPost]
        public ActionResult SaveCustomerRegister(string user, string slmCode, string cusCode, string promotionCode, string promotionSeq, string packQty, string packAmt, string sumPackAmt, string txtReason)
        {
            int numSuccess = 0;
            int numError = 0;
            string message = "Y";
            //string cusCodArr = "";
            //if (cusCode != null)
            //{
            //    cusCodArr = String.Join(",", cusCode.Select(s => "" + s + ""));
            //}
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            try
            {
                Connection.Open();
                //foreach (var listData in (List<listCustomerRegisterSave>)request)
                //{
                var command = new SqlCommand("P_Save_Customer_Register", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inUser", user);
                command.Parameters.AddWithValue("@inSlmCode", slmCode);
                command.Parameters.AddWithValue("@inCustomerCode", cusCode);
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inPromotionSeq", Convert.ToInt32(promotionSeq));
                command.Parameters.AddWithValue("@inPackQty", Convert.ToInt32(packQty));
                command.Parameters.AddWithValue("@inPackAmt", packAmt);
                command.Parameters.AddWithValue("@inSumPackAmt", sumPackAmt);
                command.Parameters.AddWithValue("@inReason", txtReason);
                SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                returnValue.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnValue);
                command.ExecuteNonQuery();
                if (!string.IsNullOrEmpty(returnValue.Value.ToString())) { 
                    message = returnValue.Value.ToString();
                }
                //if (message == "Y")
                //{
                //    ++numSuccess;
                //}
                //else
                //{
                //    ++numError;
                //}
                command.Dispose();
                //}
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                Connection.Close();
            }
            return Json(new { status = message, numSuccess = numSuccess, numError = numError }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ApproveChangeCustomer()
        {
            string user = Session["UserID"] as string;
            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("LogIn", "Account");
            }
            string flagSup = string.Empty;
            List<SelectListItem> slmCodeList = new List<SelectListItem>();
            List<SelectListItem> productList = new List<SelectListItem>();
            using (SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString))
            {
                Connection.Open();
                //get salesman
                var command = new SqlCommand("P_Price_Approve_Data", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inUsrID", user);
                command.Parameters.AddWithValue("@inType", 2);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    slmCodeList.Add(new SelectListItem() { Value = dr["SLMCOD"].ToString(), Text = dr["SLMCOD"].ToString() + "/" + dr["SLMNAM"].ToString() });
                }
                dr.Close();
                command.Dispose();

                command = new SqlCommand("P_Price_Approve_Data", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inUsrID", user);
                command.Parameters.AddWithValue("@inType", 4);
                SqlDataReader dr2 = command.ExecuteReader();
                while (dr2.Read())
                {
                    productList.Add(new SelectListItem() { Value = dr2["PROD"].ToString(), Text = dr2["PROD"].ToString() + "/" + dr2["PRODNAM"].ToString() });
                }
                dr2.Close();
                command.Dispose();
            }

            ViewBag.slmCodeList = slmCodeList;
            ViewBag.pmCodeList = productList;
            //ViewBag.slmCode = slmCode == null ? slmCodeDefault : slmCode;
            ViewBag.flagSup = flagSup;

            return View();
        }
        //get all tab
        public ActionResult GetManagePackPromotion(string slmCode, string customerCode, string customerName, string promotionCode, string promotionSeq)
        {
            @ViewBag.slmCode = slmCode;
            @ViewBag.customerCode = customerCode;
            @ViewBag.customerName = customerName;
            @ViewBag.promotionCode = promotionCode;
            @ViewBag.promotionSeq = promotionSeq;

            return PartialView("_ManagePackPromotion", new
            {
                @ViewBag.slmCode,
                @ViewBag.customerCode,
                @ViewBag.promotionCode,
                @ViewBag.promotionSeq,
                @ViewBag.customerName
            });
        }
        //get table list pack by customer 
        public ActionResult GetListPackPromotionByCustomerCurrent(string customerCode, string promotionCode, string promotionSeq)
        {
            List<listCustomerRegister> customerRegisterList = new List<listCustomerRegister>();
            if (!string.IsNullOrEmpty(customerCode) && !string.IsNullOrEmpty(promotionCode) && !string.IsNullOrEmpty(promotionSeq))
            {
                customerRegisterList = GetListPackPromotionByCustomer(customerCode, promotionCode, "");
            }
            @ViewBag.customerRegisterList = customerRegisterList;
            return PartialView("_ListPackPromotionByCustomer", new
            {
                @ViewBag.customerRegisterList
            });
        }
        //ดึงข้อมูลของ CustomerCode ตาม Seq
        public ActionResult GetManagePackPromotionByCustomer(string customerCode, string promotionCode, string promotionSeq)
        {
            int countCustomer = 0;
            int countCustomerReg = 0;
            string message = "Y";
            string customerMasterList = string.Empty;
            string customerCodeBag = string.Empty;
            string customerNameBag = string.Empty;
            string slmCode = string.Empty;
            string flagReg = string.Empty;
            string flagApprove = string.Empty;
            string packQtyPedingApprove = string.Empty;
            int packQty = 0;
            int packAmt = 0;
            Double sumPackAmt = 0;

            List<listCustomerRegister> customerRegisterList = new List<listCustomerRegister>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Search_Customer_Register", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inSlmCode", "");
                command.Parameters.AddWithValue("@inCuscodeSearch", customerCode);
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inPromotionSeq", promotionSeq);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ++countCustomer;
                    if (!string.IsNullOrEmpty(reader["FLAG_REG"].ToString()))
                    {
                        ++countCustomerReg;
                    }
                    customerCodeBag = reader["CUSCOD"].ToString();
                    customerNameBag = reader["CUSNAM"].ToString();
                    slmCode = reader["SLMCOD"].ToString();
                    flagReg = reader["FLAG_REG"].ToString();
                    flagApprove = reader["FLAG_APPROVE"].ToString();
                    packQty = reader["PackQty"] != DBNull.Value ? Convert.ToInt32(reader["PackQty"]) : 0;
                    packAmt = reader["PackAmt"] != DBNull.Value ? Convert.ToInt32(reader["PackAmt"]) : 0;
                    sumPackAmt = reader["SumPackAmt"] != DBNull.Value ? Convert.ToDouble(reader["SumPackAmt"]) : 0.0;
                    if (packQty == 1 && sumPackAmt == 0.0)
                    {
                        sumPackAmt = packAmt;
                    }
                    packQtyPedingApprove = reader["PackQtyPedingApprove"].ToString();

                }
                reader.Close();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (!string.IsNullOrEmpty(customerCode) && !string.IsNullOrEmpty(promotionCode) && !string.IsNullOrEmpty(promotionSeq)) { 
                customerRegisterList = GetListPackPromotionByCustomer(customerCode, promotionCode, "");
            }
            @ViewBag.customerRegisterList = customerRegisterList;
            @ViewBag.customerCodeBag = customerCodeBag;
            @ViewBag.customerNameBag = customerNameBag;
            @ViewBag.promotionCode = promotionCode;
            @ViewBag.promotionSeq = promotionSeq;
            @ViewBag.slmCode = slmCode;
            @ViewBag.flagReg = flagReg;
            @ViewBag.flagApprove = flagApprove;
            @ViewBag.packQty = packQty;
            @ViewBag.packAmt = packAmt;
            @ViewBag.sumPackAmt = sumPackAmt;
            @ViewBag.packQtyPedingApprove = packQtyPedingApprove;
            @ViewBag.countCustomer = countCustomer;
            @ViewBag.countCustomerReg = countCustomerReg;
            @ViewBag.messageError = message;
            return PartialView("_ManagePackPromotionBySeq", new
            {
                @ViewBag.customerRegisterList,
                @ViewBag.customerCodeBag,
                @ViewBag.customerNameBag, 
                @ViewBag.promotionCode, 
                @ViewBag.promotionSeq, 
                @ViewBag.slmCode,
                @ViewBag.flagReg,
                @ViewBag.flagApprove,
                @ViewBag.packQty,
                @ViewBag.packAmt,
                @ViewBag.sumPackAmt,
                @ViewBag.packQtyPedingApprove,
                @ViewBag.countCustomer,
                @ViewBag.countCustomerReg,
                @ViewBag.messageError
            });
        }
        ///ดึง list promotion ทั้งหมดที่ลูกค้านี้ลงใน PromotionCode นี้
        public List<listCustomerRegister> GetListPackPromotionByCustomer(string customerCode, string promotionCode, string promotionSeq)
        {
            string message = "Y";
            string customerMasterList = string.Empty;
            List<listCustomerRegister> customerRegisterList = new List<listCustomerRegister>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Search_Customer_Register", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inSlmCode", "");
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inPromotionSeq", promotionSeq);
                command.Parameters.AddWithValue("@inCuscodeSearch", customerCode);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    customerRegisterList.Add(new listCustomerRegister()
                    {
                        Cuscode = reader["CUSCOD"].ToString(),
                        Cusname = reader["CUSNAM"].ToString(),
                        Slmcode = reader["SLMCOD"].ToString(),
                        FlagReg = reader["FLAG_REG"].ToString(),
                        FlagApprove = reader["FLAG_APPROVE"].ToString(),
                        PackQty = reader["PackQty"] != DBNull.Value ? Convert.ToInt32(reader["PackQty"]) : 0,
                        PackAmt = reader["PackAmt"] != DBNull.Value ? Convert.ToInt32(reader["PackAmt"]) : 0,
                        SumPackAmt = reader["SumPackAmt"] != DBNull.Value ? Convert.ToDouble(reader["SumPackAmt"]) : 0.0,
                        PackQtyPedingApprove = reader["PackQtyPedingApprove"].ToString(),
                        Reason = reader["Reason"].ToString(),
                    });
                }
                reader.Close();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return customerRegisterList;
        }
        //get table list pack by promotion code all customer of slmcode
        public ActionResult GetCustomerByPromotionCode(string customerCode, string promotionCode, string promotionSeq, string slmCode)
        {
            List<listCustomerRegisterByCustomer> customerByPromotionCodeList = new List<listCustomerRegisterByCustomer>();
            if (!string.IsNullOrEmpty(promotionCode))
            {
                customerByPromotionCodeList = GetListPackPromotionByCustomerAllPm(customerCode, promotionCode, promotionSeq, slmCode);
            }
            @ViewBag.customerByPromotionCodeList = customerByPromotionCodeList;
            return PartialView("_ListCustomerByPromotionCode", new
            {
                @ViewBag.customerByPromotionCodeList
            });
        }
        //get table list pack by customer all pm
        public ActionResult GetPackPromotionByCustomerCurrentAllPm(string customerCode, string promotionCode, string promotionSeq, string slmCode)
        {
            List<listCustomerRegisterByCustomer> customerRegisterAllPmList = new List<listCustomerRegisterByCustomer>();
            if (!string.IsNullOrEmpty(customerCode))
            {
                customerRegisterAllPmList = GetListPackPromotionByCustomerAllPm(customerCode, promotionCode, promotionSeq, slmCode);
            }
            @ViewBag.customerRegisterAllPmList = customerRegisterAllPmList;
            return PartialView("_ListPackPromotionByCustomerAllPm", new
            {
                @ViewBag.customerRegisterAllPmList
            });
        }
        ///ดึง list promotion ทั้งหมดที่ลูกค้านี้ลง
        public List<listCustomerRegisterByCustomer> GetListPackPromotionByCustomerAllPm(string customerCode, string promotionCode, string promotionSeq, string slmCode)
        {
            string message = "Y";
            List<listCustomerRegisterByCustomer> customerRegisterAllPmList = new List<listCustomerRegisterByCustomer>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Search_Customer_Register_By_Customer", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCustomerCode", customerCode);
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inPromotionSeq", promotionSeq);
                command.Parameters.AddWithValue("@inSlmCode", slmCode);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    customerRegisterAllPmList.Add(new listCustomerRegisterByCustomer()
                    {
                        Cuscode = reader["CUSCOD"].ToString(),
                        Cusname = reader["CUSNAM"].ToString(),
                        Slmcode = reader["SLMCOD"].ToString(),
                        Promotion_Code = reader["Promotion_Code"].ToString(),
                        Promotion_Sub = reader["Promotion_Sub"].ToString(),
                        Description = reader["Description"].ToString(),
                        ProductCode = reader["ProductCode"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        PackQty = reader["PackQty"] != DBNull.Value ? Convert.ToInt32(reader["PackQty"]) : 0,
                        PackAmt = reader["PackAmt"] != DBNull.Value ? Convert.ToInt32(reader["PackAmt"]) : 0,
                        SumPackAmt = reader["SumPackAmt"] != DBNull.Value ? Convert.ToDouble(reader["SumPackAmt"]) : 0.0,
                        Promotion_Year = reader["Promotion_Year"].ToString(),
                        FlagApprove = reader["FLAG_APPROVE"].ToString(),
                        PackQtyPedingApprove = reader["PackQtyPedingApprove"].ToString(),
                        Reason = reader["Reason"].ToString(),
                    });
                }
                reader.Close();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return customerRegisterAllPmList;
        }
        //ดึงผลรวม SumPackAmount
        public ActionResult GetAmountPromotionByCustomer(string customerCode, string promotionCode, string promotionSeq, string packQty, string packAmt, string sumPackAmt)
        {
            string message = "Y";
            string sumPackAmtCurrent = string.Empty;
            string sumPackAmtAfter = string.Empty;
            string sumPackQtyCurrent = string.Empty;
            string sumPackQtyAfter = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Get_Amount_Promotion_By_Customer", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCustomerCode", customerCode);
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inPromotionSeq", promotionSeq);
                command.Parameters.AddWithValue("@inPackQty", packQty);
                command.Parameters.AddWithValue("@inPackAmt", packAmt);
                command.Parameters.AddWithValue("@inSumPackAmt", sumPackAmt);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sumPackQtyCurrent = reader["sumPackQtyCurrent"].ToString();
                    sumPackAmtCurrent = reader["sumPackAmtCurrent"].ToString();
                    sumPackQtyAfter = reader["sumPackQtyAfter"].ToString();
                    sumPackAmtAfter = reader["sumPackAmtAfter"].ToString();
                }
                reader.Close();
                command.Dispose();
                Connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new
            {
                sumPackQtyCurrent = sumPackQtyCurrent,
                sumPackAmtCurrent = sumPackAmtCurrent,
                sumPackQtyAfter = sumPackQtyAfter,
                sumPackAmtAfter = sumPackAmtAfter,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }
        public List<listPromotionBeforeAfter> GetListPromotionBeforeAfter(string customerCode, string promotionCode, string Flag)
        {
            string message = "Y";
            string user = Session["UserID"].ToString();
            List<listPromotionBeforeAfter> promotionBeforeAfterList = new List<listPromotionBeforeAfter>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Search_Promotion_Before_After", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inCustomerCode", customerCode);
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inFlag", Flag);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    promotionBeforeAfterList.Add(new listPromotionBeforeAfter()
                    {
                        RowNumber = dr["RowNumber"].ToString(),
                        Cuscode = dr["Cuscode"].ToString(),
                        Cusname = dr["Cusname"].ToString(),
                        Slmcode = dr["Slmcode"].ToString(),
                        PackQty = dr["PackQty"].ToString(),
                        PackAmt = dr["PackAmt"].ToString(),
                        SumPackAmt = dr["SumPackAmt"].ToString(),
                        InsertDate = dr["InsertDate"].ToString(),
                        Reason = dr["Reason"].ToString(),
                    });
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
            return promotionBeforeAfterList;
        }
        public ActionResult GetPromotionApprove(string slmCode, string company, string year, string period, string prodMgr, string promotionCode)
        {
            string message = "Y";
            string user = Session["UserID"].ToString();
            List<listCustomerApprove> promotionChangeList = new List<listCustomerApprove>();
            var resultList = new List<CustomerPromotionChangeViewModel>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                var command = new SqlCommand("P_Search_Approve_Change_Customer", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inSlmCode", slmCode);
                command.Parameters.AddWithValue("@inCompany", company);
                command.Parameters.AddWithValue("@inPeriod", period);
                command.Parameters.AddWithValue("@inProd", prodMgr);
                command.Parameters.AddWithValue("@inPromotionCode", promotionCode);
                command.Parameters.AddWithValue("@inUser", user);
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    var customer = new listCustomerApprove
                    {
                        Slmcode = dr["SLMCOD"].ToString(),
                        Cuscode = dr["ProCusSelCod"].ToString(),
                        Cusname = dr["CUSNAM"].ToString(),
                        Date_change = dr["Date_change"].ToString(),
                        Promotion_Code_Old = dr["Promotion_Code_old"].ToString(),
                        Promotion_Code_old_Description = dr["Promotion_Code_old_Description"].ToString(),
                        Promotion_Sub_Old = dr["Promotion_Sub_Old"].ToString(),
                        Description_Old = dr["Description_old"].ToString(),
                        Reward_Old = dr["Reward_old"].ToString(),
                        PackQty_total_old = dr["PackQty_total_old"].ToString(),
                        SumPackQty_total_old = dr["SumPackQty_total_old"].ToString(),
                        Cost_Old = dr["Cost_old"].ToString(),
                        PackQty_Old = dr["PackQty_old"].ToString(),
                        PackAmt_Old = dr["PackAmt_old"].ToString(),
                        SumPackAmt_Old = dr["SumPackAmt_old"].ToString(),
                        Promotion_Code_New = dr["Promotion_Code_new"].ToString(),
                        Promotion_Sub_New = dr["Promotion_Sub_New"].ToString(),
                        Description_New = dr["Description_new"].ToString(),
                        Reward_New = dr["Reward_new"].ToString(),
                        PackQty_total_new = dr["PackQty_total_new"].ToString(),
                        SumPackQty_total_new = dr["SumPackQty_total_new"].ToString(),
                        Cost_New = dr["Cost_New"].ToString(),
                        PackQty_New = dr["PackQty_New"].ToString(),
                        PackAmt_New = dr["PackAmt_New"].ToString(),
                        SumPackAmt_New = dr["SumPackAmt_New"].ToString(),
                        Decrease_PackQty = dr["Decrease_PackQty"].ToString(),
                        Decrease_SumPackAmt = dr["Decrease_SumPackAmt"].ToString(),
                        Reason = dr["Reason"].ToString(),
                    };

                    var cusCode = customer.Cuscode;
                    var Promotion_Code_Old = customer.Promotion_Code_Old;

                    resultList.Add(new CustomerPromotionChangeViewModel
                    {
                        CustomerData = customer,
                        PromotionBeforeList = GetListPromotionBeforeAfter(cusCode, Promotion_Code_Old, "before"),
                        PromotionAfterList = GetListPromotionBeforeAfter(cusCode, Promotion_Code_Old, "after")
                    });
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

            @ViewBag.messageError = message;
            @ViewBag.promotionChangeList = resultList;
            return PartialView("_DetailChangeCustomer", new
            {
                @ViewBag.promotionChangeList,
                @ViewBag.messageError
            });
        }
        [HttpPost]
        public ActionResult SaveApproveChangeCustomer(listSaveCustomerApprove[] requestData, string user, string flagApprove) //(string Cuscode, string Codeold, string Seqold, string Codenew, string Seqnew, string User, string Flag)
        {
            int numSuccess = 0;
            int numError = 0;
            string message = "Y";
            List<listSaveCustomerApprove> promotionChangeList = new List<listSaveCustomerApprove>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            try
            {
                if (requestData != null)
                {
                    Connection.Open();
                    foreach (var rowData in requestData)
                    {
                        var command = new SqlCommand("P_Save_Approve_Change_Customer", Connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@inCuscode", rowData.Cuscode);
                        command.Parameters.AddWithValue("@inCodeold", rowData.Codeold);
                        command.Parameters.AddWithValue("@inSeqold", Convert.ToInt32(rowData.Seqold));
                        command.Parameters.AddWithValue("@inPackqtyold", Convert.ToInt32(rowData.Packqtyold));
                        command.Parameters.AddWithValue("@inSumpackamtold", Convert.ToInt32(rowData.Sumpackamtold));
                        command.Parameters.AddWithValue("@inCodenew", rowData.Codenew);
                        command.Parameters.AddWithValue("@inSeqnew", Convert.ToInt32(rowData.Seqnew));
                        command.Parameters.AddWithValue("@inPackqtynew", Convert.ToInt32(rowData.Packqtynew));
                        command.Parameters.AddWithValue("@inSumpackamtnew", Convert.ToInt32(rowData.Sumpackamtnew));
                        command.Parameters.AddWithValue("@inUser", user);
                        command.Parameters.AddWithValue("@inFlagApprove", flagApprove);
                        //SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                        //returnValue.Direction = System.Data.ParameterDirection.Output;
                        //command.Parameters.Add(returnValue);
                        command.ExecuteNonQuery();
                        //message = returnValue.Value.ToString();
                        command.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                Connection.Close();
            }
            return Json(new { status = message, numSuccess = numSuccess, numError = numError }, JsonRequestBehavior.AllowGet);
        }
        public List<SelectListItem> GetCustpmerBySlmcode(string slmCode)
        {
            List<SelectListItem> CUSList = new List<SelectListItem>();
            var connectionString = ConfigurationManager.ConnectionStrings["Promotion_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            //SqlCommand cmd = new SqlCommand("select * from lip.dbo.CUSPROV cusMaster inner join lip.dbo.CUSPROV cusPro on cusMaster.CUSCOD = cusPro.[Promotion Group] where cusMaster.SLMCOD =N'" + slmCode + "' group by cusMaster.CUSCOD, cusMaster.CUSNAM, cusMaster.SLMCOD order by cusMaster.CUSCOD", Connection);
            //SqlDataReader rev_CUSPROV = cmd.ExecuteReader();
            //while (rev_CUSPROV.Read())
            //{
            //    CUSList.Add(new SelectListItem() {
            //        Value = rev_CUSPROV["CUSCOD"].ToString(), 
            //        Text = rev_CUSPROV["CUSCOD"].ToString() + "/" + rev_CUSPROV["CUSNAM"].ToString() 
            //    });
            //}            //
            var command = new SqlCommand("P_Search_Customer_By_Slmcode", Connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@inSlmCode", slmCode);
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                CUSList.Add(new SelectListItem()
                {
                    Value = dr["CUSCOD"].ToString(),
                    Text = dr["CUSCOD"].ToString() + "/" + dr["CUSNAM"].ToString() 
                });
            }
            dr.Close();
            dr.Dispose();
            command.Dispose();
            Connection.Close();
            return CUSList;
        }
    }
}
