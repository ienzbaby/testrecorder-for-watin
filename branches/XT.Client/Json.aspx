<%@ Page Language="C#" AutoEventWireup="true" %>

<script type="text/C#" runat="server">
    protected override void  OnInit(EventArgs e)
    {
        base.OnInit(e);
        XD.QQ.JsonServices.ProcessRequest(HttpContext.Current);
        Context.Response.ContentType = "application/x-javascript; charset=utf-8";
        Context.Response.End();
    }
</script>

