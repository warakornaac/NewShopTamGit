using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Reporting.WebForms;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Net;

namespace OrderingMobile.Report
{
    public partial class SOForm : System.Web.UI.Page
    {
        protected DataSet ds = new DataSet();
        string SONUM = string.Empty;
        string _strSONUM = string.Empty;
        string _strCus = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                fnLoadReport();
            }
        }

        private void fnLoadReport()
        {
            try
            {
                _strSONUM = Request.QueryString["SONUM"];
                if (string.IsNullOrEmpty(_strSONUM))
                {
                    throw new Exception("SONUM is required");
                }

                string[] words = _strSONUM.Split('/');
                SONUM = words[0];
                _strCus = words[1];

                ReportViewer.LocalReport.ReportPath = Server.MapPath("~/Report/SOForm.rdlc");

                DataSet ds1 = new DataSet();
                string conString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;

                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    // ✅ ใช้ Parameter แล้ว - ปลอดภัยจาก SQL Injection
                    using (SqlCommand cmd1 = new SqlCommand("P_SaleOrderPrint_TD_catalog", con))
                    {
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@inSONumber", SONUM ?? (object)DBNull.Value);

                        using (SqlDataAdapter sda1 = new SqlDataAdapter(cmd1))
                        {
                            sda1.Fill(ds1, "DataSet1");
                        }
                    }

                    using (SqlCommand cmd2 = new SqlCommand("P_SaleOrderPrint_TH_catalog", con))
                    {
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@inSONumber", SONUM ?? (object)DBNull.Value);

                        using (SqlDataAdapter sda2 = new SqlDataAdapter(cmd2))
                        {
                            sda2.Fill(ds1, "DataSet2");
                        }
                    }
                }

                // ✅ Check ว่า DataSet มี data ก่อนใช้
                if (ds1.Tables.Count < 2 || ds1.Tables[0].Rows.Count == 0 || ds1.Tables[1].Rows.Count == 0)
                {
                    throw new Exception("No data found for SONUM: " + SONUM);
                }

                ReportDataSource datasource1 = new ReportDataSource("DataSet1", ds1.Tables[0]);
                ReportDataSource datasource2 = new ReportDataSource("DataSet2", ds1.Tables[1]);

                ReportViewer.LocalReport.DataSources.Clear();
                ReportViewer.ShowPrintButton = true;
                ReportViewer.LocalReport.DataSources.Add(datasource1);
                ReportViewer.LocalReport.DataSources.Add(datasource2);

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =
                "<DeviceInfo>" +
                " <OutputFormat>PDF</OutputFormat>" +
                " <PageWidth>8.5in</PageWidth>" +
                "<PageHeight>11.7in</PageHeight>" +
                "<MarginTop>0in</MarginTop>" +
                " <MarginLeft>0.4in</MarginLeft>" +
                " <MarginRight>0.2in</MarginRight>" +
                " <MarginBottom>0in</MarginBottom>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = ReportViewer.LocalReport.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename=SOReport_" + SONUM + "_" + _strCus + "." + fileNameExtension);
                Response.BinaryWrite(renderedBytes);

                // ✅ ลบ file ที่ download ในเครื่อง (optional)
                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads",
                    "SOReport_" + SONUM + "_" + _strCus + ".pdf");

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                Response.End();
            }
            catch (Exception ex)
            {
                // ✅ Log error แล้ว show user
                System.Diagnostics.Debug.WriteLine("Error in fnLoadReport: " + ex.Message);
                Response.Write("<h3>Error: " + HttpUtility.HtmlEncode(ex.Message) + "</h3>");
            }
        }
    }
}