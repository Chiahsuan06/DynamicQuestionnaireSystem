<%@ Page Title="" Language="C#" MasterPageFile="~/Models/Main.Master" AutoEventWireup="true" CodeBehind="USProblemManagement.aspx.cs" Inherits="Dynamic_questionnaire_system.UserSide.USProblemManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>常用問題管理</h2>
    <asp:Label ID="Label1" runat="server" Text="問題"></asp:Label>
    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox><br />
    <asp:Label ID="Label2" runat="server" Text="回答"></asp:Label>
    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    <asp:DropDownList ID="DropDownList1" runat="server"></asp:DropDownList><asp:CheckBox ID="CheckBox1" runat="server" />
    <asp:Button ID="Button1" runat="server" Text="Button" />
</asp:Content>
