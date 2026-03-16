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
using ClosedXML.Excel;

namespace NewShopTAM.Controllers
{
    public class UploadsExcelController : Controller
    {
        //
        // GET: /UploadsExcel/

        public ActionResult Index()
        {
            if (this.Session["UserType"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            return View();
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
                        if (dReader.GetValue(0).ToString() != "" && (dReader.GetValue(1).ToString() != "" || dReader.GetValue(2).ToString() != ""))
                        {
                            SqlCommand cmdUp = new SqlCommand("p_Upload_Order_Temp", Connection);
                            cmdUp.Connection = Connection;
                            cmdUp.CommandType = CommandType.StoredProcedure;
                            cmdUp.Parameters.AddWithValue("@Cuscod", cus);
                            cmdUp.Parameters.AddWithValue("@Company", dReader.GetValue(0) ?? string.Empty);
                            cmdUp.Parameters.AddWithValue("@Stkcod", dReader.GetValue(1) ?? string.Empty);
                            cmdUp.Parameters.AddWithValue("@cStkcod", dReader.GetValue(2) ?? string.Empty);
                            cmdUp.Parameters.AddWithValue("@Qty", dReader.GetValue(3) ?? string.Empty);
                            cmdUp.Parameters.AddWithValue("@Price", dReader.GetValue(4) ?? string.Empty);
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
                        cmdUpload_Customer_Order.CommandTimeout = 1000;
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
                                    Company = dr["Company"].ToString(),
                                    STKCOD = dr["STKCOD"].ToString(),
                                    Cus_STKCOD = dr["Cus_STKCOD"].ToString(),
                                    UOM = dr["UOM"].ToString(),
                                    Qty = dr["Qty"].ToString(),
                                    Price = dr["Price"].ToString(),
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
        //Export result
        public ActionResult ExportResult(string referenceNo)
        {
            List<Upload_History> list = new List<Upload_History>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString);
            connection.Open();
            SqlCommand cmdSearch = new SqlCommand("P_Search_Ordercustomer_upload_History", connection);
            cmdSearch.CommandType = CommandType.StoredProcedure;
            cmdSearch.Parameters.AddWithValue("@Doc_No", referenceNo);
            SqlDataReader dr = cmdSearch.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Upload_History()
                {
                    Reference_No = dr["Reference_No"].ToString(),
                    ID = dr["ID"].ToString(),
                    CUSCOD = dr["CUSCOD"].ToString(),
                    Company = dr["Company"].ToString(),
                    STKCOD = dr["STKCOD"].ToString(),
                    Cus_STKCOD = dr["Cus_STKCOD"].ToString(),
                    UOM = dr["UOM"].ToString(),
                    Qty = dr["Qty"].ToString(),
                    Price = dr["Price"].ToString(),
                    Status = dr["Status"].ToString(),
                    Status_Message = dr["Status Message"].ToString(),
                    Inserted_Date = dr["Inserted Date"].ToString(),
                    Inserted_By = dr["Inserted By"].ToString(),
                });
            }
            dr.Close();
            connection.Close();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Upload History");
                worksheet.Cell(1, 1).Value = "Reference No";
                worksheet.Cell(1, 2).Value = "CustomerCode";
                worksheet.Cell(1, 3).Value = "Company";
                worksheet.Cell(1, 4).Value = "PartNo";
                worksheet.Cell(1, 5).Value = "CustomerPartNo";
                worksheet.Cell(1, 6).Value = "UOM";
                worksheet.Cell(1, 7).Value = "Qty";
                worksheet.Cell(1, 8).Value = "Price";
                worksheet.Cell(1, 9).Value = "Status";
                worksheet.Cell(1, 10).Value = "Status Message";
                worksheet.Cell(1, 11).Value = "Inserted Date";
                worksheet.Cell(1, 12).Value = "Inserted By";

                int row = 2;
                foreach (var item in list)
                {
                    worksheet.Cell(row, 1).Value = item.Reference_No;
                    worksheet.Cell(row, 2).Value = item.CUSCOD;
                    worksheet.Cell(row, 3).Value = item.Company;
                    worksheet.Cell(row, 4).Value = item.STKCOD;
                    worksheet.Cell(row, 5).Value = item.Cus_STKCOD;
                    worksheet.Cell(row, 6).Value = item.UOM;
                    worksheet.Cell(row, 7).Value = item.Qty;
                    worksheet.Cell(row, 8).Value = item.Price;
                    worksheet.Cell(row, 9).Value = item.Status;
                    worksheet.Cell(row, 10).Value = item.Status_Message;
                    worksheet.Cell(row, 11).Value = item.Inserted_Date;
                    worksheet.Cell(row, 12).Value = item.Inserted_By;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    byte[] fileBytes = stream.ToArray();

                    //set filename
                    string timestamp = DateTime.Now.ToString("ddMMyyyy_HHmmss");
                    string fileName = $"Upload_Order_Result_{timestamp}.xlsx";

                    return File(fileBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                fileName);
                }
            }
        }
    }
}