using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using TestRecorder.Tools;

namespace TestRecorder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!ComUtilitiesIsRegistered())
            {
                //MessageBox.Show("One of the required components was not registered."+Environment.NewLine+"Attempting to register now.", "Unregistered Component", MessageBoxButtons.OK, MessageBoxIcon.Error);
                string registerMessage = RegisterComUtilities();
                if (!ComUtilitiesIsRegistered())
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine(
                        "Unable to register ComUtilities.dll-- you may need to run as an administrator.");
                    if (registerMessage != "") messageBuilder.AppendLine("Registration Message: " + registerMessage);
                    MessageBox.Show(messageBuilder.ToString(), "Unregistered Component", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }
            }

            if (!IE7OrHigher())
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine(
                    "This program requires Microsoft Internet Explorer version 7 or higher to operate.");
                messageBuilder.AppendLine("Please install IE7 from Microsoft's website before running.");
                MessageBox.Show(messageBuilder.ToString(), "Update Browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*Hao µ½ºöÂÔCross-thread InvalidOperationException½¨Òé*/
            Control.CheckForIllegalCrossThreadCalls = false;
 
            var mainform = new frmMain();
            // popup blocker
            try
            {
                RegistryKey popupBlockerKey =
                                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\New Windows");
                if (popupBlockerKey != null
                    && popupBlockerKey.GetValue("PopupMgr").ToString() == "yes")
                {
                    MessageBox.Show(
                        "IE Popup blocker is on." + Environment.NewLine +
                        "This may cause some sites not to function properly." + Environment.NewLine +
                        "For best results, please disable it.", "Popup Blocker", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
            }

            try
            {
                Application.Run(mainform);
            }
            catch (Exception ex)
            {
                EmailException(ex);
            }

        }

        private static void EmailException(Exception e)
        {
            var frm = new frmException
            {
                lblError = { Text = e.Message },
                rtbStack = { Text = e.StackTrace }
            };
            if (frm.ShowDialog() == DialogResult.OK)
            {
                EmailHelper.SendMail("", frm.txtComments.Text, e, frm.chkCopy.Checked, false);
            }
        }

        private static string RegisterComUtilities()
        {
            string exceptionMessage = "";
            try
            {
                string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                path = System.IO.Path.Combine(path, "ComUtilities.dll");
                Process.Start(@"c:\Windows\System32\regsvr32.exe", path+" /s");
                Thread.Sleep(new TimeSpan(0, 0, 1));
                Application.DoEvents();
                Thread.Sleep(new TimeSpan(0, 0, 1));
                Application.DoEvents();
                Thread.Sleep(new TimeSpan(0, 0, 1));
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }
            return exceptionMessage;
        }

        private static bool ComUtilitiesIsRegistered()
        {
            try
            {
                RegistryKey comUtilityKey = Registry.ClassesRoot.OpenSubKey(@"ComUtilities.UtilMan\CLSID");
                if (comUtilityKey == null) throw new ArgumentException("ComUtility Key Not Found");
                if (comUtilityKey.ValueCount == 0) throw new ArgumentException("ComUtility GUID not found");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IE7OrHigher()
        {
            try
            {
                bool result = false;
                RegistryKey ieKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer");
                if (ieKey != null)
                {
                    string version = ieKey.GetValue("Version").ToString();
                    if (!version.StartsWith("5") && !version.StartsWith("6"))
                    {
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}