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
    public partial class CSConfirmation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string IDNumber = this.Request.QueryString["ID"];
            this.reHeading.DataSource = GetHeading(IDNumber);
            this.reHeading.DataBind();

            this.ltlName.Text = Session["Name"] as string;
            this.ltlPhone.Text = Session["Phone"] as string;
            this.ltlEmail.Text = Session["Email"] as string;
            this.ltlAge.Text = Session["Age"] as string;
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
                    WHERE Number = @IDNumber
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@IDNumber", IDNumber));

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
        //跳得回去但沒有帶Session =>未實作
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Session["Name"] = this.ltlName.Text;
            this.Session["Phone"] = this.ltlPhone.Text;
            this.Session["Email"] = this.ltlEmail.Text;
            this.Session["Age"] = this.ltlAge.Text;
            Response.Redirect("/ClientSide/CSPage.aspx");
        }

        /// <summary>
        /// 送出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSent_Click(object sender, EventArgs e)
        {
            string Name = this.ltlName.Text;
            int Phone = Convert.ToInt32(this.ltlPhone.Text);
            string Email = this.ltlEmail.Text;
            int Age = Convert.ToInt32(this.ltlAge.Text);

            //詢問是否送出
            DialogResult result = MessageBox.Show($"請確認是否送出問卷", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                sentUserInformation(Name, Phone, Email, Age);
            }
            MessageBox.Show($"資料送出成功，即將返回列表頁", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Response.Redirect("/ClientSide/CSList.aspx");
        }

        /// <summary>
        /// 寫進資料庫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static DataTable sentUserInformation(string Name, int Phone, string Email, int Age)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" INSERT INTO [Information] ([UserID], [Name], [Phone], [Email], [Age])
                    SELECT newid() AS [UserID], @Name, @Phone, @Email, @Age
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Name", Name));
            list.Add(new SqlParameter("@Phone", Phone));
            list.Add(new SqlParameter("@Email", Email));
            list.Add(new SqlParameter("@Age", Age));

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

        //這邊還有問卷問題的選擇
    }
}