using DBSource;
using DQS_Models;
using Json.Net;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dynamic_questionnaire_system.ClientSide
{
    public partial class CSStatistics : System.Web.UI.Page
    {
        // todo: 這裡還沒完成
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

            // todo: 這裡卡著  1203
            //var tb = GetStatisticsDBSourceTB(IDNum);
            var dr = GetStatisticsDBSource(IDNum);
            Response.Write(dr.ItemArray.ToString());

            var getStatistics = new GetStatistics
            {
                TopicNum = (int)dr["TopicNum"],
                TopicDescription = (string)dr["TopicDescription"],
                TopicType = (string)dr["TopicType"],
                answer1 = dr["answer1"] as string,
                answer2 = dr["answer2"] as string,
                answer3 = dr["answer3"] as string,
                answer4 = dr["answer4"] as string,
                answer5 = dr["answer5"] as string,
                answer6 = dr["answer6"] as string,
                OptionsAll = (int)dr["OptionsAll"],
                RDAns = (string)dr["RDAns"],
            };
            
            // 用迴圈跑，但要in ??
            foreach (var item in dr.ItemArray)
            {
                if (getStatistics.TopicType.ToString() == "CB")
                {
                    if (getStatistics.TopicNum == 6)
                    {
                        string s = getStatistics.RDAns;
                        string[] subs = s.Split(';');
                        foreach (string sub in subs)
                        {
                            if (sub == getStatistics.answer1)
                            {
                                getStatistics.answer1Vaule += 1;
                            }
                            else if (sub == getStatistics.answer2)
                            {
                                getStatistics.answer2Vaule += 1;
                            }
                            else if (sub == getStatistics.answer3)
                            {
                                getStatistics.answer3Vaule += 1;
                            }
                            else if (sub == getStatistics.answer4)
                            {
                                getStatistics.answer4Vaule += 1;
                            }
                            else if (sub == getStatistics.answer5)
                            {
                                getStatistics.answer5Vaule += 1;
                            }
                            else if (sub == getStatistics.answer6)
                            {
                                getStatistics.answer6Vaule += 1;
                            }
                        }
                    }
                }
            }
            
            #region 統計圖 - 長條圖

            // Define the name and type of the client scripts on the page.
            string csname11 = "Script1";
            string csname22 = "Script2";
            Type cstype1 = this.GetType();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs1 = Page.ClientScript;

            if (!cs1.IsStartupScriptRegistered(cstype1, csname11))
            {
                string cstext1 = "google.load('visualization', '1.0', { 'packages': ['corechart'] });";
                cstext1 += "google.setOnLoadCallback(drawChart);";

                cs1.RegisterStartupScript(cstype1, csname11, cstext1, true);
                // 使用 addScriptTags (最後一個)參數，指出 script 參數所提供的指令碼是否包裝在 <script> 項目區塊中。 
                // 最後一個參數 addScriptTags 設為 true，表示<script>指令碼標記會自動加入。
            }

            // Check to see if the client script is already registered.
            if (!cs1.IsClientScriptBlockRegistered(cstype1, csname22))
            {
                StringBuilder cstext2 = new StringBuilder();
                cstext2.Append("<script type=\"text/javascript\">  function drawChart() {");
                cstext2.Append("var data = new google.visualization.DataTable();");
                cstext2.Append("data.addColumn('string', 'Topping');");
                cstext2.Append("data.addColumn('number', 'Slices');");
                cstext2.Append($"data.addRows([[{getStatistics.answer1}, {getStatistics.answer1Vaule}], [{getStatistics.answer2}, {getStatistics.answer2Vaule}], [{getStatistics.answer3}, {getStatistics.answer3Vaule}], [{getStatistics.answer4}, {getStatistics.answer4Vaule}], [{getStatistics.answer5}, {getStatistics.answer5Vaule}], [{getStatistics.answer6}, {getStatistics.answer6Vaule}]]);");
                cstext2.Append("var options = { 'title': '圖表的標題--How Much Pizza I Ate Last Night', 'width': 400, 'height': 300 };");
                cstext2.Append("var chart = new google.visualization.BarChart(document.getElementById('Barchart_div'));");
                cstext2.Append("chart.draw(data, options);");
                cstext2.Append("}</script>");

                cs1.RegisterClientScriptBlock(cstype1, csname22, cstext2.ToString(), false);

            }

            #endregion

            foreach (var item_RB in dr.ItemArray)
            {
                getStatistics.answer1Vaule = 0;
                getStatistics.answer2Vaule = 0;
                getStatistics.answer3Vaule = 0;
                getStatistics.answer4Vaule = 0;
                getStatistics.answer5Vaule = 0;
                getStatistics.answer6Vaule = 0;

                if (getStatistics.TopicType.ToString() == "RB")
                {
                    if (getStatistics.RDAns == getStatistics.answer1)
                    {
                        getStatistics.answer1Vaule += 1;
                    }
                    else if (getStatistics.RDAns == getStatistics.answer2)
                    {
                        getStatistics.answer2Vaule += 1;
                    }
                    else if (getStatistics.RDAns == getStatistics.answer3)
                    {
                        getStatistics.answer3Vaule += 1;
                    }
                    else if (getStatistics.RDAns == getStatistics.answer4)
                    {
                        getStatistics.answer4Vaule += 1;
                    }
                    else if (getStatistics.RDAns == getStatistics.answer5)
                    {
                        getStatistics.answer5Vaule += 1;
                    }
                    else if (getStatistics.RDAns == getStatistics.answer6)
                    {
                        getStatistics.answer6Vaule += 1;
                    }
                }
            }
            #region  統計圖 - 圓餅圖
            //** 資料來源  http://msdn.microsoft.com/zh-tw/library/z9h4dk8y(v=vs.110).aspx

            // Define the name and type of the client scripts on the page.
            string csname1 = "Script1";
            string csname2 = "Script2";
            Type cstype = this.GetType();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;

            // Check to see if the startup script is already registered.
            // 呼叫 IsStartupScriptRegistered 方法，判斷特定索引鍵和型別組的啟始指令碼是否已註冊，避免不必要的指令碼加入嘗試。


            if (!cs.IsStartupScriptRegistered(cstype, csname1))
            {
                string cstext1 = "google.load('visualization', '1.0', { 'packages': ['corechart'] });";
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
                cstext2.Append($"data.addRows([[{getStatistics.answer1}, {getStatistics.answer1Vaule}], [{getStatistics.answer2}, {getStatistics.answer2Vaule}], [{getStatistics.answer3}, {getStatistics.answer3Vaule}], [{getStatistics.answer4}, {getStatistics.answer4Vaule}], [{getStatistics.answer5}, {getStatistics.answer5Vaule}], [{getStatistics.answer6}, {getStatistics.answer6Vaule}]]);");
                cstext2.Append("var options = { 'title': '圖表的標題--How Much Pizza I Ate Last Night', 'width': 400, 'height': 300 };");
                cstext2.Append("var chart = new google.visualization.PieChart(document.getElementById('chart_div'));");
                cstext2.Append("chart.draw(data, options);");
                cstext2.Append("}</script>");

                cs.RegisterClientScriptBlock(cstype, csname2, cstext2.ToString(), false);

            }
            #endregion


            foreach (var item in dr.ItemArray)
            {
                if (getStatistics.TopicType.ToString() == "TB")
                {
                    Label label = new Label();
                    label.Text = "文字不做統計";
                }
            }


            //int totalA1Vaule_CB = 0;
            //int totalA2Vaule_CB = 0;
            //int totalA3Vaule_CB = 0;
            //int totalA4Vaule_CB = 0;
            //int totalA5Vaule_CB = 0;
            //int totalA6Vaule_CB = 0;

            //foreach (var sub in StatisticsList)
            //{
            //    totalA1Vaule_CB += sub.answer1Vaule;
            //    totalA2Vaule_CB += sub.answer2Vaule;
            //    totalA3Vaule_CB += sub.answer3Vaule;
            //    totalA4Vaule_CB += sub.answer4Vaule;
            //    totalA5Vaule_CB += sub.answer5Vaule;
            //    totalA6Vaule_CB += sub.answer6Vaule;
            //}

            // todo: 這裡先寫顯示結果，再思考下一步如何處理，但因為上面卡著所以還沒跑出來
            //string json = JsonConvert.SerializeObject(StatisticsList, Formatting.Indented);
            //Response.Write(json);



            
        }

        /// <summary>
        /// 取得 統計所需資料
        /// </summary>
        /// <param name="IDNumber"></param>
        /// <returns></returns>
        public static DataRow GetStatisticsDBSource(int IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Record Details].[TopicNum], [Questionnaires].[TopicDescription], [Questionnaires].[TopicType], 
                          [Question].[answer1], [Question].[answer2],[Question].[answer3], [Question].[answer4], [Question].[answer5],
		                  [Question].[answer6], [Question].[OptionsAll],[RDAns]
                    FROM [Record Details]
                    JOIN [Questionnaires] 
                    ON [Questionnaires].TopicNum = [Record Details].TopicNum
                    JOIN [Question] 
                    ON [Question].TopicNum = [Record Details].TopicNum
                    WHERE [Record Details].[QuestionnaireID] = @QuestionnaireID
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", IDNumber));

            try
            {
                return DBHelper.ReadDataRow(connStr, dbcommand, list);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                return null;
            }
        }
        public static DataTable GetStatisticsDBSourceTB(int IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Record Details].[TopicNum], [Questionnaires].[TopicDescription], [Questionnaires].[TopicType], 
                          [Question].[answer1], [Question].[answer2],[Question].[answer3], [Question].[answer4], [Question].[answer5],
		                  [Question].[answer6], [Question].[OptionsAll],[RDAns]
                    FROM [Record Details]
                    JOIN [Questionnaires] 
                    ON [Questionnaires].TopicNum = [Record Details].TopicNum
                    JOIN [Question] 
                    ON [Question].TopicNum = [Record Details].TopicNum
                    WHERE [Record Details].[QuestionnaireID] = @QuestionnaireID
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", IDNumber));

            try
            {
                return DBHelper.ReadDataTable(connStr, dbcommand, list);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                return null;
            }
        }
        public string DataTableToJson(DataTable table)
        {
            var JsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
            }
            return JsonString.ToString();
        }


    }
}