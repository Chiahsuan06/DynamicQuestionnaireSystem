using DBSource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Label = System.Web.UI.WebControls.Label;

namespace Dynamic_questionnaire_system.ClientSide
{
    public partial class CSConfirmation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["ID"] == null)
            {
                Response.Redirect("/ClientSide/CSList.aspx");
            }
            string IDNumber = this.Request.QueryString["ID"];

            this.reHeading.DataSource = GetHeading(IDNumber);
            this.reHeading.DataBind();

            this.ltlName.Text = Session["Name"] as string;
            this.ltlPhone.Text = Session["Phone"] as string;
            this.ltlEmail.Text = Session["Email"] as string;
            this.ltlAge.Text = Session["Age"] as string;

            // todo: 這裡還沒完成 =>構思是將問卷畫面坐起來，所以有些程式碼和上頁相同，照理已選的選項已經存在Session裡，又將其轉回ArrayList....如何讓其顯示???
            ArrayList M1_D1_D2 = Session["ListM1_D1_D2"] as ArrayList;
            foreach (var item in M1_D1_D2)
            {
                Response.Write(item);
                
            }


        }

        /// <summary>
        /// 顯示資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetHeading(string IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Heading]
                    FROM [Outline]
                    WHERE QuestionnaireID = @QuestionnaireID
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
        /// 會回到填寫頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>  
        //OK
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Session["CSCFIName"] = this.ltlName.Text;
            this.Session["CSCFIPhone"] = this.ltlPhone.Text;
            this.Session["CSCFIEmail"] = this.ltlEmail.Text;
            this.Session["CSCFIAge"] = this.ltlAge.Text;
            Response.Redirect("/ClientSide/CSPage.aspx?ID=" + Request.QueryString["ID"]);
        }

        /// <summary>
        /// 送出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSent_Click(object sender, EventArgs e)
        {
            int QuestionnaireID = Convert.ToInt32(this.Request.QueryString["ID"]);
            string Name = this.ltlName.Text;
            int Phone = Convert.ToInt32(this.ltlPhone.Text);
            string Email = this.ltlEmail.Text;
            int Age = Convert.ToInt32(this.ltlAge.Text);

            //詢問是否送出
            DialogResult result = MessageBox.Show($"請確認是否送出問卷", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                sentUserInformation(QuestionnaireID, Name, Phone, Email, Age);
                var dr = getNewUserInformation(QuestionnaireID, Name, Phone, Email, Age);
                int RecordNum = Convert.ToInt32(dr["RecordNum"].ToString());
                var TNRD = Session["TNRDList"] as List<QAList>;
                foreach (var item in TNRD)
                {
                    sentAnswers(QuestionnaireID, RecordNum, item.TopicNum, item.RDAns);
                }

                Session.Clear();
            }

            HttpContext.Current.Response.Write("<script> alert('資料送出成功，即將返回列表頁') </script>");
            Response.Redirect("/ClientSide/CSList.aspx");
        }

        /// <summary>
        /// 寫進資料庫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static DataTable sentUserInformation(int QuestionnaireID, string Name, int Phone, string Email, int Age)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" INSERT INTO [Record] ([QuestionnaireID], [AnswererID], [AnsName], [AnsPhone], [AnsEmail], [AnsAge], [AnsTime])
                    SELECT @QuestionnaireID, newid() AS [AnswererID], @AnsName, @AnsPhone, @AnsEmail, @AnsAge, GETDATE() AS [AnsTime]
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", QuestionnaireID));
            list.Add(new SqlParameter("@AnsName", Name));
            list.Add(new SqlParameter("@AnsPhone", Phone));
            list.Add(new SqlParameter("@AnsEmail", Email));
            list.Add(new SqlParameter("@AnsAge", Age));

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

        public static DataRow getNewUserInformation(int QuestionnaireID, string Name, int Phone, string Email, int Age)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [RecordNum]
                    FROM [Record]
                    WHERE [QuestionnaireID] = @QuestionnaireID AND [AnsName] = @AnsName AND 
                          [AnsPhone] = @AnsPhone AND [AnsEmail] = @AnsEmail AND [AnsAge] = @AnsAge
                    ORDER BY [AnsTime] DESC
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", QuestionnaireID));
            list.Add(new SqlParameter("@AnsName", Name));
            list.Add(new SqlParameter("@AnsPhone", Phone));
            list.Add(new SqlParameter("@AnsEmail", Email));
            list.Add(new SqlParameter("@AnsAge", Age));

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

        public static DataTable sentAnswers(int QuestionnaireID, int RecordNum, int TopicNum, string RDAns)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" INSERT INTO [Record Details] ([QuestionnaireID], [RecordNum], [TopicNum], [RDAns])
                    SELECT @QuestionnaireID, @RecordNum, @TopicNum, @RDAns
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", QuestionnaireID));
            list.Add(new SqlParameter("@RecordNum", RecordNum));
            list.Add(new SqlParameter("@TopicNum", TopicNum));
            list.Add(new SqlParameter("@RDAns", RDAns));

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