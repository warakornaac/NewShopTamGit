using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;

namespace NewShopTAM.Models
{
    public class ImportExcelController : Controller
    {
        //
        // GET: /ImportExcel/

        //public ActionResult Index()
        //{
        //    return View();
        //}
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
       {
        string filePath = string.Empty;
        if (postedFile != null)
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
            string path = Server.MapPath("~/Uploadsexcel/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
 
            filePath = path + Path.GetFileName(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);
            postedFile.SaveAs(filePath);
 
            string conString = string.Empty;
            switch (extension)
            {
                case".xls": //Excel 97-03.
                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case".xlsx": //Excel 07 and above.
                    conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                    break;
            }
 
            DataTable dt = new DataTable();
            conString = string.Format(conString, filePath);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;
                        string message = "false";
                        string cus = Session["CUSCOD"].ToString();
                        string usr = Session["UserID"].ToString();
                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();
 
                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                            OleDbDataReader dReader;
                         dReader = cmdExcel.ExecuteReader();
                         conString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
                            using (SqlConnection con = new SqlConnection(conString))
                            {
                                con.Open();
                                 SqlCommand cmd = new SqlCommand("p_Upload_Order_Temp", con);
                                            
                                            while (dReader.Read())
                                            {
                                                cmd.Connection = con;
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Parameters.AddWithValue("@Cuscod", cus);
                                                cmd.Parameters.AddWithValue("@Stkcod", dReader.GetValue(0));
                                                cmd.Parameters.AddWithValue("@Qty", dReader.GetValue(1));
                                                cmd.Parameters.AddWithValue("@Userlogin", usr);
                                                SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                                                returnValue.Direction = System.Data.ParameterDirection.Output;
                                                cmd.Parameters.Add(returnValue);
                                                cmd.ExecuteNonQuery();
                                                message = returnValue.Value.ToString();
                                            }
                                 con.Close();
                             }
                            
                        odaExcel.Fill(dt);
                        connExcel.Close();                           
                    }
                }
            }

            conString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name.
                    //sqlBulkCopy.DestinationTableName = "dbo.Ordercustomer_upload";
                    //string message = "false";
               
                    //SqlCommand cmd = new SqlCommand("p_SaveOrderCart", con);
                    //cmd.Connection = con;
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Cuscod", Session["CUSCOD"].ToString());
                    //cmd.Parameters.AddWithValue("@Stkcod", );
                    //cmd.Parameters.AddWithValue("@Qty", _ItemList[i].company);
                    //cmd.Parameters.AddWithValue("@Userlogin", Session["UserID"].ToString());
                    //SqlParameter returnValue = new SqlParameter("@outGenstatus", SqlDbType.NVarChar, 100);
                    //returnValue.Direction = System.Data.ParameterDirection.Output;
                    //cmd.Parameters.Add(returnValue);
                    //cmd.ExecuteNonQuery();
                    //message = returnValue.Value.ToString();
                  
                    //[OPTIONAL]: Map the Excel columns with that of the database table
                    //sqlBulkCopy.ColumnMappings.Add("CUSCOD", Session["CUSCOD"].ToString());
                    //sqlBulkCopy.ColumnMappings.Add("STKCOD", "STKCOD");
                    //sqlBulkCopy.ColumnMappings.Add("Qty", "Qty");
                    //sqlBulkCopy.ColumnMappings.Add("Status", "N");
                    //sqlBulkCopy.ColumnMappings.Add("[Inserted Date]", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    //sqlBulkCopy.ColumnMappings.Add("[Inserted By]", Session["UserID"].ToString());
                    con.Open();
                   // sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
        }
 
        return View();
        }
    }
 }

