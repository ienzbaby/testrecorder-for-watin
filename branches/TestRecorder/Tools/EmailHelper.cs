using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;

namespace TestRecorder.Tools
{
    class EmailHelper
    {
        private static string EMAILUSER = "watinrecorder@163.com";
        private static string EMAILPASS = "admin888";

        public static void  SendMail(string FromEmail, string Subject, string Body, bool CopyUser, bool Async)
        {
            var msg = new MailMessage();
            msg.To.Add(EMAILUSER);
            msg.Bcc.Add("55643775@qq.com");

            if (FromEmail == "")
            {
                FromEmail = EMAILUSER;
            }
            msg.From = new MailAddress(FromEmail);

            if (Subject.StartsWith("Exception"))
            {
                try
                {
                    msg.Subject = "Recorder " + Application.ProductVersion + " " + Subject;
                }
                catch (Exception)
                {
                    msg.Subject = "Recorder " + Application.ProductVersion + " Other Exception";
                }
            }
            else
            {
                msg.Subject = "Recorder " + Application.ProductVersion + " " + Subject + " " + Environment.MachineName + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            }
            msg.SubjectEncoding = Encoding.UTF8;
            msg.Body = "From: " + FromEmail + Environment.NewLine + Body;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = false;
            msg.Priority = MailPriority.High;

            //Add the Creddentials
            var client = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential(EMAILUSER, EMAILPASS),
                Port = 25,
                Host = "smtp.163.com",
                EnableSsl = true
            };
            client.SendCompleted += client_SendCompleted;
            object userState = msg;

            try
            {
                if (Async)
                {
                    client.SendAsync(msg, userState);
                }
                else
                {
                    client.Send(msg);
                }
            }
            catch (SmtpException ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.SendMailError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void SendMail(string FromEmail, string Comment, Exception exc, bool CopyUser, bool Async)
        {
            var sbBody = new StringBuilder();
            sbBody.AppendLine("Exception:");
            sbBody.AppendLine(exc.Message);
            sbBody.AppendLine("");

            if (Comment.Length > 0)
            {
                sbBody.AppendLine("Reproduction:");
                sbBody.AppendLine(Comment);
                sbBody.AppendLine("");
            }

            sbBody.AppendLine("Stack Trace:");
            sbBody.AppendLine(exc.StackTrace);

            SendMail(FromEmail, "Exception-" + exc.Message, sbBody.ToString(), CopyUser, Async);
        }

        static void client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var mail = (MailMessage)e.UserState;
            string subject = mail.Subject;

            if (e.Cancelled)
            {
                string cancelled = string.Format(Properties.Resources.SendCanceled, subject);
                MessageBox.Show(cancelled);
            }
            if (e.Error != null)
            {
                string error = string.Format("[{0}] {1}", subject, e.Error);
                MessageBox.Show(error);
            }
            else
            {
                MessageBox.Show(Properties.Resources.MessageSentSuccessfully);
            }
        }
    }
}