using DBSource;
using Sitecore.FakeDb;
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
        protected void Page_Init(object sender, EventArgs e)
        {
            string Account = HttpContext.Current.Session["UserLoginInfo"] as string;
            this.GridView1.DataSource = ContextManager.GetUSDBData(Account);
            this.GridView1.DataBind();
            var dt = ContextManager.GetUSDBData(Account);
            DataSearch(dt);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // check is logined
            if (!AuthManager.IsLogined())
            {
                Response.Redirect("/UserSide/USLogin.aspx");
                return;
            }
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
                HttpContext.Current.Response.Write("<script> alert('開始時間大於結束時間，請重新填寫') </script>");
            }

            this.GridView1.DataSource = ContextManager.FindData(findTitle, findStart, findEnd);
            this.GridView1.DataBind();
            var dtFD = ContextManager.FindData(findTitle, findStart, findEnd);
            DataSearch(dtFD);
        }

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

                this.GridView1.DataSource = dtPaged;
                this.GridView1.DataBind();
                this.ucPager.Visible = true;
            }
            else
            {
                this.GridView1.Visible = false;
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

        /// <summary>
        /// 修改狀態 =>已關閉/開放中
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
                    e.Row.Cells[3].Text = "已關閉";
                    e.Row.Cells[4].Text = "-";
                    e.Row.Cells[5].Text = "-";
                }
                else if (vt == "已完結")
                {
                    e.Row.Cells[3].Text = "已關閉";
                    e.Row.Cells[4].Text = "-";
                    e.Row.Cells[5].Text = "-";
                }
                else //投票中
                {
                    e.Row.Cells[3].Text = "開放中";
                }
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
        protected void ImgbtnBin_Click1(object sender, ImageClickEventArgs e)
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox cb = (row.Cells[0].FindControl("CheckBox1") as CheckBox);
                    if (cb.Checked == true)
                    {
                        int ID = Convert.ToInt32(row.Cells[1].Text);
                        DelQuestionnaireID(ID);
                    }
                }
            }
            GridView1.DataBind();
            Response.Redirect("/UserSide/USList.aspx");
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
                $@"DELETE FROM [Outline]
                      WHERE QuestionnaireID = @QuestionnaireID
                   DELETE FROM [Questionnaires]
                      WHERE QuestionnaireID = @QuestionnaireID
                   DELETE FROM [Question]
                      WHERE QuestionnaireID = @QuestionnaireID
                   DELETE FROM [Record]
                      WHERE QuestionnaireID = @QuestionnaireID
                   DELETE FROM [Record Details]
                      WHERE QuestionnaireID = @QuestionnaireID
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