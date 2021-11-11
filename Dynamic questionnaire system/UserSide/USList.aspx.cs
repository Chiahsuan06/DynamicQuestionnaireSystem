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
using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace Dynamic_questionnaire_system.UserSide
{
    public partial class USList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.GridView1.DataSource = GetDBData();  ///讓GridView1顯示DB的資料
            this.GridView1.DataBind();
        }
        /// <summary>
        /// 搜尋區
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFind_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtStart.Text))
            {
                this.txtStart.Text = "1911/01/01";
            }
            if (string.IsNullOrEmpty(this.txtEnd.Text))
            {
                this.txtEnd.Text = "1911/12/31";
            }

            string findTitle = this.txtTitle.Text;
            DateTime findStart = Convert.ToDateTime(this.txtStart.Text);
            DateTime findEnd = Convert.ToDateTime(this.txtEnd.Text);

            if (findStart > findEnd)   //日期檢查
            {
                MessageBox.Show($"開始時間大於結束時間，請重新填寫", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.GridView1.DataSource = findData(findTitle, findStart, findEnd);
            this.GridView1.DataBind();
            var dt = findData(findTitle, findStart, findEnd);  //這個部分是否有缺??....
            DataSearch(dt);
        }

        private void DataSearch(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                var dtPaged = this.GetPagedDataTable(dt);

                this.GridView1.DataSource = dtPaged;
                this.GridView1.DataBind();
                //this.ucPager.Visible = true;
            }
            else
            {
                this.GridView1.Visible = false;
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "請重新搜尋";   //按搜尋，一直跳到這裡....
            }
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
        private DataTable GetPagedDataTable(DataTable dt)
        {
            DataTable dtPaged = dt.Clone();
            //int pageSize = this.ucPager.PageSize;

            int startIndex = (this.GetCurrentPage() - 1) * 10;
            int endIndex = (this.GetCurrentPage()) * 10;

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

        /// <summary>
        /// 顯示資料、判斷投票狀態(成功)
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDBData()
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"UPDATE [Outline]
	                SET [Vote] = '已完結'
	                WHERE [EndTime] < GETDATE()

                  UPDATE [Outline]
	                SET [Vote] = '尚未開始'
	                WHERE [StartTime] > GETDATE()
                  SELECT [QuestionnaireID],[Heading],[Vote],[StartTime],[EndTime]
                  FROM [Outline]
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
        /// 搜尋
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static DataTable findData(string Title, DateTime Start, DateTime End)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [QuestionnaireID],[Heading],[Vote],[StartTime],[EndTime]
                    FROM [Outline]
                    WHERE [Heading] LIKE (@Title + '%')
                    OR [StartTime] >= @Start
                    OR [EndTime] <= @End
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Title", Title));
            list.Add(new SqlParameter("@Start", Start));
            list.Add(new SqlParameter("@End", End));

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
        /// 新增問卷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ImgbtnAdd_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("/UserSide/USPage.aspx");
        }
        /// <summary>
        /// 刪除問卷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>    
        //刪除方法沒成功
        protected void ImgbtnBin_Click(object sender, ImageClickEventArgs e)
        {
            List<object> parameters = new List<object>();
            List<object> parameters_value = new List<object>();

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                CheckBox cb = (CheckBox)GridView1.Rows[i].FindControl("CheckBox1");

                if (cb.Checked == true)
                {
                    parameters.Add("@QuestionnaireID");
                    parameters_value.Add(GridView1.Rows[i].Cells[1].Text);
                }
            }

            if (parameters.Count != 0)  //寫刪除方法進DB
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    int CHQuestionnaireID = Convert.ToInt32(parameters_value[i]);
                    DelQuestionnaireID(CHQuestionnaireID);
                }
                Response.Write("<Script language= 'JaveScript'> alert('刪除完成!');<Script>");
            }
        }
        /// <summary>
        /// DB刪除問卷
        /// </summary>
        /// <param name="QuestionnaireID"></param>
        /// <returns></returns>
        public static DataTable DelQuestionnaireID(int QuestionnaireID)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"  DELETE FROM [Outline] WHERE [QuestionnaireID]= @QuestionnaireID
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


    }
}