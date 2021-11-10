<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CSList.aspx.cs" Inherits="Dynamic_questionnaire_system.ClientSide.CSList" %>

<%@ Register Src="~/UserControl/ucPager.ascx" TagPrefix="uc1" TagName="ucPager" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>前台列表頁</title>
<style type="text/css">
        .auto-style1 {
            height: 67px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="auto-style1" style="border: thin solid #000000" >
            <asp:Label ID="lblTitle" runat="server" Text="問卷標題："></asp:Label><asp:TextBox ID="txtTitle" runat="server" TextMode="Search"></asp:TextBox><br />
            <asp:Label ID="lblStart" runat="server" Text="開始時間："></asp:Label><asp:TextBox ID="txtStart" runat="server" TextMode="Date"></asp:TextBox>&nbsp;&nbsp;
            <asp:Label ID="lblEnd" runat="server" Text="結束時間："></asp:Label><asp:TextBox ID="txtEnd" runat="server" TextMode="Date"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnFind" runat="server" Text="搜尋" OnClick="btnFind_Click" />
        </div>
        <div>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField HeaderText="#" DataField="QuestionnaireID" />
                    <asp:TemplateField HeaderText="問卷" >
                        <ItemTemplate>
                            <a href="CSPage.aspx?ID=<%# Eval("QuestionnaireID") %>" id="goPage"><%# Eval("Heading") %></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="狀態" DataField="Vote" />
                    <asp:BoundField HeaderText="開始時間" DataField="StartTime" DataFormatString="{0:yyyy-MM-dd}"/>
                    <asp:BoundField HeaderText="結束時間" DataField="EndTime" DataFormatString="{0:yyyy-MM-dd}"/>
                    <asp:TemplateField HeaderText="觀看統計">
                        <ItemTemplate>
                            <a href="CSStatistics.aspx?StatisticsID=<%# Eval("QuestionnaireID") %>">前往</a>     <%--0726課程--%>
                        </ItemTemplate>
                    </asp:TemplateField>                   
                </Columns>
            </asp:GridView>
            <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>
        </div>
        <uc1:ucPager runat="server" id="ucPager" />
    </form>
</body>
</html>
