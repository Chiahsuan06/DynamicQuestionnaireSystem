<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Dynamic_questionnaire_system.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>
                        <asp:ImageButton ID="imgbtnLogin" runat="server" ImageUrl="~/Images/login.png" Height="250px" Width="250px" OnClick="imgbtnLogin_Click" ToolTip="問卷管理者登入" />
                    </td>
                    <td></td>
                    <td>
                         <asp:ImageButton ID="imgbtnQuestion" runat="server" ImageUrl="~/Images/survey.png" Height="250px" Width="250px" OnClick="imgbtnQuestion_Click" ToolTip="開始填寫問卷"/>
                    </td>
                </tr>
            </table>
            
        </div>
    </form>
</body>
</html>
