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
            var tb = ContextManager.GetStatisticsDBSourceTB(IDNum);
            GetStatistics getStatistics;
            Dictionary<int, GetStatistics> dict = new Dictionary<int, GetStatistics>();

            foreach (DataRow dr in tb.Rows)
            {
                int TopicNum = (int)dr["TopicNum"];
                if (!dict.ContainsKey(TopicNum))
                {
                    getStatistics = new GetStatistics
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
                        answer1Vaule = 0,
                        answer2Vaule = 0,
                        answer3Vaule = 0,
                        answer4Vaule = 0,
                        answer5Vaule = 0,
                        answer6Vaule = 0
                    };
                    dict[TopicNum] = getStatistics;
                }

                if (dict[TopicNum].TopicType == "CB") //複選題
                {
                    string s = (string)dr["RDAns"];
                    string[] subs = s.Split(';');
                    foreach (string sub in subs)
                    {
                        if (sub == dict[TopicNum].answer1)
                        {
                            dict[TopicNum].answer1Vaule += 1;
                        }
                        else if (sub == dict[TopicNum].answer2)
                        {
                            dict[TopicNum].answer2Vaule += 1;
                        }
                        else if (sub == dict[TopicNum].answer3)
                        {
                            dict[TopicNum].answer3Vaule += 1;
                        }
                        else if (sub == dict[TopicNum].answer4)
                        {
                            dict[TopicNum].answer4Vaule += 1;
                        }
                        else if (sub == dict[TopicNum].answer5)
                        {
                            dict[TopicNum].answer5Vaule += 1;
                        }
                        else if (sub == dict[TopicNum].answer6)
                        {
                            dict[TopicNum].answer6Vaule += 1;
                        }
                    }
                }
                else //單選題
                {
                    string s = (string)dr["RDAns"];
                    if (s == dict[TopicNum].answer1)
                    {
                        dict[TopicNum].answer1Vaule += 1;
                    }
                    else if (s == dict[TopicNum].answer2)
                    {
                        dict[TopicNum].answer2Vaule += 1;
                    }
                    else if (s == dict[TopicNum].answer3)
                    {
                        dict[TopicNum].answer3Vaule += 1;
                    }
                    else if (s == dict[TopicNum].answer4)
                    {
                        dict[TopicNum].answer4Vaule += 1;
                    }
                    else if (s == dict[TopicNum].answer5)
                    {
                        dict[TopicNum].answer5Vaule += 1;
                    }
                    else if (s == dict[TopicNum].answer6)
                    {
                        dict[TopicNum].answer6Vaule += 1;
                    }
                }
            }

            #region 統計圖

            // Define the name and type of the client scripts on the page.
            string csname1 = "Script1";
            string csname2 = "Script2";
            Type cstype1 = this.GetType();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs1 = Page.ClientScript;

            if (!cs1.IsStartupScriptRegistered(cstype1, csname1))
            {
                string cstext1 = "google.load('visualization', '1.0', { 'packages': ['corechart'] });";
                cstext1 += "google.setOnLoadCallback(drawChart);";

                cs1.RegisterStartupScript(cstype1, csname1, cstext1, true);
                // 使用 addScriptTags (最後一個)參數，指出 script 參數所提供的指令碼是否包裝在 <script> 項目區塊中。 
                // 最後一個參數 addScriptTags 設為 true，表示<script>指令碼標記會自動加入。
            }

            // Check to see if the client script is already registered.
            if (!cs1.IsClientScriptBlockRegistered(cstype1, csname2))
            {
                StringBuilder cstext2 = new StringBuilder();
                cstext2.Append("<script type=\"text/javascript\">  function drawChart() {");
                cstext2.Append("var data,options,chart;\n");
                foreach (KeyValuePair<int, GetStatistics> kvp in dict)
                {
                    cstext2.Append("data = new google.visualization.DataTable();\n");
                    cstext2.Append("data.addColumn('string', 'Topping');\n");
                    cstext2.Append("data.addColumn('number', 'Slices');\n");
                    cstext2.Append($"data.addRows([['{kvp.Value.answer1}', {kvp.Value.answer1Vaule}], ['{kvp.Value.answer2}', {kvp.Value.answer2Vaule}], ['{kvp.Value.answer3}', {kvp.Value.answer3Vaule}], ['{kvp.Value.answer4}', {kvp.Value.answer4Vaule}], ['{kvp.Value.answer5}', {kvp.Value.answer5Vaule}], ['{kvp.Value.answer6}', {kvp.Value.answer6Vaule}]]);\n");
                    cstext2.Append($"options = {{ 'title': '{kvp.Key}.{kvp.Value.TopicDescription}', 'width': 500, 'height': 300 }};\n");
                    cstext2.Append($"document.write(\"<div id='chart_div{kvp.Key}'></div>\");\n");
                    if (kvp.Value.TopicType == "RB")
                        cstext2.Append($"chart = new google.visualization.PieChart(document.getElementById('chart_div{kvp.Key}'));\n"); //圓餅圖
                    else
                        cstext2.Append($"chart = new google.visualization.BarChart(document.getElementById('chart_div{kvp.Key}'));\n"); //長條圖
                    cstext2.Append("chart.draw(data, options);\n");
                }
                cstext2.Append("}</script>\n");
                cs1.RegisterClientScriptBlock(cstype1, csname2, cstext2.ToString(), false);

            }

            #endregion            
        }

        ///// <summary>
        ///// 取得 統計所需資料
        ///// </summary>
        ///// <param name="IDNumber"></param>
        ///// <returns></returns>
        //public static DataTable GetStatisticsDBSourceTB(int IDNumber)
        //{
        //    string connStr = DBHelper.GetConnectionString();
        //    string dbcommand =
        //        $@"SELECT [Record Details].[TopicNum], [Questionnaires].[TopicDescription], [Questionnaires].[TopicType], 
        //                  [Question].[answer1], [Question].[answer2],[Question].[answer3], [Question].[answer4], [Question].[answer5],
		      //            [Question].[answer6], [Question].[OptionsAll],[RDAns]
        //            FROM [Record Details]
        //            JOIN [Questionnaires] 
        //            ON [Questionnaires].TopicNum = [Record Details].TopicNum
        //            JOIN [Question] 
        //            ON [Question].TopicNum = [Record Details].TopicNum
        //            WHERE [Record Details].[QuestionnaireID] = @QuestionnaireID
        //        ";

        //    List<SqlParameter> list = new List<SqlParameter>();
        //    list.Add(new SqlParameter("@QuestionnaireID", IDNumber));

        //    try
        //    {
        //        return DBHelper.ReadDataTable(connStr, dbcommand, list);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(ex);
        //        return null;
        //    }
        //}



    }
}