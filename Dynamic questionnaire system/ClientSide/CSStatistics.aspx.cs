using DBSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dynamic_questionnaire_system.ClientSide
{
    public partial class CSStatistics : System.Web.UI.Page
    {
        // todo: 這裡還沒完成 => 單一題可顯示，一題以上就沒有.....或許是資料來源衝突??
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["StatisticsID"] == null)
            {
                Response.ClearHeaders();
                Response.Redirect("/ClientSide/CSList.aspx");
            }

            int IDNum = Convert.ToInt32(this.Request.QueryString["StatisticsID"]);
            this.reHD.DataSource = ContextManager.GetHeadingContent(IDNum);
            this.reHD.DataBind();

            this.reTopicDescription.DataSource = ContextManager.GetTopicDescription(IDNum);
            this.reTopicDescription.DataBind();



            //** 資料來源  http://msdn.microsoft.com/zh-tw/library/z9h4dk8y(v=vs.110).aspx

            // Define the name and type of the client scripts on the page.
            String csname1 = "Script1";
            String csname2 = "Script2";
            Type cstype = this.GetType();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;

            // Check to see if the startup script is already registered.
            // 呼叫 IsStartupScriptRegistered 方法，判斷特定索引鍵和型別組的啟始指令碼是否已註冊，避免不必要的指令碼加入嘗試。


            if (!cs.IsStartupScriptRegistered(cstype, csname1))
            {
                String cstext1 = "google.load('visualization', '1.0', { 'packages': ['corechart'] });";
                cstext1 += "google.setOnLoadCallback(drawChart);";

                cs.RegisterStartupScript(cstype, csname1, cstext1, true);
                // 使用 addScriptTags (最後一個)參數，指出 script 參數所提供的指令碼是否包裝在 <script> 項目區塊中。 
                // 最後一個參數 addScriptTags 設為 true，表示<script>指令碼標記會自動加入。
            }

            // Check to see if the client script is already registered.
            if (!cs.IsClientScriptBlockRegistered(cstype, csname2))
            {
                StringBuilder cstext2 = new StringBuilder();
                cstext2.Append("<script type=\"text/javascript\">  function drawChart() {");
                cstext2.Append("var data = new google.visualization.DataTable();");
                cstext2.Append("data.addColumn('string', 'Topping');");
                cstext2.Append("data.addColumn('number', 'Slices');");
                cstext2.Append("data.addRows([['Mushrooms', 3], ['Onions', 1], ['Olives', 1], ['Zucchini', 1], ['Pepperoni', 2]]);");
                cstext2.Append("var options = { 'title': '圖表的標題--How Much Pizza I Ate Last Night', 'width': 400, 'height': 300 };");
                cstext2.Append("var chart = new google.visualization.PieChart(document.getElementById('chart_div'));");
                cstext2.Append("chart.draw(data, options);");
                cstext2.Append("}</script>");

                cs.RegisterClientScriptBlock(cstype, csname2, cstext2.ToString(), false);

            }
        }
        

        
    }
}