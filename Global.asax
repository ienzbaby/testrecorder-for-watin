<%@ Application Language="C#" %>
<script runat="server">
    private readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HttpApplication));
    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(Server.MapPath(@"~/App_Data/log4net.config")));
        XD.Tools.Tasks.TaskManager.Initialize(Server.MapPath(@"~/App_Data/Task.config")).Start();
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs
        Exception err = Server.GetLastError().GetBaseException();
        string msg = "[IP:" + Request.UserHostAddress + "][Url:" + Request.Url + "][referrer:" + Request.UrlReferrer + "]";
        log.Error("application error", err);
    }   
</script>
