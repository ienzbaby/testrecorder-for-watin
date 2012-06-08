using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Json : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        XD.QQ.JsonServices.ProcessRequest(HttpContext.Current);
        Response.End();
    }
}
