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
using DQS_Models;
using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace Dynamic_questionnaire_system.UserSide
{
    public partial class USPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // check is logined
            if (!AuthManager.IsLogined())
            {
                Response.Redirect("/UserSide/USLogin.aspx");
                return;
            }
            if (this.Request.QueryString["ID"] == null)  //全新問卷
            {
                this.txtStartT.Text = DateTime.Now.ToString("yyyy-MM-dd");  //預設為當日
            }
            else //既有問卷
            {
                int QuestionnaireID = Convert.ToInt32(this.Request.QueryString["ID"]);
                var dr = GetDBData(QuestionnaireID);
                this.txtQuestaireName.Text = dr["Heading"].ToString();
                this.txtContent.Text = dr["Content"].ToString();
                this.txtStartT.Text = Convert.ToDateTime(dr["StartTime"]).ToString("yyyy/MM/dd");
                this.txtEndT.Text = Convert.ToDateTime(dr["EndTime"]).ToString("yyyy/MM/dd");
                if (dr["Vote"].ToString() == "投票中")
                {
                    this.ckbActivated.Checked = true;
                }
                else
                {
                    this.ckbActivated.Checked = false;
                }

                //後台內頁2-問題
                //this.givQuestion.DataSource = this.Session["GivQuestionList"] as DataTable;
                this.givQuestion.DataSource = GetGivDBData(QuestionnaireID);
                this.givQuestion.DataBind();

                //後台內頁2-問題
                if (ddlType.SelectedIndex == 1)   //常用問題1 =>常用問題設定要去常用問題管理，要記得處理
                {
                    this.txtQuestion.Text = "";
                    this.txtOptions.Text = "";
                }
            }




            //後台內頁3-填寫資料
            this.givExport.DataSource = GetRecordData();  //做方法
            this.givExport.DataBind();
            var dt = GetRecordData();
            DataSearch(dt);

        }

        #region 分頁
        /// <summary>
        /// 分頁控制
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private void DataSearch(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                var dtPaged = this.GetPagedDataTable(dt);

                this.ucPager.TotalSize = dt.Rows.Count;
                this.ucPager.Bind();

                this.givExport.DataSource = dtPaged;
                this.givExport.DataBind();
                this.ucPager.Visible = true;
            }
            else
            {
                this.givExport.Visible = false;
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "請重新搜尋";
            }
        }
        private DataTable GetPagedDataTable(DataTable dt)
        {
            DataTable dtPaged = dt.Clone();
            int pageSize = this.ucPager.PageSize;

            int startIndex = (this.GetCurrentPage() - 1) * pageSize;
            int endIndex = (this.GetCurrentPage()) * pageSize;

            if (endIndex > dt.Rows.Count)
                endIndex = dt.Rows.Count;

            for (var i = startIndex; i < endIndex; i++)
            {
                DataRow dr = dt.Rows[i];
                var drNew = dtPaged.NewRow();

                foreach (DataColumn dc in dt.Columns)
                {
                    drNew[dc.ColumnName] = dr[dc];
                }

                dtPaged.Rows.Add(drNew);
            }
            return dtPaged;
        }
        private int GetCurrentPage()
        {
            string pageText = Request.QueryString["Page"];

            if (string.IsNullOrWhiteSpace(pageText))
                return 1;

            int intPage;
            if (!int.TryParse(pageText, out intPage))
                return 1;

            if (intPage <= 0)
                return 1;

            return intPage;
        }
        #endregion


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
            string Vote = "";

            if (string.IsNullOrEmpty(this.txtQuestaireName.Text) || string.IsNullOrEmpty(this.txtContent.Text) || string.IsNullOrEmpty(this.txtStartT.Text))
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "問卷名稱、描述內容、開始時間 皆為必填";
            }

            if (this.Request.QueryString["ID"] == null)  //=>生成新問卷
            {
                Guid QuestionnaireNum = Guid.NewGuid();
                string Account = this.Session["UserLoginInfo"].ToString();

                if (ckbActivated.Checked == true)  //狀態要顯示已啟用
                {                    
                    Vote = "投票中";          //已啟用 Vote 要寫投票中
                } 
                else
                {
                    if (StartT > DateTime.Now) //尚未開始
                    {
                        Vote = "尚未開始";

                    }
                    else if (EndT < DateTime.Now) //已完結
                    {
                        Vote = "已完結";
                    }
                }
                MessageBox.Show($"提醒您問卷將送出，請確認", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Add(Heading, Content, StartT, EndT, QuestionnaireNum, Vote, Account);
            }
            else  //=>更新問卷
            {
                int QuestionnaireID = Convert.ToInt32(this.Request.QueryString["ID"]);

                if (ckbActivated.Checked == true)  //狀態要顯示已啟用
                {
                    Vote = "投票中";
                }
                else
                {
                    if (StartT > DateTime.Now) //尚未開始
                    {
                        Vote = "尚未開始";

                    }
                    else if (EndT < DateTime.Now) //已完結
                    {
                        Vote = "已完結";                        
                    }
                }
                MessageBox.Show($"提醒您問卷將送出，請確認", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpData(Heading, Content, Vote, StartT, EndT, QuestionnaireID);
            }
            Response.Redirect("/UserSide/USList.aspx");
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
        /// 取得已有問卷資料
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public static DataRow GetDBData(int QuestionnaireID)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Heading],[Vote],[StartTime],[EndTime],[Content]
                     FROM [Outline]
                     WHERE[QuestionnaireID] = @QuestionnaireID
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", QuestionnaireID));

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
        /// <summary>
        /// 更新既有問卷
        /// </summary>
        /// <param name="Heading"></param>
        /// <param name="Content"></param>
        /// <param name="Vote"></param>
        /// <param name="StartT"></param>
        /// <param name="EndT"></param>
        /// <param name="QuestionnaireID"></param>
        /// <returns></returns>
        public static DataTable UpData(string Heading, string Content, string Vote, DateTime StartT, DateTime EndT, int QuestionnaireID)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"UPDATE [Outline]
                    SET [Heading] = @Heading, [Vote] = @Vote, [StartTime] = @StartTime
                    	,[EndTime] = @EndTime, [Content] = @Content
                    WHERE [QuestionnaireID] = @QuestionnaireID;
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", QuestionnaireID));
            list.Add(new SqlParameter("@Heading", Heading));
            list.Add(new SqlParameter("@Content", Content));
            list.Add(new SqlParameter("@StartTime", StartT));
            list.Add(new SqlParameter("@EndTime", EndT));
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
        /// 問卷 - 寫進資料庫
        /// </summary>
        /// <param name="IDNumber"></param>
        /// <returns></returns>
        public static DataTable Add(string Heading, string Content, DateTime StartT, DateTime EndT, Guid QuestionnaireNum, string Vote, string Account)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" INSERT INTO Outline (Heading, Content, StartTime, EndTime, QuestionnaireNum, Vote, Account)
                    VALUES (@Heading, @Content, @StartTime, @EndTime, NEWID(), @Vote, @Account )
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Heading", Heading));
            list.Add(new SqlParameter("@Content", Content));
            list.Add(new SqlParameter("@StartTime", StartT));
            list.Add(new SqlParameter("@EndTime", EndT));
            list.Add(new SqlParameter("@QuestionnaireNum", QuestionnaireNum));
            list.Add(new SqlParameter("@Vote", Vote)); 
            list.Add(new SqlParameter("@Account", Account));

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
        /// 問卷 - 取得新增問卷的QuestionnaireID
        /// </summary>
        /// <param name="Heading"></param>
        /// <param name="Content"></param>
        /// <param name="StartT"></param>
        /// <param name="EndT"></param>
        /// <param name="QuestionnaireNum"></param>
        /// <param name="Vote"></param>
        /// <returns></returns>
        public static DataRow GetQuestionnaireID(string Heading, string Content, DateTime StartT, DateTime EndT, string Account)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [QuestionnaireID]
                    FROM [Outline]
                    WHERE [Heading] = @Heading　
                      AND [Content] = @Content AND [StartTime] = @StartTime 
                      AND [EndTime] = @EndTime AND [Account] = @Account
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Heading", Heading));
            list.Add(new SqlParameter("@Content", Content));
            list.Add(new SqlParameter("@StartTime", StartT));
            list.Add(new SqlParameter("@EndTime", EndT));
            list.Add(new SqlParameter("@Account", Account));

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
        #endregion

        #region 問題
        /// <summary>
        /// 修正問卷顯示 文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void givQuestion_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string tt = (string)DataBinder.Eval(e.Row.DataItem, "TopicType");
                if (tt == "CB")
                {
                    e.Row.Cells[3].Text = "複選方塊";
                }
                else if (tt == "RB")
                {
                    e.Row.Cells[3].Text = "單選方塊";
                }
                else
                {
                    e.Row.Cells[3].Text = "文字";
                }

                CheckBox gQckb = e.Row.Cells[4].FindControl("chbMustKeyIn") as CheckBox;
                string mk = (string)DataBinder.Eval(e.Row.DataItem, "TopicMustKeyIn");
                if (mk == "Y")
                {
                    gQckb.Checked = true;
                    e.Row.Cells[5].Visible = false;
                }
                else
                {
                    gQckb.Checked = false;
                    e.Row.Cells[5].Visible = false;
                }
                givQuestion.HeaderRow.Cells[5].Visible = false;
            }
        }
        /// <summary>
        /// 按編輯送資料到txtbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void givQuestion_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            this.txtQuestion.Text = givQuestion.Rows[e.RowIndex].Cells[2].Text;

            if (givQuestion.Rows[e.RowIndex].Cells[3].Text == "單選方塊")
            {
                this.ddlChoose.SelectedIndex = 0;
            }
            else if (givQuestion.Rows[e.RowIndex].Cells[3].Text == "複選方塊")
            {
                this.ddlChoose.SelectedIndex = 1;
            }
            else
            {
                this.ddlChoose.SelectedIndex = 2;
            }

            if (givQuestion.Rows[e.RowIndex].Cells[5].Text == "Y")
            {
                this.ckbRequired.Checked = true;
            }
            else
            {
                this.ckbRequired.Checked = false;
            }

            int QuestionnaireID = Convert.ToInt32(this.Request.QueryString["ID"]);
            int TopicNum = Convert.ToInt32(givQuestion.Rows[e.RowIndex].Cells[1].Text);
            var dr = GetGivAnsDBDataRow(QuestionnaireID, TopicNum);
            string a1 = dr["answer1"].ToString();
            string a2 = dr["answer2"].ToString();
            string a3 = dr["answer3"].ToString();
            string a4 = dr["answer4"].ToString();
            string a5 = dr["answer5"].ToString();
            string a6 = dr["answer6"].ToString();
            string a7 = dr["answer7"].ToString();
            string a8 = dr["answer8"].ToString();
            string a9 = dr["answer9"].ToString();
            string a10 = dr["answer10"].ToString();
            int noq = Convert.ToInt32(dr["OptionsAll"].ToString());
            if (noq == 10)
            {
                this.txtOptions.Text = a1 + ";" + a2 + ";" + a3 + ";" + a4 + ";" + a5 + ";" + a6 + ";" + a7 + ";" + a8 + ";" + a9 + ";" + a10;
            }
            if (noq == 9)
            {
                this.txtOptions.Text = a1 + ";" + a2 + ";" + a3 + ";" + a4 + ";" + a5 + ";" + a6 + ";" + a7 + ";" + a8 + ";" + a9;
            }
            if (noq == 8)
            {
                this.txtOptions.Text = a1 + ";" + a2 + ";" + a3 + ";" + a4 + ";" + a5 + ";" + a6 + ";" + a7 + ";" + a8;
            }
            if (noq == 7)
            {
                this.txtOptions.Text = a1 + ";" + a2 + ";" + a3 + ";" + a4 + ";" + a5 + ";" + a6 + ";" + a7;
            }
            if (noq == 6)
            {
                this.txtOptions.Text = a1 + ";" + a2 + ";" + a3 + ";" + a4 + ";" + a5 + ";" + a6;
            }
            if (noq == 5)
            {
                this.txtOptions.Text = a1 + ";" + a2 + ";" + a3 + ";" + a4 + ";" + a5;
            }
            if (noq == 4)
            {
                this.txtOptions.Text = a1 + ";" + a2 + ";" + a3 + ";" + a4;
            }
            if (noq == 3)
            {
                this.txtOptions.Text = a1 + ";" + a2 + ";" + a3;
            }
            if (noq == 2)
            {
                this.txtOptions.Text = a1 + ";" + a2;
            }
            if (noq == 1)
            {
                this.txtOptions.Text = a1;
            }
        }

        /// <summary>
        /// 問題 - 加入到表裡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //沒有顯示
        protected void btnAddIn_Click(object sender, EventArgs e)
        {
            string QuestionType;
            if (ddlType.SelectedIndex == 1)   //要從資料庫呼出來
            {
                
                QuestionType = "常用問題1";
            }
            else
            {
                QuestionType = "自訂問題";

                if (string.IsNullOrEmpty(txtQuestion.Text) || string.IsNullOrEmpty(txtOptions.Text))
                {
                    this.lblAddMessage.Text = "問題 和 回答 不得空白";
                    return;
                }

                List<AddInGivQuestionList> gpList;

                if (this.Session["GivQuestion"] != null)
                {
                    gpList = this.Session["GivQuestion"] as List<AddInGivQuestionList>;
                }
                else
                {
                    gpList = new List<AddInGivQuestionList>();
                    this.Session["GivQuestion"] = gpList;
                }


                int Num = 1;
                if (Num != 1)
                {
                    Num += 1;
                }

                string Question = this.txtQuestion.Text;

                string QuestChoose;
                if (ddlChoose.SelectedIndex == 0)
                {
                    QuestChoose = "單選方塊";
                }
                else if (ddlChoose.SelectedIndex == 1)
                {
                    QuestChoose = "複選方塊";
                }
                else
                {
                    QuestChoose = "文字";
                }

                bool ckbRe;
                if (ckbRequired.Checked)
                {
                    ckbRe = true;
                }
                else
                {
                    ckbRe = false;
                }

                string Options = this.txtOptions.Text;

                //var gpList = new AddInGivQuestionList()
                //{
                //   TopicNum = Num,
                //   TopicDescription = Question,
                //   TopicType = QuestChoose,
                //   TopicMustKeyIn = ckbRe,
                //   Options = Options
                //};


                this.givQuestion.DataSource = gpList;
                this.givQuestion.DataBind();
            }
            
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



        /// <summary>
        /// 取得已有問卷資料
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public static DataTable GetGivDBData(int QuestionnaireID)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Questionnaires].[TopicNum],[TopicDescription],[TopicSummary],[TopicType],[TopicMustKeyIn]
                   	     ,[Question].[answer1],[Question].[answer2],[Question].[answer3],[Question].[answer4],[Question].[answer5]
                         ,[Question].[answer6],[Question].[answer7],[Question].[answer8],[Question].[answer9],[Question].[answer10]
                   	     ,[Question].[OptionsAll]
                     FROM [Questionnaire].[dbo].[Questionnaires]
                     RIGHT JOIN [Question] ON [Questionnaires].[TopicNum] = [Question].[TopicNum]
                     WHERE [Questionnaires].[QuestionnaireID] = @QuestionnaireID
                     ORDER BY [Questionnaires].[TopicNum]
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", QuestionnaireID));

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
        public static DataRow GetGivDBDataRow(int QuestionnaireID)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Questionnaires].[TopicNum],[TopicDescription],[TopicSummary],[TopicType],[TopicMustKeyIn]
                   	     ,[Question].[answer1],[Question].[answer2],[Question].[answer3],[Question].[answer4],[Question].[answer5]
                         ,[Question].[answer6],[Question].[answer7],[Question].[answer8],[Question].[answer9],[Question].[answer10]
                   	     ,[Question].[OptionsAll]
                     FROM [Questionnaire].[dbo].[Questionnaires]
                     RIGHT JOIN [Question] ON [Questionnaires].[TopicNum] = [Question].[TopicNum]
                     WHERE [Questionnaires].[QuestionnaireID] = @QuestionnaireID
                     ORDER BY [Questionnaires].[TopicNum]
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", QuestionnaireID));

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
        public static DataRow GetGivAnsDBDataRow(int QuestionnaireID, int TopicNum)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Questionnaires].[TopicNum],[TopicDescription],[TopicSummary],[TopicType],[TopicMustKeyIn]
                   	     ,[Question].[answer1],[Question].[answer2],[Question].[answer3],[Question].[answer4],[Question].[answer5]
                         ,[Question].[answer6],[Question].[answer7],[Question].[answer8],[Question].[answer9],[Question].[answer10]
                   	     ,[Question].[OptionsAll]
                     FROM [Questionnaire].[dbo].[Questionnaires]
                     RIGHT JOIN [Question] ON [Questionnaires].[TopicNum] = [Question].[TopicNum]
                     WHERE [Questionnaires].[QuestionnaireID] = @QuestionnaireID
                     AND [Questionnaires].[TopicNum] = @TopicNum
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", QuestionnaireID));
            list.Add(new SqlParameter("@TopicNum", TopicNum));

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
        #endregion

        #region 填寫資料
        /// <summary>
        /// 填寫資料 - 匯出紐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt = GetAnsRecordDetailsData();//獲取datatable資料源
            string title = "問卷資料" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";//匯出的檔案名
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
            list.Add(new SqlParameter("@RecordNum", RecordNum)); ;

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
                          ,[Record Details].[ReplicationNum]
	                      ,[RDAns]
                      FROM [Questionnaire].[dbo].[Record]
                      JOIN [Questionnaire].[dbo].[Outline]
                      ON [Record].[QuestionnaireID] = [Outline].[QuestionnaireID]
                      JOIN [Questionnaire].[dbo].[Record Details]
                      ON [Record].RecordNum = [Record Details].RecordNum
                      JOIN[Questionnaire].[dbo].[Questionnaires]
                      ON [Record Details].[TopicNum] = [Questionnaires].[TopicNum]  
					  ORDER BY [Record].[RecordNum],[Record Details].[TopicNum]
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




        #endregion


    }
}