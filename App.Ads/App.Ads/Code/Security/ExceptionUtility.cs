using System;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace App.Ads.Code.Security
{
    public sealed class ExceptionUtility
    {
        /// <summary>
        /// Assign Error ID in Global.asa Application_Error
        /// </summary>
        private static string _ErrorID = string.Empty;
        public static string ErrorID
        {
            get { return _ErrorID; }
            set { _ErrorID = value; }
        }

        //// Log an Exception
        public static void LogException(Exception exc, HttpRequest request, string source, string ErrorID)
        {
            // Include enterprise logic for logging exceptions
            // Get the absolute path to the log file

            
            string logFile = "/App_Data/ErrorLog/" + DateTime.Now.ToString("yyyy-MMM") + "/" + DateTime.Now.ToString("dd") + ".txt";
            logFile = HttpContext.Current.Server.MapPath(logFile);

            if (!Directory.Exists(Path.GetDirectoryName(logFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFile));
            }

            // Open the log file for append and write the log
            StreamWriter sw = new StreamWriter(logFile, true);
            sw.WriteLine("********** {0} **********", DateTime.Now);

            sw.WriteLine("Error ID: " + ErrorID);

            if (exc.InnerException != null)
            {
                sw.Write("Inner Exception Type: ");
                sw.WriteLine(exc.InnerException.GetType().ToString());
                sw.Write("Inner Exception: ");
                sw.WriteLine(exc.InnerException.Message);
                sw.Write("Inner Source: ");
                sw.WriteLine(exc.InnerException.Source);
                if (exc.InnerException.StackTrace != null)
                {
                    sw.WriteLine("Inner Stack Trace: ");
                    sw.WriteLine(exc.InnerException.StackTrace);
                }
            }
            sw.Write("Exception Type: ");
            sw.WriteLine(exc.GetType().ToString());
            sw.WriteLine("Exception: " + exc.Message);
            sw.WriteLine("URL: " + request.Url);
            sw.WriteLine("Source: " + source);
            sw.WriteLine("Stack Trace: ");
            if (exc.StackTrace != null)
            {
                sw.WriteLine(exc.StackTrace);
                sw.WriteLine();
            }
            sw.Close();
        }


        // Notify System Operators about an exception
        public static void NotifySystemOps(Exception exc, HttpRequest request, string ErrorID)
        {

            StringBuilder errorMessage = new StringBuilder();

            if (exc.Message != null)
            {
                //Skip Sending Email if isLocal
                if (!request.IsLocal)
                {
                    string message = string.Empty;
                    if (exc.InnerException != null)
                    {
                        message = exc.InnerException.Message;
                    }
                    else
                    {
                        message = exc.Message;
                    }

                    errorMessage.AppendLine("ERROR ID: " + ErrorID);
                    errorMessage.AppendLine("ERROR: " + message);
                    errorMessage.AppendLine("TIME:" + DateTime.Now.ToLongTimeString());
                    errorMessage.AppendLine("URL: " + request.Url);
                    errorMessage.AppendLine("Referral Url: " + request.UrlReferrer);
                    errorMessage.AppendLine("HTTP_X_FORWARDED_FOR: " + request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                    errorMessage.AppendLine("REMOTE_ADDR: " + request.ServerVariables["REMOTE_ADDR"]);
                    //errorMessage.AppendLine("User Id: " + SessionHelper.UserId);
                    //errorMessage.AppendLine("Anonymous User Id: " + SessionHelper.AnonymousUserId);
                    errorMessage.AppendLine("UserAgent: " + request.UserAgent);
                    errorMessage.Append(Environment.NewLine);

                    errorMessage.AppendLine("Form Data: " + request.Form.ToString());
                    errorMessage.Append(Environment.NewLine);

                    if (exc.InnerException != null)
                    {
                        errorMessage.AppendLine("Inner Exception: " + exc.InnerException.ToString());
                        errorMessage.Append(Environment.NewLine);
                    }

                    errorMessage.AppendLine("Exception: " + exc.ToString());

                    Regex myRegex = new Regex(@"[^A-Za-z0-9 .]", RegexOptions.Multiline);
                    message = myRegex.Replace(message, "");
                    if (exc.InnerException != null)
                    {
                        message = myRegex.Replace(exc.InnerException.Message, "");
                    }
                    else
                    {
                        message = myRegex.Replace(exc.Message, "");
                    }
                    if (message.Length > 80)
                    {
                        message = message.Substring(0, 80);
                    }

                    string emailSubject = "[" + request.Url.Host + "] " + message;

                    //Email.SendSystemEmail(emailSubject, errorMessage.ToString(), true);
                }
            }

        }


    }
}