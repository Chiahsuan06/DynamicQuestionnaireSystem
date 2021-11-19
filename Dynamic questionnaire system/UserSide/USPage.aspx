<%@ Page Title="" Language="C#" MasterPageFile="~/Models/Main.Master" AutoEventWireup="true" CodeBehind="USPage.aspx.cs" Inherits="Dynamic_questionnaire_system.UserSide.USPage" %>

<%@ Register Src="~/UserControl/ucPager.ascx" TagPrefix="uc1" TagName="ucPager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1">
       <%-- <style>
            body {font-family: Arial;}

            .tab {
              overflow: hidden;
              border: 1px solid #ccc;
              background-color: #f1f1f1;
            }

            .tab button {
              background-color: inherit;
              float: left;
              border: none;
              outline: none;
              cursor: pointer;
              padding: 14px 16px;
              transition: 0.3s;
              font-size: 17px;
            }

            .tab button:hover {
              background-color: #ddd;
            }

            .tab button.active {
              background-color: #ccc;
            }

            .tabcontent {
              display: none;
              padding: 6px 12px;
              border: 1px solid #ccc;
              border-top: none;
            }
        </style>
        <script>
            function openQuestionnaire(evt, idName) {

                var i, tabcontent, tablinks;
                tabcontent = document.getElementsByClassName("tabcontent");
                for (i = 0; i < tabcontent.length; i++) {
                    tabcontent[i].style.display = "none";
                }

                tablinks = document.getElementsByClassName("tablinks");
                for (i = 0; i < tablinks.length; i++) {
                    tablinks[i].className = tablinks[i].className.replace(" active", "");
                }

                document.getElementById(idName).style.display = "block";
                evt.currentTarget.className += " active";
            }

            document.getElementById("defaultOpen").click();
        </script>--%>

        <div class="tab">
          <button type="button" class="tablinks" onclick="openQuestionnaire(event, 'Questionnaire')" id="defaultOpen">問卷</button>
          <button type="button" class="tablinks" onclick="openQuestionnaire(event, 'Question')">問題</button>
          <button type="button" class="tablinks" onclick="openQuestionnaire(event, 'WriteInformation')">填寫資料</button>
          <button type="button" class="tablinks" onclick="openQuestionnaire(event, 'Statistics')">統計</button>
        </div>

        <div id="Questionnaire" class="tabcontent">  <%--問卷--%>
            <asp:Label ID="lblQuestaireName" runat="server" Text="問卷名稱"></asp:Label>&nbsp;&nbsp;
            <asp:TextBox ID="txtQuestaireName" runat="server" Height="25px" Width="271px"></asp:TextBox>
            <br />
            <asp:Label ID="lblContent" runat="server" Text="描述內容"></asp:Label>&nbsp;&nbsp;
            <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Height="130px" Width="279px"></asp:TextBox>
            <br />
            <asp:Label ID="lblStartT" runat="server" Text="開始時間"></asp:Label>&nbsp;&nbsp;
            <asp:TextBox ID="txtStartT" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="lblEndT" runat="server" Text="結束時間"></asp:Label>&nbsp;&nbsp;
            <asp:TextBox ID="txtEndT" runat="server"></asp:TextBox>
            <br />
            <asp:CheckBox ID="ckbActivated" runat="server" Checked="True" /><asp:Label ID="lblActivated" runat="server" Text="已啟用"></asp:Label>
            <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblMessage" runat="server" Visible="False" ForeColor="Red" ></asp:Label>
            <asp:Button ID="btnCancel" runat="server" Text="取消" OnClick="btnCancel_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnSent" runat="server" Text="送出" OnClick="btnSent_Click" />
        </div>

        <div id="Question" class="tabcontent">  <%--問題--%>
            <asp:Label ID="lblType" runat="server" Text="種類"></asp:Label>&nbsp;&nbsp;
            <asp:DropDownList ID="ddlType" runat="server">
                <asp:ListItem Value="0">自訂問題</asp:ListItem>
                <asp:ListItem Value="1">常用問題1</asp:ListItem>
            </asp:DropDownList>
            <br />
            <asp:Label ID="lblQuestion" runat="server" Text="問題"></asp:Label>&nbsp;&nbsp;
            <asp:TextBox ID="txtQuestion" runat="server"></asp:TextBox>&nbsp;&nbsp;
            <asp:DropDownList ID="ddlChoose" runat="server">
                <asp:ListItem Value="0">單選方塊</asp:ListItem>
                <asp:ListItem Value="1">複選方塊</asp:ListItem>
                <asp:ListItem Value="2">文字</asp:ListItem>
            </asp:DropDownList>&nbsp;&nbsp;
            <asp:CheckBox ID="ckbRequired" runat="server" /><asp:Label ID="lblRequired" runat="server" Text="必填"></asp:Label>
            <br />
            <asp:Label ID="lblOptions" runat="server" Text="回答"></asp:Label>&nbsp;&nbsp;
            <asp:TextBox ID="txtOptions" runat="server"></asp:TextBox>(多個答案以；分隔)&nbsp;&nbsp;
            <asp:Button ID="btnAddIn" runat="server" Text="加入" OnClick="btnAddIn_Click" />
            <br /><br />
            <asp:ImageButton ID="ImgbtnBin" runat="server" ImageUrl="~/Images/bin.png" Height="29px" Width="34px" OnClick="ImgbtnBin_Click"/>&nbsp;&nbsp;<asp:Label ID="lblAddMessage" runat="server" ForeColor="Red"></asp:Label>
            <asp:GridView ID="givQuestion" runat="server" AutoGenerateColumns="False" OnRowCommand="givQuestion_RowCommand" OnRowDataBound="givQuestion_RowDataBound" OnRowUpdating="givQuestion_RowUpdating">
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
                            <asp:CheckBox ID="chbMustKeyIn" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TopicMustKeyIn" />
                    <asp:ButtonField ButtonType="Button" CommandName="Update" Text="編輯" />
                </Columns>
            </asp:GridView>
            <div>
                <asp:Button ID="btngivCancel" runat="server" Text="取消" OnClick="btngivCancel_Click"/>&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btngivSent" runat="server" Text="送出" OnClick="btngivSent_Click"/>
            </div>

        </div>

        <div id="WriteInformation" class="tabcontent">  <%--填寫資料--%>
            <asp:Button ID="btnExport" runat="server" Text="匯出" OnClick="btnExport_Click" />
            <asp:GridView ID="givExport" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField HeaderText="#" DataField="RecordNum"/>
                    <asp:BoundField HeaderText="姓名" DataField="AnsName" />
                    <asp:BoundField HeaderText="填寫時間" DataField="AnsTime"/>
                    <asp:TemplateField HeaderText="觀看細節">
                        <ItemTemplate>
                            <a href="USPage.aspxID=<%#Eval("RecordNum") %>">前往</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <uc1:ucPager runat="server" ID="ucPager" PageSize ="10" Url="/UserSide/USPage.aspx"/>

            <%--分頁--%>

            <asp:PlaceHolder ID="PlaceHolderDetail" runat="server">
                <div>
                    <asp:Label ID="plblName" runat="server" Text="姓名"></asp:Label>
                    <asp:TextBox ID="txtName" runat="server" ReadOnly="True"></asp:TextBox><br />
                    <asp:Label ID="plblPhone" runat="server" Text="手機"></asp:Label>
                    <asp:TextBox ID="txtPhone" runat="server" ReadOnly="True"></asp:TextBox><br />
                    <asp:Label ID="plblEmail" runat="server" Text="Email"></asp:Label>
                    <asp:TextBox ID="txtEmail" runat="server" ReadOnly="True"></asp:TextBox><br />
                    <asp:Label ID="plblAge" runat="server" Text="年齡"></asp:Label>
                    <asp:TextBox ID="txtAge" runat="server" ReadOnly="True"></asp:TextBox>
                    <asp:Label ID="lblWriteT" runat="server" Text=""></asp:Label>  <%--連結時間--%>
                </div>
                <div>
                    <%--跳出填答問卷--%>
                </div>
            </asp:PlaceHolder>

        </div>

        <div id="Statistics" class="tabcontent">  <%--統計--%>
          <h3>統計</h3>
        </div>
</asp:Content>
