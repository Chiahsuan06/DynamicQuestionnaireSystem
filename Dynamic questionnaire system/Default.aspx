<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Dynamic_questionnaire_system.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table align="center" valign="middle">
                <tr>
                    <td>
                        <asp:ImageButton ID="imgbtnLogin" runat="server" ImageUrl="~/Images/login.png" Height="250px" Width="250px" OnClick="imgbtnLogin_Click" ToolTip="問卷管理者登入" />
                        <p class="auto-style1">問卷管理者登入</p>
                    </td>
                    <td></td>
                    <td>
                         <asp:ImageButton ID="imgbtnQuestion" runat="server" ImageUrl="~/Images/survey.png" Height="250px" Width="250px" OnClick="imgbtnQuestion_Click" ToolTip="開始填寫問卷"/>
                        <p class="auto-style1">開始填寫問卷</p>
                    </td>
                </tr>
            </table>
            
        </div>
    </form>
</body>
</html>
