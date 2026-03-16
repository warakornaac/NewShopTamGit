<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SOForm.aspx.cs" Inherits="OrderingMobile.Report.SOForm" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
   
   
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%--<%@ Import Namespace="Neodynamic.SDK.Web" %>
<%@ Import Namespace="Neodynamic.SDK.Web" %>--%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"> 
<html>
<%--<script runat="server">


    protected void Page_Init(object sender, EventArgs e)
    {
        //Print report???
        if (WebClientPrint.ProcessPrintJob(Request))
        {
            
            //create PDF version of RDLC report
            LocalReport myReport = new LocalReport();
            myReport.ReportPath = "SOForm.rdlc";
            DataSet ds = new DataSet();
            ds.ReadXml(Server.MapPath("~/NorthwindProducts.xml"));
            myReport.DataSources.Add(new ReportDataSource("Products", ds.Tables[0]));

            //Export to PDF. Get binary content.
            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;

		string deviceInfo =
            "<deviceinfo>" +
            "  <outputformat>PDF</outputformat>" +
            "  <pagewidth>8.5in</pagewidth>" +
            "  <pageheight>11in</pageheight>" +
            "  <margintop>0.5in</margintop>" +
            "  <marginleft>0.75in</marginleft>" +
            "  <marginright>0.5in</marginright>" +
            "  <marginbottom>0.5in</marginbottom>" +
            "</deviceinfo>";

            byte[] pdfContent = myReport.Render("PDF", deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            //Now send this file to the client side for printing
            //IMPORTANT: Adobe Reader needs to be installed at the client side
                       
            bool useDefaultPrinter = (Request["useDefaultPrinter"] == "checked");
            string printerName = Server.UrlDecode(Request["printerName"]);

            //create a temp file name for our PDF report...
            string fileName = Guid.NewGuid().ToString("N") + ".pdf";
            
            //Create a PrintFile object with the pdf report
            PrintFile file = new PrintFile(pdfContent, fileName);
            //Create a ClientPrintJob and send it back to the client!
            ClientPrintJob cpj = new ClientPrintJob();
            //set file to print...
            cpj.PrintFile = file;
            //set client printer...
            if (useDefaultPrinter || printerName == "null")
                cpj.ClientPrinter = new DefaultPrinter();
            else
                cpj.ClientPrinter = new InstalledPrinter(printerName);
            //send it...
            cpj.SendToClient(Response);            

        }
    }
    
    

    
</script>--%>

<%--<html xmlns="http://www.w3.org/1999/xhtml">--%>
<head runat="server">
    <title>Print Sale Order</title>

    <style>
        body{font: 13px 'Segoe UI', Tahoma, Arial, Helvetica, sans-serif;background:#ddd;color:#333;margin:0;}
        h1{background:#333;color:#fff;padding:10px;font: 29px 'Segoe UI Light', 'Tahoma Light', 'Arial Light', 'Helvetica Light', sans-serif;}
        .myRow{width:auto;padding:0 20px 0 20px;height:auto;}        
        .myMenu{float:left;margin:0 20px 0 0;padding:2px;color:#333;}  
        .cBlue{border-bottom: 5px solid #6B89B7;}
        .cYellow{border-bottom: 5px solid #FCAA25;}
        .cSand{border-bottom: 5px solid #CCCC66;}
    </style>

</head>
<body>
    <%-- Store User's SessionId --%>
    <input type="hidden" id="sid" name="sid" value="<%=Session.SessionID%>" />
   
    <form id="form1" runat="server">

    <h1>Print Sale order</h1>
            
    <div class="myRow" style="clear:both" id="pnlReport">    
            
          <rsweb:ReportViewer ID="ReportViewer" runat="server" Font-Names="Verdana" Font-Size="8pt" Visible="true"
        InteractiveDeviceInfos="(Collection)" ProcessingMode="Remote" WaitMessageFont-Names="Verdana" 
        WaitMessageFont-Size="14pt" Width="1280px" Height="700px" ShowZoomControl="True" ShowPrintButton="True" ShowPageNavigationControls="True"
        ZoomMode="Percent" ZoomPercent="100" InternalBorderStyle="Solid" SizeToReportContent="True" AsyncRendering="False" >
              <LocalReport EnableHyperlinks="True">
              </LocalReport>
    </rsweb:ReportViewer>
    </div>
   
    


    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    </form>

  <%--  <%=Neodynamic.SDK.Web.WebClientPrint.CreateScript()%>--%>
     <script src="../Scripts/jquery-1.7.1.js"></script>
 
<script type="text/javascript" lang="javascript">
    $(document).ready(function () {
        if ($.browser.mozilla || $.browser.webkit) {
            try {
                showPrintButton();
            }
            catch (e) {
                alert(e);

            }
        }
        if ($.browser.safari) {
            $("#ReportViewerControl table").each(function (i, item) {
                $(item).css('display', 'inline-block');
            });
        }
    });
    function showPrintButton() {
        var table = $("table[title='Refresh']");
        var parentTable = $(table).parents('table');
        var parentDiv = $(parentTable).parents('div').parents('div').first();
        parentDiv.append('<input type="image" style="border-width: 0px; padding: 3px;margin-top:2px; height:16px; width: 16px;" alt="Print" src="../Reserved.ReportViewerWebControl.axd?OpType=Resource&amp;Version=9.0.30729.1&amp;Name=Microsoft.Reporting.WebForms.Icons.Print.gif";title="Print" onclick="PrintReport();">');
    }
    // Print Report function
    function PrintReport() {
        var strFrameName = ("printer-" + (new Date()).getTime());
        var jFrame = $("<iframe name='" + strFrameName + "'>");
        jFrame
        .css("width", "1px")
        .css("height", "1px")
        .css("position", "absolute")
        .css("left", "-2000px")
        .appendTo($("body:first"));

        var objFrame = window.frames[strFrameName];
        var objDoc = objFrame.document;
        var jStyleDiv = $("<div>").append($("style").clone());

        objDoc.open();
        objDoc.write($("head").html());
        objDoc.write($("#ReportViewer_ctl09").html());
        objDoc.close();
        objFrame.print();
        
        setTimeout(function () { jFrame.remove(); }, (60 * 1000));
    }
    //function PrintReport() {
    //    //  $find("ReportViewer").invokePrintDialog();
    //    //    var viewerReference = $find("ReportViewer");

    //    //    var stillonLoadState = viewerReference.get_isLoading();

    //    //    if (!stillonLoadState) {
    //    //        var reportArea = viewerReference.get_reportAreaContentType();
    //    //        if (reportArea == Microsoft.Reporting.WebFormsClient.ReportAreaContent.ReportPage) {
    //    //            $find("ReportViewer").invokePrintDialog();
    //    //        }
    //    //    }
    //    //}

    //    //get the ReportViewer Id

    //    var rv1 = $('#ReportViewer_ctl09');

    //    var iDoc = rv1.parents('html');



    //    //Reading the report styles

    //    var styles = iDoc.find("head style[id$='ReportControl_styles']").html();

    //    if ((styles == undefined) || (styles == '')) {

    //        iDoc.find('head script').each(function () {

    //            var cnt = $(this).html();

    //            var p1 = cnt.indexOf('ReportStyles":"');
    //            console.log(p1);
    //            if (p1 > 0) {

    //                p1 += 15;

    //                var p2 = cnt.indexOf('"', p1);

    //                styles = cnt.substr(p1, p2 - p1);

    //            }

    //        });

    //    }

    //    if (styles == '') { alert("Cannot generate styles, Displaying without styles.."); }

    //    styles = '<style type="text/css">' + styles + "</style>";



    //    // Reading the report html

    //    var table = rv1.find("div[id$='_oReportDiv']");

    //    if (table == undefined) {

    //        alert("Report source not found.");

    //        return;

    //    }



    //    //  Generating a copy of the report in a new window

    //    var docType = '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/loose.dtd">';

    //    var docCnt = styles + table.parent().html();

    //    var docHead = '<head><style>body{margin:5;padding:0;}</style></head>';

    //    var winAttr = "location=yes, statusbar=no, directories=no, menubar=no, titlebar=no, toolbar=no, dependent=no, width=720, height=600, resizable=yes, screenX=200, screenY=200, personalbar=no, scrollbars=yes";;

    //    var newWin = window.open("", "_blank", winAttr);

    //    writeDoc = newWin.document;

    //    writeDoc.open();

    //    writeDoc.write(docType + '<html>' + docHead + '<body onload="window.print();">' + docCnt + '</body></html>');

    //    writeDoc.close();

    //    newWin.focus();

    //    // uncomment to autoclose the preview window when printing is confirmed or canceled.

    //    //  newWin.close();
    //};

</script>
</body>
</html>