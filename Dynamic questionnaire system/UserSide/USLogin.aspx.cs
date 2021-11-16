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
    public partial class USLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.ClearHeaders();
            Response.Redirect("/Default.aspx");
        }
        
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string inp_Account = this.txtAccount.Text;
            string inp_PWD = this.txtPassword.Text;

            if (string.IsNullOrWhiteSpace(inp_Account) || string.IsNullOrWhiteSpace(inp_PWD))
            {
                this.ltlMsg.Text = "Account / PWD is required.";
                return;
            }

            var dr = AuthManager.GetUserInfoByAccount(inp_Account);

            //check null
            if (dr == null)
            {
                this.ltlMsg.Text = "Account doesn't exists.";
                return;
            }

            //check account / PWD
            if (string.Compare(dr["Account"].ToString(), inp_Account, true) == 0 && string.Compare(dr["Password"].ToString(), inp_PWD, false) == 0)
            {
                this.Session["UserLoginInfo"] = dr["Account"].ToString();
                Response.Redirect("/UserSide/USList.aspx");
            }
            else
            {
                this.ltlMsg.Text = "Login fail. Place check Account / PWD.";
                return;
            }

        }
    }
}