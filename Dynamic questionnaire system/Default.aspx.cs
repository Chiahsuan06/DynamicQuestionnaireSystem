using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Dynamic_questionnaire_system
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imgbtnLogin_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("/UserSide/USLogin.aspx");
        }

        protected void imgbtnQuestion_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("/ClientSide/CSList.aspx");
        }
    }
}