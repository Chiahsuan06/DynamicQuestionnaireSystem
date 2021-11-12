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
        /// <summary>
        /// 隱藏顯示超連結
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string vt = (string)DataBinder.Eval(e.Row.DataItem, "Vote");
                if (vt == "尚未開始")
                {
                    e.Row.Cells[1].Visible = false;
                    e.Row.Cells[2].Visible = true;   //讓文字出現
                }
                else if (vt == "已完結")
                {
                    e.Row.Cells[1].Visible = false;
                    e.Row.Cells[2].Visible = true;  //讓文字出現
                }
                else //投票中
                {
                    e.Row.Cells[1].Visible = true;
                    e.Row.Cells[2].Visible = false;
                }
                GridView1.HeaderRow.Cells[2].Visible = false;
            }
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
                HttpContext.Current.Response.Write("<script> alert('開始時間大於結束時間，請重新填寫') </script>");
            }

            this.GridView1.DataSource = findData(findTitle, findStart, findEnd);
            this.GridView1.DataBind();
            var dt = findData(findTitle, findStart, findEnd);
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
                   
                   UPDATE [Outline]
                      SET [Vote] = '投票中'
                      WHERE [StartTime] < GETDATE() AND [EndTime] > GETDATE()
                   
                    SELECT [Outline].[QuestionnaireID],[Heading],[Vote],[StartTime],[EndTime]
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
        public static DataTable findData(string Title, DateTime Start, DateTime End)   //OK
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [Outline].[QuestionnaireID],[Heading],[Vote],[StartTime],[EndTime]
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


    }
}