<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CSStatistics.aspx.cs" Inherits="Dynamic_questionnaire_system.ClientSide.CSStatistics" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
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
                <p><%#Eval("TopicNum") %>.<%#Eval("TopicDescription") %></p>
                <asp:Chart ID="Chart1" runat="server">
                            <Series>
                                <asp:Series Name="Series1"></asp:Series>
                            </Series>
                            <ChartAreas>
                                <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                            </ChartAreas>
                        </asp:Chart>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:QuestionnaireConnectionString2 %>" SelectCommand="SELECT [Record Details].[RDAns], COUNT([Record Details].[RDAns]) AS COUNT
	                                     FROM [Questionnaire].[dbo].[Record Details]
	                                     WHERE [QuestionnaireID] = @QuestionnaireID AND [TopicNum] = @TopicNum
	                                     GROUP BY [RDAns];">
                                        <SelectParameters>
                                            <asp:QueryStringParameter  Name="QuestionnaireID" QueryStringField="StatisticsID" />
                                            <asp:QueryStringParameter DefaultValue="1" Name="TopicNum" QueryStringField="TopicNum" />
                                        </SelectParameters>
                        </asp:SqlDataSource>
            </ItemTemplate>
        </asp:Repeater>
        <%--<div>
            <asp:Chart ID="Chart1" runat="server" DataSourceID="SqlDataSource1">
                <Series>
                    <asp:Series Name="Series1" XValueMember="RDAns" YValueMembers="COUNT" ChartType="Bar"></asp:Series>
                </Series>
                <ChartAreas>
                    <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                </ChartAreas>
            </asp:Chart>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:QuestionnaireConnectionString2 %>" SelectCommand="  SELECT [Record Details].[RDAns], COUNT([Record Details].[RDAns]) AS COUNT
	            FROM [Questionnaire].[dbo].[Record Details]
	            WHERE [QuestionnaireID] = @QuestionnaireID
	            GROUP BY [RDAns];">
                <SelectParameters>
                    <asp:QueryStringParameter Name="QuestionnaireID" QueryStringField="StatisticsID" />

                </SelectParameters>
            </asp:SqlDataSource>
        </div>--%>
    </form>
</body>
</html>
