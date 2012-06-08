using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using SHDocVw;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using System.Drawing;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
    public class ActionNavigate : ActionBase
    {
        public override string Name
        {
            get { return "Navigate"; }
        }

        /// <summary>
        /// Url to navigate to
        /// </summary>
        public string URL
        {
            get
            {
                if (!Regex.IsMatch(_url, @"\b(https?|ftp|file)://[-A-Z0-9+&@#/%?=~_|!:,.;]*[-A-Z0-9+&@#/%=~_|]", RegexOptions.IgnoreCase)
                    && !Regex.IsMatch(_url, @".:\\.+"))
                {
                    _url = _url.Insert(0, "http://");
                }
                return _url;
            }
            set
            {
                _url = value;
            }
        }
        private string _url = "";

        /// <summary>
        /// Username for entry into the web page
        /// </summary>
        public string Username = "";

        /// <summary>
        /// Password for entry into the web page
        /// </summary>
        public string Password = "";

        public override IUcBaseEditor GetEditor()
        {
            return new ucNavigate();
        }

        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("Navigate.bmp");
        }
        public ActionNavigate(ActionContext context):base(context)
        {
        }
        /// <summary>
        /// navigates to the indicated Url
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Perform()
        {
            Context.ActivePage.Browser.WaitForComplete();
            GoToURL();
            Status = StatusIndicators.Validated;
            return true;
        }

        private void GoToURL()
        {
            /* 替换成超时模式
            Context.ActivePage.Browser.GoToNoWait(URL);
            */

            Context.ActivePage.Browser.GoTo(URL);
            Context.ActivePage.Browser.WaitForComplete();

            InternetExplorer ie = Context.ActivePage.Browser.InternetExplorer as InternetExplorer;
            while (ie != null && (ie.Busy || ie.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE))
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }

        }

        /// <summary>
        /// test whether we can get to the page
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Validate()
        {
            bool result = false;

            // test whether we can get to the page
            var web = new WebClient();

            try
            {
                web.Credentials = new NetworkCredential(Username, Password, "");
#pragma warning disable 618,612
                ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();
#pragma warning restore 618,612
                web.DownloadData(URL);

                Status = StatusIndicators.Validated;
                result = true;
            }
            catch (Exception ex)
            {
                Status = StatusIndicators.Faulted;
                ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// retrieves the description text about this action
        /// </summary>
        /// <returns>description of the action</returns>
        public override string Description
        {
            get
            {
                return "Navigate to " + URL;
            }
        }

        /// <summary>
        /// returns object as code, formatted for a specific language
        /// </summary>
        /// <param name="Formatter">language-specific formatting</param>
        /// <returns>code from the object</returns>
        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine();
            line.NoModel = true;
            var sb = new StringBuilder();
            string browserFriendlyName = Context.ActivePage.Browser.FriendlyName;

            if (Username != "")
            {
               
                sb.AppendLine(Formatter.ClassNameFormat(typeof(WatiN.Core.DialogHandlers.LogonDialogHandler), "logon" + Context.ActivePage.FriendlyName) + "  = " + Formatter.NewDeclaration + " LogonDialogHandler(\"" + Username + "\",\"" + Password + "\")" + Formatter.LineEnding);
                sb.AppendLine(Formatter.VariableDeclarator + browserFriendlyName + Formatter.MethodSeparator + "AddDialogHandler(dhdlLogon)" + Formatter.LineEnding);
            }

            string localurl = URL;
            if (localurl.Contains("\\")) sb.Append(Formatter.VariableDeclarator + browserFriendlyName + Formatter.MethodSeparator + "GoTo(@\"" + localurl + "\")" + Formatter.LineEnding);
            else sb.Append(Formatter.VariableDeclarator + browserFriendlyName + Formatter.MethodSeparator + "GoTo(\"" + localurl + "\")" + Formatter.LineEnding);
            line.FullLine = sb.ToString();
            return line;
        }

        public override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml( node);
            Username = node.Attributes["Username"].Value;
            Password = node.Attributes["Password"].Value;
            URL = node.InnerText;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "Navigate");
            writer.WriteAttributeString("Username", Username);
            writer.WriteAttributeString("Password", Password);
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteValue(URL);
            writer.WriteEndElement();
        }
    }
    /// <summary>
    /// Shell policy class to accept all certificates
    /// since Centris is too cheap to have Verisign Certs
    /// </summary>
    sealed class AcceptAllCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            // Just accept. 
            return true;
        }
    }
}