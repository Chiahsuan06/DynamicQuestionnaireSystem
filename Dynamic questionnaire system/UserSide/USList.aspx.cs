using DBSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
        /// 顯示資料、判斷投票狀態(成功)
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDBData()
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"
                  UPDATE [Outline]
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
                     SELECT [QuestionnaireID]
                          ,[QuestionnaireNum]
                          ,[Heading]
                          ,[Vote]
                          ,[StartTime]
                          ,[EndTime]
                          ,[Content]
                      FROM [Questionnaire].[dbo].[Outline]
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