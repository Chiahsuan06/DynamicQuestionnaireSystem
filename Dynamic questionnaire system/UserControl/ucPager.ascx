<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucPager.ascx.cs" Inherits="Dynamic_questionnaire_system.UserControl.ucPager" %>
<div>
    <a runat="server" id="aLinkFirst" href="#">First</a>
    <a runat="server" id="aLink1" href="#">1</a>
    <a runat="server" id="aLink2" href="#">2</a>
    <asp:Literal ID="ltlCurrentPage" runat="server"></asp:Literal>

    <a runat="server" id="aLink3" href="#">3</a>
    <a runat="server" id="aLink4" href="#">4</a>
    <a runat="server" id="aLinkLast" href="#">Last</a>

    

    <asp:Literal ID="ltPager" runat="server"></asp:Literal>
</div>