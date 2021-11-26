﻿using DBSource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Label = System.Web.UI.WebControls.Label;

namespace Dynamic_questionnaire_system.ClientSide
{
    public partial class CSConfirmation : System.Web.UI.Page
    {
        //string connStr = DBHelper.GetConnectionString();
        //private int M_ID = 0;
        //private int D1_ID = 0;
        //private string D1_TITLE, D1_MustKeyIn, D1_SUMMARY;

        //protected void Page_Init(object sender, EventArgs e)
        //{
        //    //== 產生投票、問卷的畫面
        //    Generate_Page();
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["ID"] == null)
            {
                Response.Redirect("/ClientSide/CSList.aspx");
            }
            string IDNumber = this.Request.QueryString["ID"];

            this.reHeading.DataSource = GetHeading(IDNumber);
            this.reHeading.DataBind();

            this.ltlName.Text = Session["Name"] as string;
            this.ltlPhone.Text = Session["Phone"] as string;
            this.ltlEmail.Text = Session["Email"] as string;
            this.ltlAge.Text = Session["Age"] as string;

            // todo: 這裡還沒完成 =>構思是將問卷畫面坐起來，所以有些程式碼和上頁相同，照理已選的選項已經存在Session裡，又將其轉回ArrayList....如何讓其顯示???
            ArrayList M1_D1_D2 = Session["ListM1_D1_D2"] as ArrayList;
            foreach (var item in M1_D1_D2)
            {
                Response.Write(item);
            }



        }
        /// <summary>
        /// 問卷
        /// </summary>
        //public void Generate_Page()
        //{
        //    SqlConnection Conn = new SqlConnection(connStr);
        //    Conn.Open();   //-- 連結DB

        //    int M_ID = Convert.ToInt32(this.Request.QueryString["ID"]);
        //    SqlCommand cmd1 = new SqlCommand("SELECT * From [Questionnaires] WHERE [QuestionnaireID] = @QuestionnaireID", Conn);
        //    cmd1.Parameters.Add("@QuestionnaireID", SqlDbType.Int).Value = M_ID;
        //    SqlDataReader dr1 = cmd1.ExecuteReader();

        //    #region
        //    //**** "讀取" 這一份問卷的「每一個題目」。Questionnaires資料表 ****

        //    if (dr1.HasRows)
        //    {
        //        Label Label_table_start = new Label();
        //        Label_table_start.Text = "<table border=\"1\" width=\"480px\" id=\"table1\" style=\"border: 3px dotted #000080\">";
        //        PlaceHolder1.Controls.Add(Label_table_start);

        //        int table_i = 0;

        //        while (dr1.Read())
        //        {
        //            D1_ID = (int)dr1["TopicNum"];
        //            D1_TITLE = dr1["TopicDescription"].ToString();
        //            D1_MustKeyIn = dr1["TopicMustKeyIn"].ToString();

        //            if (DBNull.Value.Equals(dr1["TopicSummary"]))
        //            {
        //                D1_SUMMARY = dr1["TopicSummary"].ToString();
        //            }
        //            else
        //            {
        //                D1_SUMMARY = "";
        //            }

        //            string D1_TYPE = dr1["TopicType"].ToString();

        //            //**** "產生" 這一份問卷的「每一個題目」。Questionnaires資料表 ****

        //            Label Label_table_tr = new Label();
        //            if ((table_i % 2) == 0)
        //            {
        //                Label_table_tr.Text = "<tr><td>";
        //            }
        //            else
        //            {
        //                Label_table_tr.Text = "<tr><td bgcolor='#E3E6FD'>";
        //            }
        //            PlaceHolder1.Controls.Add(Label_table_tr);

        //            //---- (3-1). 共用的部分 -------------------------------------------
        //            //--  用來產生每一個題目的「主題」、「說明（摘要）」、是否必填？
        //            Generate_D1_Common(D1_ID, D1_TITLE, D1_MustKeyIn, D1_SUMMARY);

        //            //---- (3-2). 差異的部分 -------------------------------------------
        //            switch (D1_TYPE)   //-- 題目是單選、複選、文字輸入？
        //            {
        //                case "CB":  //-- 複選（CheckBoxList）
        //                            //---------------------------------------------------------------------------(start)
        //                    CheckBoxList CB_Q1 = new CheckBoxList();
        //                    CB_Q1.ID = "D1_" + D1_ID;

        //                    //-- 單/複選的子選項，記錄在 Question資料表裡面。
        //                    SqlCommand cmd2 = new SqlCommand("SELECT * FROM [Question] WHERE [TopicNum] = @TopicNum AND [QuestionnaireID] = @QuestionnaireID", Conn);
        //                    cmd2.Parameters.Add("@TopicNum", SqlDbType.Int).Value = D1_ID;
        //                    cmd2.Parameters.Add("@QuestionnaireID", SqlDbType.Int).Value = M_ID;

        //                    SqlDataReader dr2 = cmd2.ExecuteReader();  //-- 執行SQL指令。
        //                    dr2.Read();  //-- 只讀一列記錄。

        //                    for (int i = 1; i <= (int)dr2["OptionsAll"]; i++)   //-- 看看這個問題（單/複選）有幾個子選項？
        //                    {
        //                        string answer_item = "answer" + i;
        //                        CB_Q1.Items.Add(dr2[answer_item].ToString());
        //                    }
        //                    //---------------------------------------------------------------------------(end)

        //                    //string answerCB = dr2["answers"].ToString();
        //                    //string[] answersCB = answerCB.Split(';');
        //                    //foreach (var item in answersCB)
        //                    //{
        //                    //    CB_Q1.Items.Add(item);
        //                    //}

        //                    cmd2.Cancel();  //-- 用完就立即關閉資源。
        //                    dr2.Close();
        //                    PlaceHolder1.Controls.Add(CB_Q1);  //-- 動態加入畫面（PlaceHolder1）之中

        //                    Label Label_br = new Label();
        //                    Label_br.Text = "<br />";
        //                    PlaceHolder1.Controls.Add(Label_br);
        //                    break;

        //                case "RB":  //-- 單選（RadioButton）
        //                            //---------------------------------------------------------------------------(start)
        //                    RadioButtonList CB_Q2 = new RadioButtonList();
        //                    CB_Q2.ID = "D1_" + D1_ID;

        //                    //-- 單/複選的子選項，記錄在 Question_D2資料表裡面。
        //                    SqlCommand cmd3 = new SqlCommand("SELECT * FROM [Question] WHERE [TopicNum] = @TopicNum AND [QuestionnaireID] = @QuestionnaireID", Conn);
        //                    cmd3.Parameters.Add("@TopicNum", SqlDbType.Int).Value = D1_ID;
        //                    cmd3.Parameters.Add("@QuestionnaireID", SqlDbType.Int).Value = M_ID;

        //                    SqlDataReader dr3 = cmd3.ExecuteReader();  //-- 執行SQL指令。
        //                    dr3.Read();  //-- 只讀一列記錄。

        //                    for (int i = 1; i <= (int)dr3["OptionsAll"]; i++)   //-- 看看這個問題（單/複選）有幾個子選項？
        //                    {
        //                        string answer_item = "answer" + i;
        //                        CB_Q2.Items.Add(dr3[answer_item].ToString());
        //                    }
        //                    //---------------------------------------------------------------------------(end)


        //                    //string answerRB = dr3["answers"].ToString();
        //                    //string[] answersRB = answerRB.Split(';');
        //                    //foreach (var item in answersRB)
        //                    //{
        //                    //    CB_Q2.Items.Add(item);
        //                    //}
        //                    cmd3.Cancel();  //-- 用完就立即關閉資源。
        //                    dr3.Close();
        //                    PlaceHolder1.Controls.Add(CB_Q2);  //-- 動態加入畫面（PlaceHolder1）之中

        //                    Label Label_br1 = new Label();  //-- 只是為了畫面美觀而已。
        //                    Label_br1.Text = "<br />";
        //                    PlaceHolder1.Controls.Add(Label_br1);
        //                    break;

        //                default:  //-- 其他 文字輸入（TB，TextBox）
        //                    System.Web.UI.WebControls.TextBox CB_Q3 = new System.Web.UI.WebControls.TextBox();
        //                    CB_Q3.ID = "D1_" + D1_ID;
        //                    PlaceHolder1.Controls.Add(CB_Q3);  //-- 動態加入畫面（PlaceHolder1）之中

        //                    Label Label_br2 = new Label();
        //                    Label_br2.Text = "<br /><br />";
        //                    PlaceHolder1.Controls.Add(Label_br2);
        //                    break;
        //            }  // End of switch case

        //            Label Label_table_td = new Label();
        //            Label_table_td.Text = "</td></tr>";
        //            PlaceHolder1.Controls.Add(Label_table_td);

        //            table_i += 1;
        //        }

        //        Label Label_table_end = new Label();
        //        Label_table_end.Text = "</table>";
        //        PlaceHolder1.Controls.Add(Label_table_end);
        //    }
        //    cmd1.Cancel();  //-- 用完就立即關閉資源。
        //    dr1.Close();

        //    Conn.Close();
        //    Conn.Dispose();
        //    #endregion

        //    if (Conn.State == ConnectionState.Open)
        //    {
        //        Conn.Close();
        //        Conn.Dispose();
        //    }
        //}
        ////== 產生每一個問題的共同項目，例如：「標題」、「摘要」、「是否必填？」
        ////== Questionnaires資料表。
        //protected void Generate_D1_Common(int D1_ID, string D1_TITLE, string D1_MustKeyIn, string D1_SUMMARY)
        //{
        //    Label LB_title = new System.Web.UI.WebControls.Label();
        //    LB_title.ID = "Label_D1title_" + D1_ID;
        //    LB_title.Text = "<b>" + D1_TITLE + "</b>";   //-- 每一個問題的「標題」
        //    if (D1_MustKeyIn == "Y")
        //    {
        //        LB_title.Text += "&nbsp;&nbsp;<font color=red><b>*（必填）</b></font>";
        //        //-- 每一個問題的「標題」。強調「必填」！
        //    }
        //    PlaceHolder1.Controls.Add(LB_title);   //-- 動態加入畫面（PlaceHolder1）之中

        //    System.Web.UI.WebControls.Label LB_summary = new System.Web.UI.WebControls.Label();
        //    LB_summary.ID = "Label_D1summary_" + D1_ID;
        //    LB_summary.Text = "<br /><font color='#484848'><small>" + D1_SUMMARY + "</small></font><br />";  //-- 每一個問題的「摘要」
        //    PlaceHolder1.Controls.Add(LB_summary);  //-- 動態加入畫面（PlaceHolder1）之中
        //}

        ////== 計算這個問卷出了幾個題目（Questionnaires）？
        //protected int Compute_QNo(int M_ID)
        //{
        //    SqlConnection Conn = new SqlConnection(connStr);
        //    Conn.Open();   //-- 連結DB
        //    SqlCommand cmdQNo = new SqlCommand("SELECT COUNT([TopicNum]) FROM [Questionnaires] WHERE [QuestionnaireID] IN (SELECT [QuestionnaireID] = @QuestionnaireID From [Outline] WHERE [Vote] = '投票中' AND [StartTime] <= GETDATE() AND [EndTime] >= GETDATE())", Conn);
        //    cmdQNo.Parameters.Add("@QuestionnaireID", SqlDbType.Int).Value = M_ID;
        //    //-- 把 Outline資料表裡面，呈現在網站的首頁上面。

        //    int x = (int)cmdQNo.ExecuteScalar();  //-- 執行SQL指令。
        //    cmdQNo.Cancel();
        //    Conn.Close();
        //    Conn.Dispose();

        //    return x; //-- 執行SQL指令。
        //}
        //protected System.Collections.ArrayList Take_D1_ID(int M_ID)
        //{
        //    SqlConnection Conn = new SqlConnection(connStr);
        //    Conn.Open();   //-- 連結DB
        //    SqlCommand cmdD1 = new SqlCommand("SELECT [TopicNum] FROM [Questionnaires] WHERE [QuestionnaireID] = @QuestionnaireID", Conn);
        //    cmdD1.Parameters.Add("@QuestionnaireID", SqlDbType.Int).Value = M_ID;
        //    //-- 把 Outline資料表裡面，最新的一場投票，呈現在網站的首頁上面。

        //    SqlDataReader drD1 = cmdD1.ExecuteReader();  //-- 執行SQL指令。

        //    System.Collections.ArrayList D1_array = new System.Collections.ArrayList();
        //    if (drD1.HasRows)
        //    {
        //        while (drD1.Read())
        //            D1_array.Add(drD1["TopicNum"]);
        //    }
        //    cmdD1.Cancel();
        //    Conn.Close();
        //    Conn.Dispose();

        //    return D1_array;
        //}
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
                    WHERE QuestionnaireID = @QuestionnaireID
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
        /// 會回到填寫頁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>  
        //OK
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Session["CSCFIName"] = this.ltlName.Text;
            this.Session["CSCFIPhone"] = this.ltlPhone.Text;
            this.Session["CSCFIEmail"] = this.ltlEmail.Text;
            this.Session["CSCFIAge"] = this.ltlAge.Text;
            Response.Redirect("/ClientSide/CSPage.aspx?ID=" + Request.QueryString["ID"]);
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
                Session.Clear();
            }
            HttpContext.Current.Response.Write("<script> alert('資料送出成功，即將返回列表頁') </script>");
            //MessageBox.Show($"資料送出成功，即將返回列表頁", "確定", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                $@" INSERT INTO [Record] ([AnswererID], [AnsName], [AnsPhone], [AnsEmail], [AnsAge])
                    SELECT newid() AS [AnswererID], @AnsName, @AnsPhone, @AnsEmail, @AnsAge
                ";

            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@AnsName", Name));
            list.Add(new SqlParameter("@AnsPhone", Phone));
            list.Add(new SqlParameter("@AnsEmail", Email));
            list.Add(new SqlParameter("@AnsAge", Age));

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