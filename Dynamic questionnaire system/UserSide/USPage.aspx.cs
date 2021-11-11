using DBSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Text;

namespace Dynamic_questionnaire_system.UserSide
{
    public partial class USPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["ID"] == null)
            {
                this.txtStartT.Text = DateTime.Now.ToString("yyyy-MM-dd");  //預設為當日
            }

            givQuestion.DataSource = this.Session["GivQuestionList"] as DataTable;
            givQuestion.DataBind();

            //問題部分
            if (ddlType.SelectedIndex == 1)   //常用問題1 =>常用問題設定要去常用問題管理，要記得處理
            {
                this.txtQuestion.Text = "";
                this.txtOptions.Text = "";
            }
            this.givExport.DataSource = GetRecordData();  //做方法
            this.givExport.DataBind();
        }

        #region 問卷

        /// <summary>
        /// 問卷-送出紐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSent_Click(object sender, EventArgs e)
        {
            string Heading = this.txtQuestaireName.Text;
            string Content = this.txtContent.Text;
            DateTime StartT = Convert.ToDateTime(this.txtStartT.Text);
            DateTime EndT = Convert.ToDateTime(this.txtEndT.Text);
            Guid QuestionnaireNum = Guid.NewGuid();

            if (string.IsNullOrEmpty(this.txtQuestaireName.Text) || string.IsNullOrEmpty(this.txtContent.Text) || string.IsNullOrEmpty(this.txtStartT.Text) || string.IsNullOrEmpty(this.txtEndT.Text))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "問卷名稱、描述內容、開始時間、結束時間 皆為必填";
            }
            if (ckbActivated.Checked == true)  //狀態要顯示已啟用
            {
                //已啟用 Vote 要寫投票中
                string Vote = "投票中";
                MessageBox.Show($"提醒您問卷將送出，請確認", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Add(Heading, Content, StartT, EndT, QuestionnaireNum, Vote);
            }
            else
            {
                if (StartT > DateTime.Now) //尚未開始
                {
                    string Vote = "尚未開始";
                    MessageBox.Show($"提醒您問卷將送出，請確認", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Add(Heading, Content, StartT, EndT, QuestionnaireNum, Vote);

                }
                else if (EndT < DateTime.Now) //已完結
                {
                    string Vote = "已完結";
                    MessageBox.Show($"提醒您問卷將送出，請確認", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Add(Heading, Content, StartT, EndT, QuestionnaireNum, Vote);
                }
            }
            string QuestionnaireID = GetQuestionnaireID(Heading, Content, StartT, EndT).ToString();
            this.Request.QueryString["ID"] = QuestionnaireID; //錯誤訊息：集合是唯讀的
        }
        /// <summary>
        /// 問卷 - 取消紐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("/UserSide/USList.aspx");
        }
        /// <summary>
        /// 問卷 - 寫進資料庫
        /// </summary>
        /// <param name="IDNumber"></param>
        /// <returns></returns>
        public static DataTable Add(string Heading, string Content, DateTime StartT, DateTime EndT, Guid QuestionnaireNum, string Vote)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" INSERT INTO Outline (Heading, Content, StartTime, EndTime, QuestionnaireNum, Vote)
                    VALUES (@Heading, @Content, @StartTime, @EndTime, NEWID(), @Vote )
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Heading", Heading));
            list.Add(new SqlParameter("@Content", Content));
            list.Add(new SqlParameter("@StartTime", StartT));
            list.Add(new SqlParameter("@EndTime", EndT));
            list.Add(new SqlParameter("@QuestionnaireNum", QuestionnaireNum));
            list.Add(new SqlParameter("@Vote", Vote));

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

        /// <summary>
        /// 問卷 - 取得QuestionnaireID
        /// </summary>
        /// <param name="Heading"></param>
        /// <param name="Content"></param>
        /// <param name="StartT"></param>
        /// <param name="EndT"></param>
        /// <param name="QuestionnaireNum"></param>
        /// <param name="Vote"></param>
        /// <returns></returns>
        public static DataTable GetQuestionnaireID(string Heading, string Content, DateTime StartT, DateTime EndT)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [QuestionnaireID]
                    FROM [Outline]
                    WHERE [Heading] = @Heading　
                            AND [Content] = @Content AND [StartTime] = @StartTime AND [EndTime] = @EndTime
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Heading", Heading));
            list.Add(new SqlParameter("@Content", Content));
            list.Add(new SqlParameter("@StartTime", StartT));
            list.Add(new SqlParameter("@EndTime", EndT));

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
        #endregion

        #region 問題
        /// <summary>
        /// 問題 - 加入到表裡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuestion.Text) || string.IsNullOrEmpty(txtOptions.Text))
            {
                this.lblAddMessage.Text = "問題 和 回答 不得空白";
            }

            QuestionColumns qc = new QuestionColumns();
            if (qc.TopicNum == 0)
            {
                qc.TopicNum += 1;
            }
            qc.TopicNum += 1;
            qc.Question = this.txtQuestion.Text;
            qc.Options = this.txtQuestion.Text;

            if (this.ddlChoose.SelectedIndex == 0)
            { qc.OptionsType = "單選方塊"; }
            if (this.ddlChoose.SelectedIndex == 1)
            { qc.OptionsType = "複選方塊"; }
            if (this.ddlChoose.SelectedIndex == 2)
            { qc.OptionsType = "文字"; }

            if (ckbRequired.Checked == true)
            { qc.Required = 0; }
            else
            { qc.Required = -1; }


            this.Session["GivQuestionList"] = qc;
            givQuestion.DataSource = this.Session["GivQuestionList"] as DataTable;
            givQuestion.DataBind();

        }

        class QuestionColumns
        {
            public int TopicNum { get; set; } //序號
            public string Question { get; set; } //問題
            public string Options { get; set; } //回答
            public string OptionsType { get; set; } //種類
            public int Required { get; set; }  //必填

        }

        protected void givQuestion_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        /// <summary>
        /// 問題 - 刪除紐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ImgbtnBin_Click(object sender, ImageClickEventArgs e)
        {

        }
        /// <summary>
        /// 問題 - giv 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btngivCancel_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 問題 - giv送出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btngivSent_Click(object sender, EventArgs e)
        {

        }
        #endregion

        /// <summary>
        /// 填寫資料 - 匯出紐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt = GetAnsRecordDetailsData();//獲取datatable資料源
            string title = "問卷資料" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".CSV";//匯出的檔案名
            EstablishCSV(dt, title);//將dt資料源和檔案名title代入匯出方法中

        }
        private void EstablishCSV(DataTable dt, string fileName)
        {
             HttpContext.Current.Response.Clear();
             StreamWriter sw = new StreamWriter(Response.OutputStream, Encoding.UTF8);
             int iColCount = dt.Columns.Count;
             for (int i = 0; i < iColCount; i++)//表頭
             {
                 sw.Write("\"" + dt.Columns[i] + "\"");
                 if (i < iColCount - 1)
                 {
                     sw.Write(",");
                 }
             }
             sw.Write(sw.NewLine);
             foreach (DataRow dr in dt.Rows)//行內資料
             {
                 for (int i = 0; i < iColCount; i++)
                 {
                    if (!Convert.IsDBNull(dr[i]))
                            sw.Write("\"" + dr[i].ToString() + "\"");
                    else
                            sw.Write("\"\"");
                    if (i < iColCount - 1)
                    {
                        sw.Write(",");
                    }
                 }
                 sw.Write(sw.NewLine);
             }


             sw.Close();
             HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
             HttpContext.Current.Response.ContentType = "text/csv";
             HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;

             HttpContext.Current.Response.Write(sw);
             HttpContext.Current.Response.End();

         }

        /// <summary>
        /// 顯示givExport資料
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        public static DataTable GetRecordData()
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [RecordNum]
                          ,[QuestionnaireID]
                          ,[AnsName]
                          ,[AnsTime]
                      FROM [Questionnaire].[dbo].[Record]
                      ORDER BY [AnsTime] DESC
                ";

            List<SqlParameter> list = new List<SqlParameter>();

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
        /// <summary>
        /// 取得問卷填寫的細節
        /// </summary>
        /// <returns></returns>
        public static DataTable GetRecordDetailsData(int RecordNum)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [Record].[RecordNum]
                          ,[Record].[QuestionnaireID]
                          ,[AnswererID]
                          ,[AnsName]
                          ,[AnsPhone]
                          ,[AnsEmail]
                          ,[AnsAge]
                          ,[AnsTime]
	                      ,[ReplicationNum]
                          ,[Record Details].[TopicNum]
	                      ,[TopicDescription]
                          ,[Record Details].[OptionsNum]
	                      ,[OptionsDescription]
                      FROM [Questionnaire].[dbo].[Record]
                      JOIN [Questionnaire].[dbo].[Record Details]
                      ON [Record].RecordNum = [Record Details].RecordNum
                      JOIN[Questionnaire].[dbo].[Questionnaires]
                      ON [Record Details].[TopicNum] = [Questionnaires].[TopicNum]  
                      JOIN[Questionnaire].[dbo].[Question]
                      ON [Question].[OptionsNum] = [Record Details].[OptionsNum]
                      WHERE [Record].[RecordNum] = @RecordNum
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@RecordNum", RecordNum));;

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
        /// <summary>
        /// 取得使用者資訊、每個問題、每個問題的答案
        /// </summary>
        /// <param name="RecordNum"></param>
        /// <returns></returns>
        public static DataTable GetAnsRecordDetailsData()
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [Record].[RecordNum]
	　                    ,[AnswererID]
                          ,[AnsName]
                          ,[AnsPhone]
                          ,[AnsEmail]
                          ,[AnsAge]
                          ,[AnsTime]
                          ,[Record].[QuestionnaireID]
	                      ,[Outline].[Heading]
                          ,[Record Details].[TopicNum]
	                      ,[TopicDescription]
                          ,[Record Details].[OptionsNum]
	                      ,[OptionsDescription]
                      FROM [Questionnaire].[dbo].[Record]
                      JOIN [Questionnaire].[dbo].[Outline]
                      ON [Record].[QuestionnaireID] = [Outline].[QuestionnaireID]
                      JOIN [Questionnaire].[dbo].[Record Details]
                      ON [Record].RecordNum = [Record Details].RecordNum
                      JOIN[Questionnaire].[dbo].[Questionnaires]
                      ON [Record Details].[TopicNum] = [Questionnaires].[TopicNum]  
                      JOIN[Questionnaire].[dbo].[Question]
                      ON [Question].[OptionsNum] = [Record Details].[OptionsNum]
                ";

            List<SqlParameter> list = new List<SqlParameter>();

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
    }
}