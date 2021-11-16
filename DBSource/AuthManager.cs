using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DBSource
{
    public class AuthManager
    {
        public static DataRow GetUserInfoByAccount(string account)
        {
            string connStr = DBHelper.GetConnectionString();
            string dbcommand =
                $@" SELECT [UserID],[Account],[Password],[Name],[Phone],[Email],[QID1],[QID2],[QID3],[QID4],[QID5],[QID_All]
                    FROM [Questionnaire].[dbo].[Information]
                    WHERE [Account] = @Account
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@Account", account));

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



        /// <summary> 檢查目前是否登入 </summary>
        /// <returns></returns>
        //加入後一直不成功
        public static bool IsLogined()
        {
            if (HttpContext.Current.Session["UserLoginInfo"] == null)
                return false;
            else
                return true;
        }
    }
}
