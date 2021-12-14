using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSource
{
    public class ContextManager
    {
        /// <summary>
        /// 前後台搜尋區  包含指定文字、包含在開始、結束指定時間區間
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static DataTable FindData(string Title, DateTime Start, DateTime End)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [Outline].[QuestionnaireID],[Heading],[Vote],[StartTime],[EndTime]
                    FROM [Outline]
                    WHERE [Heading] LIKE (@Title + '%')
                    OR [StartTime] >= @Start
                    AND [EndTime] <= @End
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
        /// 前後台搜尋區 顯示資料、判斷投票狀態(已完結、尚未開始、投票中)
        /// </summary>
        /// <returns></returns>
        //前台        
        public static DataTable GetCSDBData()
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
        //後台 區分使用者
        public static DataTable GetUSDBData(string Account)
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
                    WHERE[Account] = @Account
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Account", Account));

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
        //前台內頁(標題、內容)、前台統計
        public static DataTable GetHeadingContent(int IDNumber)
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
        //前台統計
        public static DataTable GetTopicDescription(int IDNumber)
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

        public static DataTable GetQuestion(int IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Question].[QuestionnaireID],[Question].[TopicNum],[Questionnaires].[TopicDescription],[answer1],[answer2],[answer3]
		                    ,[answer4],[answer5] ,[answer6],[OptionsAll]
                    FROM [Questionnaire].[dbo].[Question]
                    JOIN [Questionnaire].[dbo].[Questionnaires]
                    ON  [Question].[TopicNum] = [Questionnaires].[TopicNum]
                    WHERE [Question].[QuestionnaireID] = @QuestionnaireID
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
        /// 取得 統計所需資料
        /// </summary>
        /// <param name="IDNumber"></param>
        /// <returns></returns>
        public static DataTable GetStatisticsDBSourceTB(int IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Record Details].[TopicNum], [Questionnaires].[TopicDescription], [Questionnaires].[TopicType], 
                          [Question].[answer1], [Question].[answer2],[Question].[answer3], [Question].[answer4], [Question].[answer5],
		                  [Question].[answer6], [Question].[OptionsAll],[RDAns]
                    FROM [Record Details]
                    JOIN [Questionnaires] 
                    ON [Questionnaires].TopicNum = [Record Details].TopicNum
                    JOIN [Question] 
                    ON [Question].TopicNum = [Record Details].TopicNum
                    WHERE [Record Details].[QuestionnaireID] = @QuestionnaireID
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

        public static DataTable USGetStatisticsDBSourceTB(int IDNumber, int RecordNum)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Record Details].[TopicNum], [Questionnaires].[TopicDescription], [Questionnaires].[TopicType], 
                          [Question].[answer1], [Question].[answer2],[Question].[answer3], [Question].[answer4], [Question].[answer5],
		                  [Question].[answer6], [Question].[OptionsAll],[RDAns]
                    FROM [Record Details]
                    JOIN [Questionnaires] 
                    ON [Questionnaires].TopicNum = [Record Details].TopicNum
                    JOIN [Question] 
                    ON [Question].TopicNum = [Record Details].TopicNum
                    WHERE [Record Details].[QuestionnaireID] = @QuestionnaireID
                     AND  [Record Details].[RecordNum] = @RecordNum
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", IDNumber));
            list.Add(new SqlParameter("@RecordNum", RecordNum));

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

        #region 後台

        //問卷
        /// <summary>
        /// 取得已有問卷資料
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public static DataRow GetDBData(int IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Heading],[Vote],[StartTime],[EndTime],[Content]
                     FROM [Outline]
                     WHERE[QuestionnaireID] = @QuestionnaireID
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", IDNumber));

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
        /// <summary>
        /// 更新既有問卷
        /// </summary>
        /// <param name="Heading"></param>
        /// <param name="Content"></param>
        /// <param name="Vote"></param>
        /// <param name="StartT"></param>
        /// <param name="EndT"></param>
        /// <param name="QuestionnaireID"></param>
        /// <returns></returns>
        public static DataTable UpData(string Heading, string Content, string Vote, DateTime StartT, DateTime EndT, int QuestionnaireID)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"UPDATE [Outline]
                    SET [Heading] = @Heading, [Vote] = @Vote, [StartTime] = @StartTime
                    	,[EndTime] = @EndTime, [Content] = @Content
                    WHERE [QuestionnaireID] = @QuestionnaireID;
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@QuestionnaireID", QuestionnaireID));
            list.Add(new SqlParameter("@Heading", Heading));
            list.Add(new SqlParameter("@Content", Content));
            list.Add(new SqlParameter("@StartTime", StartT));
            list.Add(new SqlParameter("@EndTime", EndT));
            list.Add(new SqlParameter("@Vote", Vote));

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
        /// 問卷 - 寫進資料庫
        /// </summary>
        /// <param name="IDNumber"></param>
        /// <returns></returns>
        public static DataTable Add(string Heading, string Content, DateTime StartT, DateTime EndT, Guid QuestionnaireNum, string Vote, string Account)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" INSERT INTO Outline (Heading, Content, StartTime, EndTime, QuestionnaireNum, Vote, Account)
                    VALUES (@Heading, @Content, @StartTime, @EndTime, NEWID(), @Vote, @Account )
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Heading", Heading));
            list.Add(new SqlParameter("@Content", Content));
            list.Add(new SqlParameter("@StartTime", StartT));
            list.Add(new SqlParameter("@EndTime", EndT));
            list.Add(new SqlParameter("@QuestionnaireNum", QuestionnaireNum));
            list.Add(new SqlParameter("@Vote", Vote));
            list.Add(new SqlParameter("@Account", Account));

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
        /// 問卷 - 取得新增問卷的QuestionnaireID
        /// </summary>
        /// <param name="Heading"></param>
        /// <param name="Content"></param>
        /// <param name="StartT"></param>
        /// <param name="EndT"></param>
        /// <param name="QuestionnaireNum"></param>
        /// <param name="Vote"></param>
        /// <returns></returns>
        public static DataRow GetQuestionnaireID(string Heading, string Content, DateTime StartT, DateTime EndT, string Account)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [QuestionnaireID]
                    FROM [Outline]
                    WHERE [Heading] = @Heading　
                      AND [Content] = @Content AND [StartTime] = @StartTime 
                      AND [EndTime] = @EndTime AND [Account] = @Account
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Heading", Heading));
            list.Add(new SqlParameter("@Content", Content));
            list.Add(new SqlParameter("@StartTime", StartT));
            list.Add(new SqlParameter("@EndTime", EndT));
            list.Add(new SqlParameter("@Account", Account));

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

        //填寫資料
        /// <summary>
        /// 顯示givExport資料
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        public static DataTable GetRecordData(int IDNumber)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [RecordNum]
                          ,[QuestionnaireID]
                          ,[AnsName]
                          ,[AnsTime]
                      FROM [Questionnaire].[dbo].[Record]
                      WHERE [QuestionnaireID] = @QuestionnaireID
                      ORDER BY [AnsTime] DESC
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
        /// 取得使用者資訊、每個問題、每個問題的答案
        /// </summary>
        /// <param name="RecordNum"></param>
        /// <returns></returns>
        public static DataTable GetAnsRecordDetailsData()
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [Record].[RecordNum]
	　                    ,[AnswererID]
                          ,[AnsName]
                          ,[AnsPhone]
                          ,[AnsEmail]
                          ,[AnsAge]
                          ,[AnsTime]
                          ,[Record].[QuestionnaireID]
	                      ,[Outline].[Heading]
                          ,[Record Details].[TopicNum]
	                      ,[TopicDescription]
                          ,[Record Details].[ReplicationNum]
	                      ,[RDAns]
                      FROM [Questionnaire].[dbo].[Record]
                      JOIN [Questionnaire].[dbo].[Outline]
                      ON [Record].[QuestionnaireID] = [Outline].[QuestionnaireID]
                      JOIN [Questionnaire].[dbo].[Record Details]
                      ON [Record].RecordNum = [Record Details].RecordNum
                      JOIN[Questionnaire].[dbo].[Questionnaires]
                      ON [Record Details].[TopicNum] = [Questionnaires].[TopicNum]  
					  ORDER BY [Record].[RecordNum],[Record Details].[TopicNum]
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
        /// 取得問卷填寫的細節
        /// </summary>
        /// <returns></returns>
        public static DataRow GetRecordDetailsData(int RecordNum)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@"SELECT [Record].[RecordNum]
                          ,[Record].[QuestionnaireID]
                          ,[AnswererID]
                          ,[AnsName]
                          ,[AnsPhone]
                          ,[AnsEmail]
                          ,[AnsAge]
                          ,[AnsTime]
	                      ,[ReplicationNum]
                          ,[Record Details].[TopicNum]
	                      ,[TopicDescription]
						  ,[RDAns]
                      FROM [Questionnaire].[dbo].[Record]
                      JOIN [Questionnaire].[dbo].[Record Details]
                      ON [Record].RecordNum = [Record Details].RecordNum
                      JOIN[Questionnaire].[dbo].[Questionnaires]
                      ON [Record Details].[TopicNum] = [Questionnaires].[TopicNum]  
                      JOIN[Questionnaire].[dbo].[Question]
                      ON [Question].[TopicNum] = [Record Details].[TopicNum]
                      WHERE [Record].[RecordNum] = @RecordNum 
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@RecordNum", RecordNum)); ;

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

    }
}
