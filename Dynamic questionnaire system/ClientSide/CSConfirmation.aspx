<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CSConfirmation.aspx.cs" Inherits="Dynamic_questionnaire_system.ClientSide.CSConfirmation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>   <%--標題--%>
            <asp:Repeater ID="reHeading" runat="server">
                <ItemTemplate>
                    <h2>
                        <%#Eval("Heading") %>
                    </h2>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div>
            <asp:Label ID="lblName" runat="server" Text="姓名："></asp:Label>
            <asp:Literal ID="ltlName" runat="server"></asp:Literal><br /><br />
            <asp:Label ID="lblPhone" runat="server" Text="手機："></asp:Label>
            <asp:Literal ID="ltlPhone" runat="server"></asp:Literal><br /><br />
            <asp:Label ID="lblEmail" runat="server" Text="Email："></asp:Label>
            <asp:Literal ID="ltlEmail" runat="server"></asp:Literal><br /><br />
            <asp:Label ID="lblAge" runat="server" Text="年齡："></asp:Label>
            <asp:Literal ID="ltlAge" runat="server"></asp:Literal>
        </div>
         <div>  <%--題目--%>
             <asp:Repeater ID="reTopic" runat="server">
                <ItemTemplate>
                    <p>
                        <%#Eval("Topic") %>
                    </p>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div>
            <asp:Button ID="btnCancel" runat="server" Text="修改" OnClick="btnCancel_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnSent" runat="server" Text="送出" OnClick="btnSent_Click" />
        </div>
    </form>
</body>
</html>
