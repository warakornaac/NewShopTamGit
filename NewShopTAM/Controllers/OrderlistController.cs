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
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;
namespace NewShopTAM.Controllers
{
    public class OrderlistController : Controller
    {
        //
        // GET: /Orderlist/

        public ActionResult Index()
        {
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");

            }
           

            return View();
        }
        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase postedFile, string customer)
        {
            List<Upload_History> List = new List<Upload_History>();
            string filePath = string.Empty;
            if (postedFile != null)
            {
                
                string path = Server.MapPath("~/Uploadsexcel/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                SqlConnection Connection = new SqlConnection(connectionString);
                Connection.Open();

                string message = "false";
                string cus = customer;
                string usr = Session["UserID"].ToString();
                string conString = string.Empty;
                string Docno = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);
                OleDbConnection excelConnection = new OleDbConnection(conString);
                OleDbCommand cmd = new OleDbCommand("Select * from [Order$]", excelConnection);
                            excelConnection.Open();
                            OleDbDataReader dReader;

                            dReader = cmd.ExecuteReader();
                            //SqlCommand cmdUp = new SqlCommand("p_Upload_Order_Temp", Connection);
                            //cmdUp.Connection = Connection;
                            //cmdUp.CommandType = CommandType.StoredProcedure;
                            while (dReader.Read())
                            {

                                SqlCommand cmdUp = new SqlCommand("p_Upload_Order_Temp", Connection);
                                cmdUp.Connection = Connection;
                                cmdUp.CommandType = CommandType.StoredProcedure;
                                cmdUp.Parameters.AddWithValue("@Cuscod", cus);
                                cmdUp.Parameters.AddWithValue("@Stkcod", dReader.GetValue(0));
                                cmdUp.Parameters.AddWithValue("@Qty", dReader.GetValue(1));
                                cmdUp.Parameters.AddWithValue("@Userlogin", usr);
                                SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                                returnValue.Direction = System.Data.ParameterDirection.Output;
                                cmdUp.Parameters.Add(returnValue);
                                cmdUp.ExecuteNonQuery();
                                cmdUp.Dispose();
                                message = returnValue.Value.ToString();
                            }
                            
                            if (message == "Y")
                            {
                                SqlCommand cmdUpload_Customer_Order = new SqlCommand("p_Upload_Customer_Order", Connection);
                                cmdUpload_Customer_Order.Connection = Connection;
                                cmdUpload_Customer_Order.CommandType = CommandType.StoredProcedure;
                                cmdUpload_Customer_Order.Parameters.AddWithValue("@Cuscod", cus);
                                SqlParameter returnValue = new SqlParameter("@Doc_Out", SqlDbType.NVarChar, 100);
                                returnValue.Direction = System.Data.ParameterDirection.Output;
                                cmdUpload_Customer_Order.Parameters.Add(returnValue);
                                cmdUpload_Customer_Order.ExecuteNonQuery();
                                Docno = returnValue.Value.ToString();
                                cmdUpload_Customer_Order.Dispose();
                                if (Docno != "")
                                {
                                    
                                    SqlCommand cmdSearch = new SqlCommand("P_Search_Ordercustomer_upload_History", Connection);
                                    cmdSearch.Connection = Connection;
                                    cmdSearch.CommandType = CommandType.StoredProcedure;
                                    cmdSearch.Parameters.AddWithValue("@Doc_No", Docno);
                                    SqlDataReader dr = cmdSearch.ExecuteReader();
                                     while (dr.Read())
                                     {
                                         List.Add(new Upload_History()
                                         {
                                                Reference_No = dr["Reference_No"].ToString(),
                                                ID = dr["ID"].ToString(),
                                                CUSCOD = dr["CUSCOD"].ToString(),
                                                STKCOD = dr["STKCOD"].ToString(),
                                                Qty = dr["Qty"].ToString(),
                                                Status = dr["Status"].ToString(),
                                                Status_Message = dr["Status Message"].ToString(),
                                                Inserted_Date = dr["Inserted Date"].ToString(),
                                                Inserted_By = dr["Inserted By"].ToString(),
                                            
                                         });
                                     }
                                }

                            }
                      
                            cmd.Dispose();
                           
                           
                 excelConnection.Close();
                 Connection.Close();
              
            }
            return PartialView("Index", List);
            //return View(List);
        }

        public JsonResult JaImportExcel(HttpPostedFileBase postedFile, string customer)
        {
            List<Upload_History> List = new List<Upload_History>();
            string filePath = string.Empty;
            string exerror = string.Empty;
            try
            {
                if (postedFile != null)
                {

                    string path = Server.MapPath("~/Uploadsexcel/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //string extension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                    //string path1 = string.Format("{0}/{1}", Server.MapPath("~/Uploadsexcel"), Request.Files["file"].FileName);
                    //if (System.IO.File.Exists(path1))
                    //{
                    //    System.IO.File.Delete(path1);
                    //}
                    //// workbook.SaveAs(path1, AccessMode: XlSaveAsAccessMode.xlShared);
                    //Request.Files["file"].SaveAs(path1);
                    // Response.End();
                    string conString = string.Empty;
                    //conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;Persist Security Info=False";


                    var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                    SqlConnection Connection = new SqlConnection(connectionString);
                    Connection.Open();

                    string message = "false";
                    string cus = customer;
                    string usr = Session["UserID"].ToString();
                 
                    string Docno = string.Empty;
                    string stkcod = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }

                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);
                    OleDbConnection excelConnection = new OleDbConnection(conString);
                    OleDbCommand cmd = new OleDbCommand("Select * from [Order$]", excelConnection);
                    excelConnection.Open();
                    OleDbDataReader dReader;

                    dReader = cmd.ExecuteReader();
                    //SqlCommand cmdUp = new SqlCommand("p_Upload_Order_Temp", Connection);
                    //cmdUp.Connection = Connection;
                    //cmdUp.CommandType = CommandType.StoredProcedure;
                    while (dReader.Read())
                    {
                        //stkcod = dReader.GetValue(0);
                        if (dReader.GetValue(0).ToString() != "" && dReader.GetValue(1).ToString() != "")
                        {
                            SqlCommand cmdUp = new SqlCommand("p_Upload_Order_Temp", Connection);
                            cmdUp.Connection = Connection;
                            cmdUp.CommandType = CommandType.StoredProcedure;
                            cmdUp.Parameters.AddWithValue("@Cuscod", cus);
                            cmdUp.Parameters.AddWithValue("@Stkcod", dReader.GetValue(0));
                            cmdUp.Parameters.AddWithValue("@Qty", dReader.GetValue(1));
                            cmdUp.Parameters.AddWithValue("@Userlogin", usr);
                            SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                            returnValue.Direction = System.Data.ParameterDirection.Output;
                            cmdUp.Parameters.Add(returnValue);
                            cmdUp.ExecuteNonQuery();
                            cmdUp.Dispose();
                            message = returnValue.Value.ToString();
                        }
                    }

                    if (message == "Y")
                    {
                        SqlCommand cmdUpload_Customer_Order = new SqlCommand("p_Upload_Customer_Order", Connection);
                        cmdUpload_Customer_Order.Connection = Connection;
                        cmdUpload_Customer_Order.CommandType = CommandType.StoredProcedure;
                        cmdUpload_Customer_Order.Parameters.AddWithValue("@Cuscod", cus);
                        SqlParameter returnValue = new SqlParameter("@Doc_Out", SqlDbType.NVarChar, 100);
                        returnValue.Direction = System.Data.ParameterDirection.Output;
                        cmdUpload_Customer_Order.Parameters.Add(returnValue);
                        cmdUpload_Customer_Order.ExecuteNonQuery();
                        Docno = returnValue.Value.ToString();
                        cmdUpload_Customer_Order.Dispose();
                        if (Docno != "")
                        {

                            SqlCommand cmdSearch = new SqlCommand("P_Search_Ordercustomer_upload_History", Connection);
                            cmdSearch.Connection = Connection;
                            cmdSearch.CommandType = CommandType.StoredProcedure;
                            cmdSearch.Parameters.AddWithValue("@Doc_No", Docno);
                            SqlDataReader dr = cmdSearch.ExecuteReader();
                            while (dr.Read())
                            {
                                List.Add(new Upload_History()
                                {
                                    Reference_No = dr["Reference_No"].ToString(),
                                    ID = dr["ID"].ToString(),
                                    CUSCOD = dr["CUSCOD"].ToString(),
                                    STKCOD = dr["STKCOD"].ToString(),
                                    Qty = dr["Qty"].ToString(),
                                    Status = dr["Status"].ToString(),
                                    Status_Message = dr["Status Message"].ToString(),
                                    Inserted_Date = dr["Inserted Date"].ToString(),
                                    Inserted_By = dr["Inserted By"].ToString(),

                                });
                            }
                        }

                    }

                    cmd.Dispose();


                    excelConnection.Close();
                    Connection.Close();

                }
            }
            catch (Exception ex)
            {

                exerror = ex.Message + '/' + ex.Source + '/' + ex.HelpLink + '/' + ex.HResult;

            }
           // return PartialView("Index", List);
            //return View(List);

            return Json(new { List, exerror }, JsonRequestBehavior.AllowGet);
            //return Json(List, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Pricelisttable()
        {
            //if (this.Session["UserType"] == null)
            //{
            //    return RedirectToAction("LogIn", "Account");

            //}

            return View();
        }
        public JsonResult Confirmationdata(string DataSend)
        {
            string com = string.Empty;
            string substkgrp = string.Empty;
            string pricepromo = string.Empty;
            string lastinvprice = string.Empty;
            var connectionString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            SqlConnection Connection = new SqlConnection(connectionString);
            Connection.Open();
            string message = "false";
            List<ItemListshop> _ItemList = new JavaScriptSerializer().Deserialize<List<ItemListshop>>(DataSend);
            PricelistpageingSearch Model = null;
            List<ListPagedList> Getdata = new List<ListPagedList>();
            //var Getdata = new List<object>();
            try
            {
                if (_ItemList.Count > 0)
                {
                    for (int i = 0; i < _ItemList.Count; i++)
                    {
                        //var root = @"..\IMAGE_B\";

                        SqlCommand cmd = new SqlCommand("p_SaveOrderCart", Connection);
                            cmd.Connection = Connection;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Customer", _ItemList[i].CUSCOD);
                            cmd.Parameters.AddWithValue("@STKCOD", _ItemList[i].STKCOD);
                            cmd.Parameters.AddWithValue("@Company", _ItemList[i].company);

                            cmd.Parameters.AddWithValue("@Price", _ItemList[i].PlcPrice);
                            cmd.Parameters.AddWithValue("@SPrice", _ItemList[i].Price);
                            cmd.Parameters.AddWithValue("@Expect_Price", "0.00");
                            cmd.Parameters.AddWithValue("@specprice", _ItemList[i].SpcPrice);
                            //cmd.Parameters.AddWithValue("@specprice", "0.00");
                            cmd.Parameters.AddWithValue("@promoprice", _ItemList[i].PromoPrice);
                            cmd.Parameters.AddWithValue("@lastinvprice", "0.00");

                            //cmd.Parameters.AddWithValue("@Price", "10.00");
                            //cmd.Parameters.AddWithValue("@SPrice", "10.00");
                            //cmd.Parameters.AddWithValue("@Expect_Price", "0.00");
                            //cmd.Parameters.AddWithValue("@specprice", "10.00");
                            //cmd.Parameters.AddWithValue("@promoprice", "10.00");
                            //cmd.Parameters.AddWithValue("@lastinvprice", "10.00");


                            cmd.Parameters.AddWithValue("@Qty", _ItemList[i].Qty);
                            cmd.Parameters.AddWithValue("@Bckorder", "0");
                            cmd.Parameters.AddWithValue("@InsertedBy", _ItemList[i].User);
                            cmd.Parameters.AddWithValue("@LineNote", _ItemList[i].LineNote);
                            cmd.Parameters.AddWithValue("@FOC", "0");
                            cmd.Parameters.AddWithValue("@ProCode", _ItemList[i].PromotionCode);
                            cmd.Parameters.AddWithValue("@minord", _ItemList[i].Moq);
                            cmd.Parameters.AddWithValue("@prclstno", _ItemList[i].PRCLST_NO);
                            //cmd.Parameters.AddWithValue("@prclstno", "0.00");
                            cmd.Parameters.AddWithValue("@promodesc", _ItemList[i].PromoDesc);
                           
                            cmd.Parameters.AddWithValue("@lastinvdate", "");
                            cmd.Parameters.AddWithValue("@sop", "");
                            SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                            returnValue.Direction = System.Data.ParameterDirection.Output;
                            cmd.Parameters.Add(returnValue);
                            cmd.ExecuteNonQuery();
                            message = returnValue.Value.ToString();


                       
               
                       
                        cmd.Dispose();
                     }
                        Connection.Close();
                    
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(message, JsonRequestBehavior.AllowGet);

        }
    }
     
}
