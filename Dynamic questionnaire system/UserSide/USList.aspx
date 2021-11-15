<%@ Page Title="" Language="C#" MasterPageFile="~/Models/Main.Master" AutoEventWireup="true" CodeBehind="USList.aspx.cs" Inherits="Dynamic_questionnaire_system.UserSide.USList" %>

<%@ Register Src="~/UserControl/ucPager.ascx" TagPrefix="uc1" TagName="ucPager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--    <div>
        <asp:Label ID="lblWelcome" runat="server"></asp:Label>
    </div>--%>
    <div class="auto-style1" style="border: thin solid #000000" >
            <asp:Label ID="lblTitle" runat="server" Text="問卷標題："></asp:Label>
            <asp:TextBox ID="txtTitle" runat="server" TextMode="Search"></asp:TextBox><br />
            <asp:Label ID="lblStart" runat="server" Text="開始時間："></asp:Label>
            <asp:TextBox ID="txtStart" runat="server" TextMode="Date"></asp:TextBox>&nbsp;&nbsp;
            <asp:Label ID="lblEnd" runat="server" Text="結束時間："></asp:Label>
            <asp:TextBox ID="txtEnd" runat="server" TextMode="Date"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnFind" runat="server" Text="搜尋" OnClick="btnFind_Click" />
        </div>
    <br />
        <div>
            <asp:ImageButton ID="ImgbtnBin" runat="server" ImageUrl="~/Images/bin.png" Height="29px" Width="34px" OnClick="ImgbtnBin_Click" OnClientClick="return confirm(&quot;請確認您將刪除的問卷，是否正確&quot;)" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:ImageButton ID="ImgbtnAdd" runat="server" ImageUrl="~/Images/add.png" Height="29px" Width="34px" OnClick="ImgbtnAdd_Click" />
            <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>
        </div>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="#" DataField="QuestionnaireID" />
            <asp:HyperLinkField HeaderText="問卷" DataTextField='Heading' DataNavigateUrlFields="QuestionnaireID" DataNavigateUrlFormatString="CSPage.aspx?ID={0}" />
            <asp:BoundField HeaderText="狀態" DataField="Vote"/>
            <asp:BoundField HeaderText="開始時間" DataField="StartTime" DataFormatString="{0:yyyy-MM-dd}"/>
            <asp:BoundField HeaderText="結束時間" DataField="EndTime" DataFormatString="{0:yyyy-MM-dd}"/>
            <asp:TemplateField HeaderText="觀看統計">
                <ItemTemplate>
                    <a href="CSStatistics.aspx?Number=<%# Eval("QuestionnaireID") %>">前往</a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
    <uc1:ucPager runat="server" ID="ucPager" PageSize ="10" Url="/UserSide/USList.aspx"/>
</asp:Content>
