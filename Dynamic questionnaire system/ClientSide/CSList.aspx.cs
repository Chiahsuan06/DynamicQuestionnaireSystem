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
    public partial class CSList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.GridView1.DataSource = GetDBData();  ///讓GridView1顯示DB的資料
            this.GridView1.DataBind();
        }

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
        public static DataTable findData(string Title, DateTime Start, DateTime End)   //SQL找得到，為什麼搜尋不到??
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

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)  //如何寫入Session or 其他方式  還有問卷的狀態  投票中有超連結 其餘沒有
        {
            var item = e.CommandSource as System.Web.UI.WebControls.Button;
            var container = item.NamingContainer;

            if (string.Compare("goPage", item.ID, true) == 0)
            {
                this.Session["QuestionnaireNum"] = container;
            }
        }
    }
}