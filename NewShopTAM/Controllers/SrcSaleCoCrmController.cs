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
    public class SrcSaleCoCrmController : Controller
    {
        //
        // GET: /SrcSaleCoCrm/

        public ActionResult Index()
        {
            //this.Session["UserType"] = "";
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");

            }
           
            return View();
        }
        public JsonResult GetDoccrm(string type)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
               string docgen =string.Empty;
               string CDocNo = string.Empty;
            var commandDoc_Control = new SqlCommand("P_SearchSave_Doc_Control", Connection);
            commandDoc_Control.CommandType = CommandType.StoredProcedure;
            commandDoc_Control.Parameters.AddWithValue("@inDocNam", type);
            SqlParameter returnValuedoc = new SqlParameter("@outResult", SqlDbType.NVarChar, 100);
            returnValuedoc.Direction = System.Data.ParameterDirection.Output;
            commandDoc_Control.Parameters.Add(returnValuedoc);

            commandDoc_Control.ExecuteNonQuery();
            docgen = returnValuedoc.Value.ToString();
           
            commandDoc_Control.Dispose();
         
            string[] strSplit = docgen.Split('=');

            if (strSplit.Length > 1)
            {
                CDocNo = strSplit[1];
            }
            Connection.Close();
            return Json(CDocNo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Filter(string department, string cuscod, string SLMID, string UserCreate, string scnumber, string sonumer)
        {
           
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<Product> inventoryList = new List<Product>();
            Product _model = new Product();
            var commandSclist = new SqlCommand("P_Search_CrmSco_catalog", Connection);
            commandSclist.CommandType = CommandType.StoredProcedure;
            commandSclist.Parameters.AddWithValue("@Status ", department);
            commandSclist.Parameters.AddWithValue("@UserCreate", UserCreate);
            commandSclist.Parameters.AddWithValue("@scno", scnumber);
            commandSclist.Parameters.AddWithValue("@inno", sonumer);
            //commandSclist.ExecuteNonQuery();
            SqlDataReader drSclist = commandSclist.ExecuteReader();
            //  int count = 1;
            while (drSclist.Read())
            {
                inventoryList.Add(new Product()
                {

                    Id = drSclist["RowNo"].ToString(),
                    No = drSclist["DocNo"].ToString(),
                    PhoneStatus = drSclist["PhoneStatus"].ToString(),
                    SaleCustomer = drSclist["ScCrm_CUSCOD"].ToString(),
                    Document_Order = drSclist["Document Order"].ToString(),
                    Step1 = drSclist["Step1"].ToString(),
                    Step2 = drSclist["Step2"].ToString(),
                    Step3 = drSclist["Step3"].ToString(),
                    Step4 = drSclist["Step4"].ToString(),
                    Step5 = drSclist["Step5"].ToString(),
                    Status = drSclist["Status"].ToString(),
                    UserCreate = drSclist["UserCreate"].ToString(),
                    SelltoCustomer = drSclist["CUSCOD"].ToString(),
                    SaleMan = drSclist["SLMCODE"].ToString(),
                    InsertedDate = drSclist["InsertedDate"].ToString(),
                    InsertedBy = drSclist["InsertedBy"].ToString()

                });
            }
            //S20161016
            drSclist.Close();
            drSclist.Dispose();
            commandSclist.Dispose();
            Connection.Close();
            Connection.Dispose();
            SqlConnection.ClearPool(Connection);
            // E20161016
            return PartialView("_PartialPage1", inventoryList);
        }
   
        public JsonResult scstaarea()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<SelectListItem> CodeBy = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand("select * from v_SCStatusArea ", Connection);
            SqlDataReader rev = cmd.ExecuteReader();
            while (rev.Read())
            {
                CodeBy.Add(new SelectListItem()
                {

                    Value = rev["CodeID"].ToString(),
                    Text = rev["CodeNam"].ToString()

                });
            }
            rev.Close();
            rev.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(CodeBy, JsonRequestBehavior.AllowGet);
        }
        public JsonResult sccalby()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<SelectListItem> CodeBy = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand("select * from v_SCCallBy ", Connection);
            SqlDataReader rev = cmd.ExecuteReader();
            while (rev.Read())
            {
                CodeBy.Add(new SelectListItem()
                {

                    Value = rev["CodeID"].ToString(),
                    Text = rev["CodeNam"].ToString()

                });
            }
            rev.Close();
            rev.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(CodeBy, JsonRequestBehavior.AllowGet);
        }
        public JsonResult scphonestatus()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<SelectListItem> CodeBy = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand("select * from v_SCPhoneStatus ", Connection);
            SqlDataReader rev = cmd.ExecuteReader();
            while (rev.Read())
            {
                CodeBy.Add(new SelectListItem()
                {

                    Value = rev["PhID"].ToString(),
                    Text = rev["PhNam"].ToString()

                });
            }
            rev.Close();
            rev.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(CodeBy, JsonRequestBehavior.AllowGet);
        }
        public JsonResult scstatus()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<SelectListItem> CodeBy = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand("select * from v_SCStatus ", Connection);
            SqlDataReader rev = cmd.ExecuteReader();
            while (rev.Read())
            {
                CodeBy.Add(new SelectListItem()
                {

                    Value = rev["ScID"].ToString(),
                    Text = rev["ScNam"].ToString()

                });
            }
            rev.Close();
            rev.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(CodeBy, JsonRequestBehavior.AllowGet);
        }
        public JsonResult scdocumenttpye()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<SelectListItem> CodeBy = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand("select * from v_SCDocType ", Connection);
            SqlDataReader rev = cmd.ExecuteReader();
            while (rev.Read())
            {
                CodeBy.Add(new SelectListItem()
                {

                    Value = rev["DoctypeID"].ToString(),
                    Text = rev["Doctype"].ToString()

                });
            }
            rev.Close();
            rev.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(CodeBy, JsonRequestBehavior.AllowGet);
        }
        public JsonResult scstep()
         {
             var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
             SqlConnection Connection = new SqlConnection(connectionString);
             Connection.Open();
             List<SelectListItem> CodeBy = new List<SelectListItem>();
             SqlCommand cmd = new SqlCommand("select * from v_SCStepType ", Connection);
             SqlDataReader rev = cmd.ExecuteReader();
             while (rev.Read())
             {
                 CodeBy.Add(new SelectListItem()
                 {

                     Value = rev["Code"].ToString(),
                     Text = rev["Description"].ToString()

                 });
             }
             rev.Close();
             rev.Dispose();
             cmd.Dispose();
             Connection.Close();
             return Json(CodeBy, JsonRequestBehavior.AllowGet);
         }
        public JsonResult GetdataStatusItem(string code)
        {
            string _str = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            if (code == "-")
            {
                _str = "WHERE 1=1";
            }
            else
            {
                _str = "WHERE CodeID='" + code + "'";

            }
            List<SelectListItem> SCStatusArea = new List<SelectListItem>();
            SqlCommand cmduserStatusArea = new SqlCommand("select * from v_SCStatusArea " + _str + "", Connection);
            SqlDataReader rev_StatusArea = cmduserStatusArea.ExecuteReader();
            while (rev_StatusArea.Read())
            {
                SCStatusArea.Add(new SelectListItem()
                {

                    Value = rev_StatusArea.GetValue(0).ToString(),
                    Text = rev_StatusArea.GetValue(1).ToString()

                });
            }
            //rev_StatusArea.Dispose();
            //S20161016
            rev_StatusArea.Close();
            rev_StatusArea.Dispose();
            cmduserStatusArea.Dispose();
            //E20161016

            Connection.Close();
            return Json(SCStatusArea, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdataStkGrptable(string stkitem, string Cus, string Slm, string User, string Company)
        {

            string substkgrp = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();

            PricelistpageingSearchTemp Modeltemp = null;
            List<ListPagedListtemp> Getdata = new List<ListPagedListtemp>();
            if (Cus != null)
            {
                var command = new SqlCommand("P_Search_Pricelist", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SaleCode", Slm);
                command.Parameters.AddWithValue("@User", User);
                command.Parameters.AddWithValue("@Customer", Cus);
                command.Parameters.AddWithValue("@Prod", "(ALL)");
                command.Parameters.AddWithValue("@StockGroup", "(ALL)");
                command.Parameters.AddWithValue("@StockCode", stkitem);
                command.Parameters.AddWithValue("@Company", Company);
                // command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {
                    Modeltemp = new PricelistpageingSearchTemp();
                    substkgrp = dr["STKGRP_PRC"].ToString();
                    Modeltemp.STKGRP_PRC = substkgrp.Substring(0, 2);
                    Modeltemp.Price = dr["Price"].ToString();
                    Getdata.Add(new ListPagedListtemp { val = Modeltemp });
                }
                // var stkgrp = db.item.Where(u => u.STKCOD == stkitem).Select(c => c.STKGRP).Distinct();
                //dr.Close();
                //S20161016
                dr.Close();
                dr.Dispose();
                command.Dispose();
                //E20161016
            }
            else
            {
               
                
                var command = new SqlCommand("P_Search_Item_dropdownlist", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@inProd", "(ALL)");
                command.Parameters.AddWithValue("@inSTKGRP", "ALL");
                command.Parameters.AddWithValue("@inFix", "2");
                command.Parameters.AddWithValue("@Company", Company);
                command.Parameters.AddWithValue("@inName", stkitem);
                
                //command.ExecuteNonQuery();
                SqlDataReader dr = command.ExecuteReader();
                while (dr.Read())
                {

                    Modeltemp = new PricelistpageingSearchTemp();
                 
                    Modeltemp.STKGRP_PRC = dr["STKGRP"].ToString();
                    Modeltemp.Price = "0";
                    Getdata.Add(new ListPagedListtemp { val = Modeltemp });
                   

                }
                //dr.Close();
                //S20161016
                dr.Close();
                dr.Dispose();
                command.Dispose();

            }

            Connection.Close();

            return Json(Getdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdateStockCode(string Name, string Prod, string STKGR, string Xcus, string XvalCompany)
        {
            string CUSCOD = string.Empty;
            List<string> StockCode = new List<string>();

           
            if (Prod == "(ALL)") { Prod = ""; }
            if (STKGR == "(ALL)") { STKGR = ""; }

            
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            var command = new SqlCommand("P_Search_Item", Connection);
            command.CommandType = CommandType.StoredProcedure;
           
            command.Parameters.AddWithValue("@inCUSCOD", Xcus);
            command.Parameters.AddWithValue("@inSearch", Name);
            command.Parameters.AddWithValue("@inProd", Prod);
            command.Parameters.AddWithValue("@inSTKGRP", STKGR);
            command.Parameters.AddWithValue("@inFix ", 1);
            command.Parameters.AddWithValue("@Company", XvalCompany);

            Connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {

                //StockCode.Add(reader.GetString(0) + "|" + reader.GetString(1));
                StockCode.Add(dr.GetString(1));

            }
            //dr.Close();
            //S20161016
            dr.Close();
            dr.Dispose();
            command.Dispose();
            //E20161016
            Connection.Close();
            //}
            return Json(StockCode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult scuser()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            List<SelectListItem> CodeBy = new List<SelectListItem>();
            SqlCommand cmd = new SqlCommand("select * from v_ScCrmserCreate ", Connection);
            SqlDataReader rev = cmd.ExecuteReader();
            while (rev.Read())
            {
                CodeBy.Add(new SelectListItem()
                {

                    Value = rev["ScCrm_UserCreate"].ToString(),
                    Text = rev["ScCrm_UserCreate"].ToString()

                });
            }
            rev.Close();
            rev.Dispose();
            cmd.Dispose();
            Connection.Close();
            return Json(CodeBy, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetdateDoc(string User)
        {
            //string u ="swkte";
            List<CusDoc> CusDocsList = new List<CusDoc>();
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string Salcod = string.Empty;
            string CUSNAM1 = string.Empty;
            //var data = db.v_OrderToCrm.Where(c => c.InsertBy == User).ToArray();
            SqlCommand cmd = new SqlCommand("select * from v_OrderToCrm where  InsertBy =N'" + User + "' Order by ORD_DocNo", Connection);
            SqlDataReader rev = cmd.ExecuteReader();
            while (rev.Read())
            {
                CusDocsList.Add(new CusDoc()
                {
                    Cus = rev["CUSCOD"].ToString(),
                    CusNam = rev["CUSNAM"].ToString(),
                    Document_Order = rev["ORD_DocNo"].ToString(),
                    Slm = rev["SLMCODE"].ToString(),
                    ORD_Date = Convert.ToDateTime(rev["ORD_Date"].ToString()).ToString("dd/MM/yyyy"),
                    InsertBy = rev["InsertBy"].ToString()
                });
            }
            //S20161016
            rev.Close();
            rev.Dispose();
            cmd.Dispose();
            //E20161016
            Connection.Close();
            Connection.Dispose();
            SqlConnection.ClearPool(Connection);
            return Json(CusDocsList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult InsertdataCrm(string User,
                    string ScCrm_DocNo,
                    string ScCrm_DocType,
                    string ScCrm_RequeDelivery,
                    string ScCrm_RequeDeliverytime,
                    string ScCrm_ByCall,
                    string ScCrm_PhoneStatus,
                    string ScCrm_CUSCOD,
                    string ScCrm_SLMCODE,
                    string ScCrm_Status,
                    string ScCrm_Step1,
                    string ScCrm_Step2,
                    string ScCrm_Step3,
                    string ScCrm_Step4,
                    string ScCrm_Step5,
                    string ScCrm_Linenote,
                    string ScCrm_UserCreate,
                    string ScCrm_Createdate,
                    string ScCrm_Createdatetime,
                    string ScCrm_UseClosed,
                    string ScCrm_UseCloseddate,
                    string ScCrm_UseCloseddatetime,
                    string Sow,
                    string DataSend)
        {
            string Message = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            try
            {
                string Usr = User;
                decimal decUnit = 0;
                decimal decAmt = 0;
                int dQty = 0;
                int _No = 0;
                string stk = string.Empty;
                List<CrmOrder> _ItemList = new JavaScriptSerializer().Deserialize<List<CrmOrder>>(DataSend);
                //var _listcrm = db.ScCrmLine.Where(r => r.ScCrm_DocNo == ScCrm_DocNo).ToArray();
                SqlCommand cmd = new SqlCommand("P_Save_Crm", Connection);
                cmd.CommandTimeout = 0;
                cmd.Connection = Connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inScCrm_DocNo", ScCrm_DocNo);
                cmd.Parameters.AddWithValue("@inScCrm_ByCall", ScCrm_ByCall);
                cmd.Parameters.AddWithValue("@inScCrm_PhoneStatus", ScCrm_PhoneStatus);
                cmd.Parameters.AddWithValue("@inScCrm_CUSCOD", ScCrm_CUSCOD);
                cmd.Parameters.AddWithValue("@inScCrm_SLMCODE ", ScCrm_SLMCODE);
                cmd.Parameters.AddWithValue("@inScCrm_Orderdate", "");
                cmd.Parameters.AddWithValue("@inScCrm_DocType  ", ScCrm_DocType);
                cmd.Parameters.AddWithValue("@inScCrm_RequeDelivery", ScCrm_RequeDelivery);
                cmd.Parameters.AddWithValue("@inSCCrm_Requetime", ScCrm_RequeDeliverytime);
                cmd.Parameters.AddWithValue("@inScCrm_Status", ScCrm_Status);
                cmd.Parameters.AddWithValue("@inScCrm_Step1", ScCrm_Step1);
                cmd.Parameters.AddWithValue("@inScCrm_Step2 ", ScCrm_Step2);
                cmd.Parameters.AddWithValue("@inScCrm_Step3 ", ScCrm_Step3);
                cmd.Parameters.AddWithValue("@inScCrm_Step4 ", ScCrm_Step4);
                cmd.Parameters.AddWithValue("@inScCrm_Step5 ", ScCrm_Step5);
                cmd.Parameters.AddWithValue("@inScCrm_Linenote", ScCrm_Linenote);
                cmd.Parameters.AddWithValue("@inScCrm_UserCreate", ScCrm_UserCreate);
                cmd.Parameters.AddWithValue("@inScCrm_Createdate", ScCrm_Createdate);
                cmd.Parameters.AddWithValue("@inScCrm_Createtime", ScCrm_Createdatetime);
                cmd.Parameters.AddWithValue("@inScCrm_UseClosed", ScCrm_UseClosed);
                cmd.Parameters.AddWithValue("@inScCrm_UseCloseddate", ScCrm_UseCloseddate);
                cmd.Parameters.AddWithValue("@inScCrm_UseClosedtime", ScCrm_UseCloseddatetime);
                cmd.Parameters.AddWithValue("@inInserted_By", User);
                cmd.Parameters.AddWithValue("@inInserted_Date ", "");
                cmd.Parameters.AddWithValue("@inUpdated_By", User);
                cmd.Parameters.AddWithValue("@inUpdated_Date", "");
                cmd.Parameters.AddWithValue("@inDocument_Order", Sow);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                if (_ItemList.Count > 0)
                {

                    for (int i = 0; i < _ItemList.Count; i++)
                    {
                        SqlCommand cmd_item = new SqlCommand("P_Save_Crm_Item", Connection);
                        cmd_item.CommandTimeout = 0;
                        cmd_item.Connection = Connection;
                        cmd_item.CommandType = CommandType.StoredProcedure;
                        cmd_item.Parameters.AddWithValue("@inSTKCOD", _ItemList[i].STKCOD);
                        cmd_item.Parameters.AddWithValue("@inQty", _ItemList[i].Qty);
                        cmd_item.Parameters.AddWithValue("@inUnitPrice", _ItemList[i].UnitPrice);
                        cmd_item.Parameters.AddWithValue("@inAmt", _ItemList[i].Amount);
                        cmd_item.Parameters.AddWithValue("@inStatusItem", _ItemList[i].StatusItem);
                        cmd_item.Parameters.AddWithValue("@inLineNote", _ItemList[i].LineNote);
                        cmd_item.Parameters.AddWithValue("@inScCrm_DocNo", ScCrm_DocNo);
                        cmd_item.Parameters.AddWithValue("@inScCrmNo", _ItemList[i].ScCrmNo);
                        cmd_item.Parameters.AddWithValue("@inCompany", _ItemList[i].company);
                        
                        cmd_item.ExecuteNonQuery();
                        cmd_item.Dispose();                      
                    }
                }
                Message = "true";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            Connection.Close();
            Connection.Dispose();
            SqlConnection.ClearPool(Connection);

            return Json(Message, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetdataCus(string cus, string Doc)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            //List<SelectListItem> SCStatusArea = new List<SelectListItem>();
            //  //List<CUSBySc> CUSList = new List<CUSBySc>();
            
            List<Crm> CrmList = new List<Crm>();
          
            List<CrmOrder> CrmOrderList = new List<CrmOrder>();



            List<SelectListItem> SCStatusArea = new List<SelectListItem>();
            SqlCommand cmd_r = new SqlCommand("select * from v_SCStatusArea ", Connection);
            SqlDataReader rev_r = cmd_r.ExecuteReader();
            while (rev_r.Read())
            {
                SCStatusArea.Add(new SelectListItem()
                {

                    Value = rev_r["CodeID"].ToString(),
                    Text = rev_r["CodeNam"].ToString()

                });
            }


            SqlCommand cmd = new SqlCommand("select * from v_ScCrm where ScCrm_DocNo =N'" + Doc + "' and ScCrm_CUSCOD =N'" + cus + "'  ", Connection);
                SqlDataReader rev = cmd.ExecuteReader();
                while (rev.Read())
                {
                    
                    CrmList.Add(new Crm()
                    {
                        ScCrm_ID = rev["ScCrm_ID"].ToString(),
                        ScCrm_DocNo = rev["ScCrm_DocNo"].ToString(),
                        Document_Order = rev["Document Order"].ToString(),
                        ScCrm_DocType = rev["ScCrm_DocType"].ToString(),
                        ScCrm_RequeDelivery = rev["ScCrm_RequeDelivery"].ToString(),
                        ScCrm_RequeDeliverytime = rev["SCCrm_Requetime"].ToString(),
                        ScCrm_ByCall = rev["ScCrm_ByCall"].ToString(),
                        ScCrm_PhoneStatus = rev["ScCrm_PhoneStatus"].ToString(),
                        ScCrm_CUSCOD = rev["ScCrm_CUSCOD"].ToString(),
                        ScCrm_CUSNAM =rev["CUSNAM"].ToString(),
                        ScCrm_SLMCODE = rev["ScCrm_SLMCODE"].ToString(),
                        ScCrm_Status = rev["ScCrm_Status"].ToString(),
                        ScCrm_Step1 = rev["ScCrm_Step1"].ToString(),
                        ScCrm_Step2 = rev["ScCrm_Step2"].ToString(),
                        ScCrm_Step3 = rev["ScCrm_Step3"].ToString(),
                        ScCrm_Step4 = rev["ScCrm_Step4"].ToString(),
                        ScCrm_Step5 = rev["ScCrm_Step5"].ToString(),
                        ScCrm_Linenote = rev["ScCrm_Linenote"].ToString(),
                        ScCrm_UserCreate = rev["ScCrm_UserCreate"].ToString(),
                        ScCrm_Createdate = rev["ScCrm_Createdate"].ToString(),
                        ScCrm_Createdatetime = rev["ScCrm_Createtime"].ToString(),
                        ScCrm_UseClosed = rev["ScCrm_UseClosed"].ToString(),
                        ScCrm_UseCloseddate = rev["ScCrm_UseCloseddate"].ToString(),
                        ScCrm_UseCloseddatetime = rev["ScCrm_UseClosedtime"].ToString()
                    });                   
                }

                rev.Close();
                rev.Dispose();
                cmd.Dispose();



                SqlCommand cmdline = new SqlCommand("select * from v_ScCrmLine_D where ScCrm_DocNo =N'" + Doc + "'", Connection);
                SqlDataReader revline = cmdline.ExecuteReader();
                while (revline.Read())
               
                {
                    

                         CrmOrderList.Add(new CrmOrder()
                        {
                            StatusItem = revline["StatusItem"].ToString(),
                            StatusItemName = revline["CodeNam"].ToString(),
                            ScCrmNo = revline["ScCrmNo"].ToString(),
                            ScCrm_DocNo = revline["ScCrm_DocNo"].ToString(),
                            STKCOD = revline["STKCOD"].ToString(),
                            STKDES = revline["STKDES"].ToString(),
                            STKGRP = revline["STKGRP"].ToString(),
                            Qty = revline["Qty"].ToString(),
                            UnitPrice = revline["UnitPrice"].ToString(),
                            Amount = revline["Amt"].ToString(),
                            LineNote = revline["LineNote"].ToString()
                        });
                   
                }
                revline.Close();
                revline.Dispose();
                cmdline.Dispose();

                rev_r.Close();
                rev_r.Dispose();
                cmd_r.Dispose();
              
            
            Connection.Close();
            Connection.Dispose();
            SqlConnection.ClearPool(Connection);
            return Json(new { CrmList, CrmOrderList, SCStatusArea }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DelSrcSaleCoCrm(string DocNo, string Users)
        {
            string message = string.Empty;

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            SqlTransaction tran = Connection.BeginTransaction();
         
            try
            {
                var command = new SqlCommand("P_DelScCrmLine", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = tran;
                command.Parameters.AddWithValue("@inDOC", DocNo);

                SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                returnValue.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnValue);
                command.ExecuteNonQuery();
                message = returnValue.Value.ToString();
                tran.Commit();
                
               
                command.Dispose();
            }
            catch (Exception ex)
            {
                tran.Rollback();

                //throw;
                message = ex.Message;
            }
            Connection.Close();
            return Json(message, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DelSrcSaleCoCrmOrder(string Scid, string stkcod)
        {
            string message = string.Empty;

            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            SqlTransaction tran = Connection.BeginTransaction();

            try
            {
                var command = new SqlCommand("P_DelScCrmLine_D", Connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = tran;
                command.Parameters.AddWithValue("@inscno", Scid);

                SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                returnValue.Direction = System.Data.ParameterDirection.Output;
                command.Parameters.Add(returnValue);
                command.ExecuteNonQuery();
                message = returnValue.Value.ToString();
                tran.Commit();


                command.Dispose();
            }
            catch (Exception ex)
            {
                tran.Rollback();

                //throw;
                message = ex.Message;
            }
            Connection.Close();
         
            return Json(message, JsonRequestBehavior.AllowGet);

        }
    }
     
}
