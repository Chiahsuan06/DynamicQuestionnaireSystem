<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CSStatistics.aspx.cs" Inherits="Dynamic_questionnaire_system.ClientSide.CSStatistics" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>

    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    

</head>

<body>
    <form id="form1" runat="server">
        <h1>前台</h1>
        <asp:Repeater ID="reHD" runat="server">
            <ItemTemplate>
                <h2><%#Eval("Heading") %></h2>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Repeater ID="reTopicDescription" runat="server">
            <ItemTemplate>
                <p><%# Container.ItemIndex + 1 %>.<%#Eval("TopicDescription") %></p>
                <div id="chart_div<%#Eval("TopicNum")%>"></div>
            </ItemTemplate>
        </asp:Repeater>
    </form>
</body>
</html>
