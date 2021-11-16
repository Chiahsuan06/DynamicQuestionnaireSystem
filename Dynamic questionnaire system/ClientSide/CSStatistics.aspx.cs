using DBSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dynamic_questionnaire_system.ClientSide
{
    public partial class CSStatistics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["StatisticsID"] == null)
            {
                Response.ClearHeaders();
                Response.Redirect("/ClientSide/CSList.aspx");
            }

            string IDNum = this.Request.QueryString["StatisticsID"];
            this.reHD.DataSource = GetHeadingContent(IDNum);
            this.reHD.DataBind();

            this.reTopicDescription.DataSource = GetTopicDescription(IDNum);
            this.reTopicDescription.DataBind();

        }
        public static DataTable GetHeadingContent(string IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Heading],[Vote],[StartTime],[EndTime],[Content]
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

        public static DataTable GetTopicDescription(string IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [TopicNum],[TopicDescription],[TopicSummary],[TopicType],[TopicMustKeyIn]
                    FROM [Questionnaire].[dbo].[Questionnaires]
                    WHERE [QuestionnaireID] = @QuestionnaireID
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
    }
}