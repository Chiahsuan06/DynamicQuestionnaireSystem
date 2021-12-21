<%@ Page Title="" Language="C#" MasterPageFile="~/Models/Main.Master" AutoEventWireup="true" CodeBehind="USProblemManagement.aspx.cs" Inherits="Dynamic_questionnaire_system.UserSide.USProblemManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>常用問題管理</h2>
    <asp:Label ID="Label1" runat="server" Text="問題"></asp:Label>
    <asp:TextBox ID="txtQuestion" runat="server"></asp:TextBox><br />
    <asp:Label ID="Label2" runat="server" Text="回答"></asp:Label>
    <asp:TextBox ID="txtOptions" runat="server"></asp:TextBox>
    <asp:DropDownList ID="ddlChoose" runat="server">
        <asp:ListItem Value="RB">單選方塊</asp:ListItem>
        <asp:ListItem Value="CB">複選方塊</asp:ListItem>
        <asp:ListItem Value="TB">文字</asp:ListItem>
    </asp:DropDownList><asp:CheckBox ID="ckbMustKeyIn" runat="server" /><asp:Label ID="lblMustKeyIn" runat="server" Text="必填"></asp:Label>&nbsp;&nbsp;
    <asp:Button ID="btnAdd" runat="server" Text="加入" />
    <br />
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" OnRowUpdating="GridView1_RowUpdating">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBox1" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="#" DataField="TopicNum"/>
            <asp:BoundField HeaderText="問題" DataField="TopicDescription"/>
            <asp:BoundField HeaderText="種類" DataField="TopicType"/>
            <asp:TemplateField HeaderText="必填">
                <ItemTemplate>
                    <asp:CheckBox ID="chbMustKeyIn" runat="server" Enabled="False" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TopicMustKeyIn" />
            <asp:ButtonField ButtonType="Button" CommandName="Update" Text="編輯" />
        </Columns>
    </asp:GridView>
    <asp:Button ID="btnCancel" runat="server" Text="取消" />&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btnSent" runat="server" Text="送出" />
</asp:Content>
