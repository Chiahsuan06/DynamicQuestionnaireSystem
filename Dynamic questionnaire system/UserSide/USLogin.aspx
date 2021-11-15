<%@ Page Title="" Language="C#" MasterPageFile="~/Models/Main.Master" AutoEventWireup="true" CodeBehind="USLogin.aspx.cs" Inherits="Dynamic_questionnaire_system.UserSide.USLogin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Label ID="lblAccount" runat="server" Text="帳號："></asp:Label>
        <asp:TextBox ID="txtAccount" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="lblPassword" runat="server" Text="密碼："></asp:Label>
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        <br /><br />
        <asp:Button ID="btnBack" runat="server" Text="返回上一頁" OnClick="btnBack_Click" />
        <asp:Button ID="btnLogin" runat="server" Text="登入" OnClick="btnLogin_Click" />
        <br />
        <asp:Literal ID="ltlMsg" runat="server"></asp:Literal>
    </div>
</asp:Content>
