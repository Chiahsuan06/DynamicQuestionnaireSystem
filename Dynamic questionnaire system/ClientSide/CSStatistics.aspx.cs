using DBSource;
using DQS_Models;
using Json.Net;
using Newtonsoft.Json;
using System;
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



            var dr = GetStatisticsDBSource(IDNum);
            // todo: 這裡卡著、SQL是OK、單獨用 Response.Write(dr["TopicNum"])是OK的
            var getStatistics = new GetStatistics
            {
                TopicNum = (int)dr["TopicNum"],
                TopicDescription = (string)dr["TopicDescription"],
                TopicType = (string)dr["TopicType"],
                answer1 = (string)dr["answer1"],
                answer2 = (string)dr["answer2"],
                answer3 = (string)dr["answer3"],
                answer4 = (string)dr["answer4"],
                answer5 = (string)dr["answer5"],
                answer6 = (string)dr["answer6"],
                OptionsAll = (int)dr["OptionsAll"],
                RDAns = (string)dr["RDAns"],
            };
            
            switch (getStatistics.TopicType)
            {
                case "CB":
                    /*=>要將複選答案分開*/
                    string s = getStatistics.RDAns;
                    string[] subs = s.Split(';');
                    foreach (string sub in subs)
                    {
                        getStatistics.RDAns = sub;
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
                    break;

                case "RB":
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
                    break;

                case "TB":
                    Label label = new Label();
                    label.Text = "文字不做統計";
                    break;
            }

            var sessionStatisticsList = this.Session["StatisticsList"] as List<GetStatistics>;//將Session轉成List，再做總和
            sessionStatisticsList.Add(getStatistics);

            int totalA1Vaule = 0;
            int totalA2Vaule = 0;
            int totalA3Vaule = 0;
            int totalA4Vaule = 0;
            int totalA5Vaule = 0;
            int totalA6Vaule = 0;

            foreach (var sub in sessionStatisticsList)
            {
                totalA1Vaule += sub.answer1Vaule;
                totalA2Vaule += sub.answer2Vaule;
                totalA3Vaule += sub.answer3Vaule;
                totalA4Vaule += sub.answer4Vaule;
                totalA5Vaule += sub.answer5Vaule;
                totalA6Vaule += sub.answer6Vaule;
            }

            // todo: 這裡先寫顯示結果，再思考下一步如何處理，但因為上面卡著所以還沒跑出來
            Response.Write($"{totalA1Vaule}, {totalA2Vaule}, {totalA3Vaule}, {totalA4Vaule}, {totalA5Vaule}, {totalA6Vaule}");
            string json = JsonConvert.SerializeObject(getStatistics, Formatting.Indented);
            Response.Write(json);
            #region  統計圖
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
            #endregion
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

        //public static string ListToJson(T data)
        //{
        //    try
        //    {
        //        System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(data.GetType());
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            serializer.WriteObject(ms, data);
        //            return Encoding.UTF8.GetString(ms.ToArray());
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}


    }
}