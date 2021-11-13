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

namespace Dynamic_questionnaire_system.ClientSide
{
    public partial class CSPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["ID"] == null) { Response.Redirect("/ClientSide/CSList.aspx"); }
            string IDNumber = this.Request.QueryString["ID"];
            this.reHeading.DataSource = GetHeading(IDNumber);
            this.reHeading.DataBind();

            this.reContent.DataSource = GetContent(IDNumber);
            this.reContent.DataBind();

            //this.reTopic.DataSource = GetTopic(IDNumber);
            //this.reTopic.DataBind();

            //this.reOptions.DataSource = GetOptions(IDNumber);
            //this.reOptions.DataBind();
        }

        /// <summary>
        /// 顯示資料
        /// </summary>
        /// <returns></returns>
        //標題
        public static DataTable GetHeading(string IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Heading],[Vote],[StartTime],[EndTime]
                    FROM [Outline]
                    WHERE [Outline].[QuestionnaireID] = @QuestionnaireID
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
        //內容
        public static DataTable GetContent(string IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Content]
                    FROM [Outline]
                    WHERE [Outline].[QuestionnaireID] = @QuestionnaireID
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
        //問卷題目
        public static DataTable GetTopic(string IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [TopicNum],[TopicDescription],[TopicSummary],[TopicType],[TopicMustKeyIn]
                     FROM [Questionnaires]
                    WHERE [Questionnaires].[QuestionnaireID] = @QuestionnaireID
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
        //問卷選項
        public static DataTable GetOptions(string IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Questionnaires].[TopicNum], [TopicDescription],[OptionsNum],[OptionsDescription]
                    FROM [Questionnaires]
                    JOIN Question ON [Question].[TopicNum] = [Questionnaires].[TopicNum]
                    WHERE [Questionnaires].[QuestionnaireID] = @QuestionnaireID
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

        /// <summary>
        /// 按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Write("<script> alert('將返回列表頁') </script>");
            //MessageBox.Show($"將返回列表頁", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Response.Redirect("/ClientSide/CSList.aspx");
        }

        protected void btnSent_Click(object sender, EventArgs e)
        {
            if (this.Session["Name"] != null || this.Session["Phone"] != null || this.Session["Email"] != null || this.Session["Age"] != null)
            {
                this.txtbName.Text = this.Session["Name"] as string;
                this.txtbPhone.Text = this.Session["Phone"] as string;
                this.txtbEmail.Text = this.Session["Email"] as string;
                this.txtbAge.Text = this.Session["Age"] as string;
            }
            if (string.IsNullOrEmpty(this.txtbName.Text) || string.IsNullOrEmpty(this.txtbPhone.Text) || string.IsNullOrEmpty(this.txtbEmail.Text) || string.IsNullOrEmpty(this.txtbAge.Text))
            {
                this.plcNoWriteData.Visible = true;
            }
            else
            {
                this.plcNoWriteData.Visible = false;

                this.Session["Name"] = this.txtbName.Text;
                this.Session["Phone"] = this.txtbPhone.Text;
                this.Session["Email"] = this.txtbEmail.Text;
                this.Session["Age"] = this.txtbAge.Text;

                MessageBox.Show($"即將前往確認頁面，請確認填寫的資訊是否正確", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Response.Redirect("/ClientSide/CSConfirmation.aspx");
            }
        }

        protected void Generate_Page()
        {
            string IDNumber = this.Request.QueryString["ID"];

            string connStr = DBHelper.GetConnectionString();
            SqlConnection Conn = new SqlConnection(connStr);
            Conn.Open();

            SqlCommand cmd1 = new SqlCommand("SELECT * FROM[Questionnaires] WHERE[Questionnaires].[QuestionnaireID] = @QuestionnaireID", Conn);
            cmd1.Parameters.AddWithValue("@QuestionnaireID", IDNumber);
            SqlDataReader dr1 = cmd1.ExecuteReader();

            if (dr1.HasRows) 
            {
                System.Web.UI.WebControls.Label Lable_table_start = new System.Web.UI.WebControls.Label();
                Lable_table_start.Text = "";
                PlaceHolder1.Controls.Add(Lable_table_start);

                int table_i = 0;

                while (dr1.Read())
                {
                    int D1_ID = (int)dr1["TopicNum"];
                    string D1_TITLE = dr1["TopicDescription"].ToString();
                    string D1_MustKetIn = dr1["TopicMustKeyIn"].ToString();

                    if (DBNull.Value.Equals(dr1["TopicSummary"]))
                    {
                        string D1_SUMMARY = dr1["TopicSummary"].ToString();
                    }
                    else 
                    {
                        string D1_SUMMARY = "";
                    }
                    String D1_TYPE = dr1["TopicType"].ToString();
                }
            }
        }
    }
}