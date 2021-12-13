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
using Button = System.Web.UI.WebControls.Button;
using Label = System.Web.UI.WebControls.Label;


namespace Dynamic_questionnaire_system.UserSide
{
    public partial class USDetail : System.Web.UI.Page
    {
        string ConnStr = DBHelper.GetConnectionString();
        private int M_ID = 0;
        private int D1_ID = 0;
        private string D1_TITLE, D1_MustKeyIn, D1_SUMMARY;

        protected void Page_Load(object sender, EventArgs e)
        {
            int IDNumber = Convert.ToInt32(this.Request.QueryString["ID"]);

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
                var dr = ContextManager.GetDBData(IDNumber);
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
                this.givQuestion.DataSource = GetGivDBData(IDNumber);
                this.givQuestion.DataBind();

                //後台內頁2-問題
                if (ddlType.SelectedIndex == 1)   //常用問題1 =>常用問題設定要去常用問題管理，要記得處理
                {
                    this.txtQuestion.Text = "";
                    this.txtOptions.Text = "";
                }
            }

            //後台內頁3-填寫資料
            this.givExport.DataSource = ContextManager.GetRecordData();  //做方法
            this.givExport.DataBind();
            var dt = ContextManager.GetRecordData();
            DataSearch(dt);

            //顯示詳細資料
            if (!IsPostBack)
            {
                this.PlaceHolderDetail.Visible = true;

                if (Request.QueryString["ID"] != null && Request.QueryString["RN"] != null)
                {
                    if (this.PlaceHolderDetail.Visible == true)
                    {
                        this.PlaceHolderExport.Visible = false;
                        this.btnreturn.Visible = true;
                    }

                    int RecordNum = Convert.ToInt32(Request.QueryString["RN"].ToString());
                    var gdd = ContextManager.GetRecordDetailsData(RecordNum);

                    this.plblName.Visible = true;
                    this.plblPhone.Visible = true;
                    this.plblEmail.Visible = true;
                    this.plblAge.Visible = true;
                    this.lblWriteT.Visible = true;

                    this.txtName.Visible = true;
                    this.txtPhone.Visible = true;
                    this.txtEmail.Visible = true;
                    this.txtAge.Visible = true;
                    this.lblAnsT.Visible = true;

                    this.txtName.Text = gdd["AnsName"].ToString();
                    this.txtPhone.Text = gdd["AnsPhone"].ToString();
                    this.txtEmail.Text = gdd["AnsEmail"].ToString();
                    this.txtAge.Text = gdd["AnsAge"].ToString();
                    this.lblAnsT.Text = Convert.ToDateTime(gdd["AnsTime"]).ToString("yyyy/MM/dd HH:mm:ss");
                    Generate_Page(IDNumber, RecordNum);
                }
            }

            #region 統計資料

            var tb = ContextManager.GetStatisticsDBSourceTB(IDNumber);
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
                        answer1Value = 0,
                        answer2Value = 0,
                        answer3Value = 0,
                        answer4Value = 0,
                        answer5Value = 0,
                        answer6Value = 0,

                        answer1percentage = "",
                        answer2percentage = "",
                        answer3percentage = "",
                        answer4percentage = "",
                        answer5percentage = "",
                        answer6percentage = "",
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
                            dict[TopicNum].answer1Value += 1;
                        }
                        else if (sub == dict[TopicNum].answer2)
                        {
                            dict[TopicNum].answer2Value += 1;
                        }
                        else if (sub == dict[TopicNum].answer3)
                        {
                            dict[TopicNum].answer3Value += 1;
                        }
                        else if (sub == dict[TopicNum].answer4)
                        {
                            dict[TopicNum].answer4Value += 1;
                        }
                        else if (sub == dict[TopicNum].answer5)
                        {
                            dict[TopicNum].answer5Value += 1;
                        }
                        else if (sub == dict[TopicNum].answer6)
                        {
                            dict[TopicNum].answer6Value += 1;
                        }
                    }
                    double percentageA1 = Convert.ToDouble(dict[TopicNum].answer1Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA2 = Convert.ToDouble(dict[TopicNum].answer2Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA3 = Convert.ToDouble(dict[TopicNum].answer3Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA4 = Convert.ToDouble(dict[TopicNum].answer4Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA5 = Convert.ToDouble(dict[TopicNum].answer5Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA6 = Convert.ToDouble(dict[TopicNum].answer6Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    
                    dict[TopicNum].answer1percentage = $"{percentageA1}%";
                    dict[TopicNum].answer2percentage = $"{percentageA2}%";
                    dict[TopicNum].answer3percentage = $"{percentageA3}%";
                    dict[TopicNum].answer4percentage = $"{percentageA4}%";
                    dict[TopicNum].answer5percentage = $"{percentageA5}%";
                    dict[TopicNum].answer6percentage = $"{percentageA6}%";

                }
                else if (dict[TopicNum].TopicType == "RB") //單選題
                {
                    string s = (string)dr["RDAns"];
                    if (s == dict[TopicNum].answer1)
                    {
                        dict[TopicNum].answer1Value += 1;
                    }
                    else if (s == dict[TopicNum].answer2)
                    {
                        dict[TopicNum].answer2Value += 1;
                    }
                    else if (s == dict[TopicNum].answer3)
                    {
                        dict[TopicNum].answer3Value += 1;
                    }
                    else if (s == dict[TopicNum].answer4)
                    {
                        dict[TopicNum].answer4Value += 1;
                    }
                    else if (s == dict[TopicNum].answer5)
                    {
                        dict[TopicNum].answer5Value += 1;
                    }
                    else if (s == dict[TopicNum].answer6)
                    {
                        dict[TopicNum].answer6Value += 1;
                    }

                    double percentageA1 = Convert.ToDouble(dict[TopicNum].answer1Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA2 = Convert.ToDouble(dict[TopicNum].answer2Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA3 = Convert.ToDouble(dict[TopicNum].answer3Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA4 = Convert.ToDouble(dict[TopicNum].answer4Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA5 = Convert.ToDouble(dict[TopicNum].answer5Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;
                    double percentageA6 = Convert.ToDouble(dict[TopicNum].answer6Value) / Convert.ToDouble(dict[TopicNum].answer1Value + dict[TopicNum].answer2Value + dict[TopicNum].answer3Value + dict[TopicNum].answer4Value + dict[TopicNum].answer5Value + dict[TopicNum].answer6Value) * 100;

                    dict[TopicNum].answer1percentage = $"{percentageA1}%";
                    dict[TopicNum].answer2percentage = $"{percentageA2}%";
                    dict[TopicNum].answer3percentage = $"{percentageA3}%";
                    dict[TopicNum].answer4percentage = $"{percentageA4}%";
                    dict[TopicNum].answer5percentage = $"{percentageA5}%";
                    dict[TopicNum].answer6percentage = $"{percentageA6}%";
                }
                else
                {
                    Label label = new Label();
                    label.Text = "文字方塊 不做統計";
                }

            }
            this.reTopicDescription.DataSource = dict.Values;
            this.reTopicDescription.DataBind();
            #endregion
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
                ContextManager.Add(Heading, Content, StartT, EndT, QuestionnaireNum, Vote, Account);
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
                ContextManager.UpData(Heading, Content, Vote, StartT, EndT, QuestionnaireID);
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

            int IDNumber = Convert.ToInt32(this.Request.QueryString["ID"]);
            int TopicNum = Convert.ToInt32(givQuestion.Rows[e.RowIndex].Cells[1].Text);
            var dr = GetGivAnsDBDataRow(IDNumber, TopicNum);
            string a1 = dr["answer1"].ToString();
            string a2 = dr["answer2"].ToString();
            string a3 = dr["answer3"].ToString();
            string a4 = dr["answer4"].ToString();
            string a5 = dr["answer5"].ToString();
            string a6 = dr["answer6"].ToString();
            int noq = Convert.ToInt32(dr["OptionsAll"].ToString());
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
            Response.Redirect($"USDetail.aspx?ID={Request.QueryString["ID"]}");
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
                         ,[Question].[answer6]
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
                         ,[Question].[answer6]
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
                         ,[Question].[answer6]
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
            DataTable dt = ContextManager.GetAnsRecordDetailsData();//獲取datatable資料源
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

        protected void btnreturn_Click(object sender, EventArgs e)
        {
            Response.Redirect($"USDetail.aspx?ID={Request.QueryString["ID"]}");
            this.PlaceHolderExport.Visible = true;
            this.PlaceHolderDetail.Visible = false;
        }





        #region 填寫資料 - 顯示問卷填寫細節
        public void Generate_Page(int IDNumber, int RecordNum)
        {
            var tb = ContextManager.USGetStatisticsDBSourceTB(IDNumber, RecordNum);
            SqlConnection Conn = new SqlConnection(ConnStr);
            Conn.Open();   //-- 連結DB

            M_ID = IDNumber;
            SqlCommand cmd1 = new SqlCommand("SELECT * From [Questionnaires] WHERE [QuestionnaireID] = @QuestionnaireID", Conn);
            cmd1.Parameters.Add("@QuestionnaireID", SqlDbType.Int).Value = M_ID;
            SqlDataReader dr1 = cmd1.ExecuteReader();

            #region
            //**** "讀取" 這一份問卷的「每一個題目」。Questionnaires資料表 ****

            if (dr1.HasRows)
            {
                Label Label_table_start = new Label();
                Label_table_start.Text = "<table border=\"1\" width=\"480px\" id=\"table1\" style=\"border: 3px dotted #000080\">";
                AnsDetail.Controls.Add(Label_table_start);

                int table_i = 0;

                while (dr1.Read())
                {
                    D1_ID = (int)dr1["TopicNum"];
                    D1_TITLE = dr1["TopicDescription"].ToString();
                    D1_MustKeyIn = dr1["TopicMustKeyIn"].ToString();

                    if (DBNull.Value.Equals(dr1["TopicSummary"]))
                    {
                        D1_SUMMARY = dr1["TopicSummary"].ToString();
                    }
                    else
                    {
                        D1_SUMMARY = "";
                    }

                    string D1_TYPE = dr1["TopicType"].ToString();

                    //**** "產生" 這一份問卷的「每一個題目」。Questionnaires資料表 ****

                    Label Label_table_tr = new Label();
                    if ((table_i % 2) == 0)
                    {
                        Label_table_tr.Text = "<tr><td>";
                    }
                    else
                    {
                        Label_table_tr.Text = "<tr><td bgcolor='#E3E6FD'>";
                    }
                    AnsDetail.Controls.Add(Label_table_tr);

                    //---- (3-1). 共用的部分 -------------------------------------------
                    //--  用來產生每一個題目的「主題」、「說明（摘要）」、是否必填？
                    Generate_D1_Common(D1_ID, D1_TITLE, D1_MustKeyIn, D1_SUMMARY);

                    //---- (3-2). 差異的部分 -------------------------------------------
                    switch (D1_TYPE)   //-- 題目是單選、複選、文字輸入？
                    {
                        case "CB":  //-- 複選（CheckBoxList）
                                    //---------------------------------------------------------------------------(start)
                            CheckBoxList CB_Q1 = new CheckBoxList();
                            CB_Q1.ID = "D1_" + D1_ID;

                            //-- 單/複選的子選項，記錄在 Question資料表裡面。
                            SqlCommand cmd2 = new SqlCommand("SELECT * FROM [Question] WHERE [TopicNum] = @TopicNum AND [QuestionnaireID] = @QuestionnaireID", Conn);
                            cmd2.Parameters.Add("@TopicNum", SqlDbType.Int).Value = D1_ID;
                            cmd2.Parameters.Add("@QuestionnaireID", SqlDbType.Int).Value = M_ID;

                            SqlDataReader dr2 = cmd2.ExecuteReader();  //-- 執行SQL指令。
                            dr2.Read();  //-- 只讀一列記錄。

                            for (int i = 1; i <= (int)dr2["OptionsAll"]; i++)   //-- 看看這個問題（單/複選）有幾個子選項？
                            {
                                string answer_item = "answer" + i;
                                CB_Q1.Items.Add(dr2[answer_item].ToString());

                                foreach (DataRow dr in tb.Rows)
                                {
                                    string s = (string)dr["RDAns"];
                                    string[] subs = s.Split(';');
                                    foreach (string sub in subs)
                                    {
                                        ListItem newItme = (ListItem)CB_Q1.Items.FindByText(dr2[answer_item].ToString());
                                        if (newItme != null)
                                            if(sub == newItme.ToString())
                                            newItme.Selected = true;
                                        newItme.Enabled = false;
                                    }
                                }
                            }
                            //---------------------------------------------------------------------------(end)
                            
                            AnsDetail.Controls.Add(CB_Q1);  //-- 動態加入畫面（PlaceHolder1）之中
                            cmd2.Cancel();  //-- 用完就立即關閉資源。
                            dr2.Close();

                            Label Label_br = new Label();
                            Label_br.Text = "<br />";
                            AnsDetail.Controls.Add(Label_br);
                            break;

                        case "RB":  //-- 單選（RadioButton）
                                    //---------------------------------------------------------------------------(start)
                            RadioButtonList CB_Q2 = new RadioButtonList();
                            CB_Q2.ID = "D1_" + D1_ID;

                            //-- 單/複選的子選項，記錄在 Question_D2資料表裡面。
                            SqlCommand cmd3 = new SqlCommand("SELECT * FROM [Question] WHERE [TopicNum] = @TopicNum AND [QuestionnaireID] = @QuestionnaireID", Conn);
                            cmd3.Parameters.Add("@TopicNum", SqlDbType.Int).Value = D1_ID;
                            cmd3.Parameters.Add("@QuestionnaireID", SqlDbType.Int).Value = M_ID;

                            SqlDataReader dr3 = cmd3.ExecuteReader();  //-- 執行SQL指令。
                            dr3.Read();  //-- 只讀一列記錄。

                            for (int i = 1; i <= (int)dr3["OptionsAll"]; i++)   //-- 看看這個問題（單/複選）有幾個子選項？
                            {
                                string answer_item = "answer" + i;
                                CB_Q2.Items.Add(dr3[answer_item].ToString());

                                foreach (DataRow dr in tb.Rows)
                                {
                                    ListItem newItme = (ListItem)CB_Q2.Items.FindByText(dr3[answer_item].ToString());
                                    if (newItme != null)
                                        if(dr["RDAns"] as string == newItme.ToString())
                                        newItme.Selected = true;
                                    newItme.Enabled = false;
                                }                                   
                            }
                            //---------------------------------------------------------------------------(end)

                            cmd3.Cancel();  //-- 用完就立即關閉資源。
                            dr3.Close();
                            AnsDetail.Controls.Add(CB_Q2);  //-- 動態加入畫面（PlaceHolder1）之中

                            Label Label_br1 = new Label();  //-- 只是為了畫面美觀而已。
                            Label_br1.Text = "<br />";
                            AnsDetail.Controls.Add(Label_br1);
                            break;

                        default:  //-- 其他 文字輸入（TB，TextBox）=>待驗證
                            System.Web.UI.WebControls.TextBox CB_Q3 = new System.Web.UI.WebControls.TextBox();
                            CB_Q3.ID = "D1_" + D1_ID;

                            foreach (DataRow dr in tb.Rows)
                            {
                                if (dr["RDAns"] as string == CB_Q3.ToString())
                                    AnsDetail.Controls.Add(CB_Q3);  //-- 動態加入畫面（PlaceHolder1）之中
                            }
                            

                            Label Label_br2 = new Label();
                            Label_br2.Text = "<br /><br />";
                            AnsDetail.Controls.Add(Label_br2);
                            break;
                    }  // End of switch case

                    Label Label_table_td = new Label();
                    Label_table_td.Text = "</td></tr>";
                    AnsDetail.Controls.Add(Label_table_td);

                    table_i += 1;
                }

                Label Label_table_end = new Label();
                Label_table_end.Text = "</table>";
                AnsDetail.Controls.Add(Label_table_end);
            }
            cmd1.Cancel();  //-- 用完就立即關閉資源。
            dr1.Close();

            Conn.Close();
            Conn.Dispose();
            #endregion

            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
                Conn.Dispose();
            }

        }

        //== 產生每一個問題的共同項目，例如：「標題」、「摘要」、「是否必填？」
        //== Questionnaires資料表。
        protected void Generate_D1_Common(int D1_ID, string D1_TITLE, string D1_MustKeyIn, string D1_SUMMARY)
        {
            Label LB_title = new Label();
            LB_title.ID = "Label_D1title_" + D1_ID;
            LB_title.Text = "<b>" + D1_TITLE + "</b>";   //-- 每一個問題的「標題」
            if (D1_MustKeyIn == "Y")
            {
                LB_title.Text += "&nbsp;&nbsp;<font color=red><b>*（必填）</b></font>";
                //-- 每一個問題的「標題」。強調「必填」！
            }
            AnsDetail.Controls.Add(LB_title);   //-- 動態加入畫面（PlaceHolder1）之中

            Label LB_summary = new Label();
            LB_summary.ID = "Label_D1summary_" + D1_ID;
            LB_summary.Text = "<br /><font color='#484848'><small>" + D1_SUMMARY + "</small></font><br />";  //-- 每一個問題的「摘要」
            AnsDetail.Controls.Add(LB_summary);  //-- 動態加入畫面（PlaceHolder1）之中
        }
        #endregion

        #endregion

    }
}
