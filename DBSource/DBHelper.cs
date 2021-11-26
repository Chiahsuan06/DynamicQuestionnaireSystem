using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSource
{
    public class DBHelper
    {
        public static string GetConnectionString()
        {
            string val = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return val;
        }

        public static DataTable ReadDataTable(string connStr, string dbCommand, List<SqlParameter> list)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand comm = new SqlCommand(dbCommand, conn))
                {
                    comm.Parameters.AddRange(list.ToArray());

                    conn.Open();
                    var reader = comm.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    return dt;
                }
            }
        }

        public static DataRow ReadDataRow(string connStr, string dbCommand, List<SqlParameter> list)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand comm = new SqlCommand(dbCommand, conn))
                {
                    comm.Parameters.AddRange(list.ToArray());

                    conn.Open();
                    var reader = comm.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    if (dt.Rows.Count == 0)
                        return null;

                    return dt.Rows[0];

                }
            }
        }

        public static SqlDataReader ExecuteReadesql(string sqlOrprocedure, SqlParameter[] param, bool isProcedure)
        {
            try
            {
                string connStr = GetConnectionString();
                SqlConnection con = new SqlConnection(connStr);

                Console.WriteLine("sucess");

                SqlCommand command = new SqlCommand(sqlOrprocedure, con);

                if (isProcedure)
                {
                    command.CommandType = CommandType.StoredProcedure;
                }

                con.Open();
                command.Parameters.AddRange(param);
                return command.ExecuteReader(CommandBehavior.CloseConnection);

            }
            catch (Exception ex)
            {
                //  writelog("執行 executeReadesql(string sqlOrprocedure, SqlParameter[] param, bool isProcedure)方法發生異常:" + ex.Message);
                throw ex;
            }

        }

    }
}

