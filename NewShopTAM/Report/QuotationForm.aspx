<%@ Page Language="C#" AutoEventWireup="true" CodeFile="QuotationForm.aspx.cs" Inherits="OrderingMobile.Report.QuotationForm" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>
   
   
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"> 
<html>


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
   
    <form id="formPo" runat="server">

    <h1>Print Sale order</h1>
            
   <!-- <div class="myRow" style="clear:both" id="pnlReport">    
            
          <rsweb:ReportViewer ID="ReportViewerPo" runat="server" Font-Names="Verdana" Font-Size="8pt"
        InteractiveDeviceInfos="(Collection)" WaitMessageFont-Names="Verdana" 
        WaitMessageFont-Size="14pt" Width="1280px" Height="750px" SizeToReportContent="True" AsyncRendering="False" >
              <LocalReport EnableHyperlinks="True" ReportPath="Report\POForm.rdlc">
              </LocalReport>
    </rsweb:ReportViewer>
    </div> -->
   
    


    <asp:ScriptManager ID="ScriptManagerPo" runat="server">
    </asp:ScriptManager>

    </form>

  <%--  <%=Neodynamic.SDK.Web.WebClientPrint.CreateScript()%>--%>
     <script src="../Scripts/jquery-1.7.1.js"></script>
    <script type="text/javascript">
//<script type="text/javascript" lang="javascript">
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
    

</script>
</body>
</html>