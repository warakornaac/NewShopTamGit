using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Drawing.Printing;


using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;


using System.Collections.Specialized;
using System.Collections.Generic;

using System.Web.UI.WebControls;

using System.Net;
namespace OrderingMobile.Report
{
    public partial class QuotationForm : System.Web.UI.Page
    {
       
       
        //string strSQL;
        protected DataSet ds = new DataSet();
        string PONUM = string.Empty;
        string _strPONUM = string.Empty;
        string _strCusPo = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                fnLoadReportPo();
            }           
        }
        private void fnLoadReportPo()
        {
            _strPONUM = Request.QueryString["QuotationNUM"];

            string[] words = _strPONUM.Split('/');
            PONUM = words[0];
            _strCusPo = words[1];
            ReportViewerPo.ProcessingMode = ProcessingMode.Local;
            ReportViewerPo.LocalReport.ReportPath = Server.MapPath("~/Report/QuotationForm.rdlc");
            DataSet ds1 = new DataSet();        
            string conString = ConfigurationManager.ConnectionStrings["MobileOrder_ConnectionString"].ConnectionString;        
            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlDataAdapter sda1 = new SqlDataAdapter(string.Format("exec P_SaleOrderPrint_TD_Quatation @inSONumber='{0}'", PONUM), con))
                {
                    sda1.Fill(ds1, "DataSet1");                 
                }
                using (SqlDataAdapter sda2 = new SqlDataAdapter(string.Format("exec P_SaleOrderPrint_TH_Quatation @inSONumber='{0}'", PONUM), con))
                {
                    sda2.Fill(ds1, "DataSet2");
                }
            } 
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ds1.Tables[0]);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", ds1.Tables[1]);
           
            ReportViewerPo.LocalReport.DataSources.Clear();
            ReportViewerPo.ShowPrintButton = true;
            ReportViewerPo.LocalReport.DataSources.Add(datasource1);
            ReportViewerPo.LocalReport.DataSources.Add(datasource2);
            
                                    string reportType = "PDF";
                        string mimeType;
                        string encoding;
                        string fileNameExtension;

                        //The DeviceInfo settings should be changed based on the reportType

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

                        //Render the report
                        renderedBytes = ReportViewerPo.LocalReport.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);
                      
                        //Clear the response stream and write the bytes to the outputstream
                        //Set content-disposition to “attachment” so that user is prompted to take an action
                        //on the file (open or save)
                        Response.Buffer = true;
                        Response.Clear();
                        Response.ContentType = mimeType;
                        Response.AddHeader("content-disposition", "attachment; filename=QuotationReport_" + PONUM + "_" + _strCusPo + "." + fileNameExtension);

                        Response.BinaryWrite(renderedBytes);
                        //string path = @"D:\OrderReport\SOReport.pdf";
                        //                //Check if the directory exists
                        //if (!System.IO.Directory.Exists(@"D:\OrderReport"))
                        //    {
                        //        System.IO.Directory.CreateDirectory(@"D:\OrderReport");
                        //    }

                        //    //Write the file
                        //    using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(@"D:\OrderReport\SOReport.pdf"))
                        //    {
                        //        outfile.Write(path);
                        //    }

                        string path = (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)) + @"\Downloads\QuotationReport_" + PONUM + "_" + _strCusPo + ".pdf";
                        //WebClient client = new WebClient();
                       // Byte[] buffer = client.DownloadData(path);
           
                       
                        System.IO.File.Delete(path);
                        //string path = (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)) + @"\Downloads\SOReport.pdf";
                        //WebClient client = new WebClient();
                        // Byte[] buffer = client.DownloadData(path);
           
                       Response.End();


                    
                        //end the processing of the current page to
                       
                       // Response.End();

                       
                        //var doc = new Document();
                        //var reader = new PdfReader(renderedBytes);

                        //using (FileStream fs = new FileStream(Server.MapPath("~/Report/SaleOrderReport.pdf"), FileMode.Create))
                        //{
                        //    PdfStamper stamper = new PdfStamper(reader, fs);
                        //    string Printer = "";
                        //    if (Printer == null || Printer == "")
                        //    {
                        //        stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                        //        stamper.Close();
                        //    }
                        //    else
                        //    {
                        //        stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = " + Printer + ";print(pp);\r";
                        //        stamper.Close();
                        //    }
                        //}
                        //reader.Close();

                        //FileStream fss = new FileStream(Server.MapPath("~/Report/SaleOrderReport.pdf"), FileMode.Open);
                        //byte[] bytes = new byte[fss.Length];
                        //fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
                        //fss.Close();
                        //System.IO.File.Delete(Server.MapPath("~/Report/SaleOrderReport.pdf"));
                        //return File(bytes, "application/pdf");

                        // Open a PDF document from file
                       // System.IO.FileStream file1 = new System.IO.FileStream("SaleOrderReport.PDF", FileMode.Open, FileAccess.Read, FileShare.Read);




                                                                          //ReportPrintDocument rp = new ReportPrintDocument(ReportViewer.ServerReport);
                                    //rp.Print();
                                }

       
    }
}