using App.Core.Models;
using App.Core.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;

namespace App.Core.Services
{
    public interface IEmailService
    {
        void SendEmail(string emailAddress, string title, string message, object data);

        void SendEmail(SendEmailModel sendEmailModel, string templateName, object data);

        void TestEmail();
    }

    public class EmailService : IEmailService
    {
        private SmtpClient smtpClient;
        private IConfigService configService;
        private EmailHostModel emailHostModel;

        public EmailService(SmtpClient smtpClient, IConfigService configService)
        {
            this.smtpClient = smtpClient;
            this.configService = configService;

            emailHostModel = new EmailHostModel
            {
                EmailHost = this.configService.GetValue(ConfigName.EmailHost),
                EmailUserName = this.configService.GetValue(ConfigName.EmailUsername),
                EmailPassword = this.configService.GetValue(ConfigName.EmailPassword),
                EmailEnableSSL = this.configService.GetValue(ConfigName.EmailEnableSSL),
                EmailFrom = this.configService.GetValue(ConfigName.EmailFrom),
                EmailPort = this.configService.GetValue(ConfigName.EmailPort)
            };
        }

        void IEmailService.TestEmail()
        {
            string FROM = emailHostModel.EmailFrom;   // Replace with your "From" address. This address must be verified.
            const String TO = "walkboy0602@gmail.com";  // Replace with a "To" address. If you have not yet requested
            // production access, this address must be verified.

            const String SUBJECT = "Amazon SES test (SMTP interface accessed using C#)";
            const String BODY = "This email was sent through the Amazon SES SMTP interface by using C#.";

            // Supply your SMTP credentials below. Note that your SMTP credentials are different from your AWS credentials.
            string SMTP_USERNAME = emailHostModel.EmailUserName;  // Replace with your SMTP username. 
            string SMTP_PASSWORD = emailHostModel.EmailPassword; // Replace with your SMTP password.

            // Amazon SES SMTP host name. This example uses the us-east-1 region.
            string HOST = emailHostModel.EmailHost;

            // Port we will connect to on the Amazon SES SMTP endpoint. We are choosing port 587 because we will use
            // STARTTLS to encrypt the connection.
            const int PORT = 587;

            // Create an SMTP client with the specified host name and port.
            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                // Create a network credential with your SMTP user name and password.
                client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                client.EnableSsl = true;

                // Send the email. 
                try
                {
                    Console.WriteLine("Attempting to send an email through the Amazon SES SMTP interface...");
                    client.Send(FROM, TO, SUBJECT, BODY);
                    Console.WriteLine("Email sent!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                }
            }

        }

        /// <summary>
        /// Send plain text email message
        /// </summary>
        /// <param name="emailAddress">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="message">Email message</param>
        void IEmailService.SendEmail(string emailAddress, string subject, string message, object data)
        {
            #region Validation

            if (String.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentException(String.Format(Global.CannotBeNullOrEmpy, "emailAddress"), "emailAddress");
            }

            if (String.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException(String.Format(Global.CannotBeNullOrEmpy, "subject"), "subject");
            }

            if (String.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(String.Format(Global.CannotBeNullOrEmpy, "message"), "message");
            }

            #endregion

            // emailAddress may contain a list of email addresses. For example: "user1@mail.com, user2@mail.com"
            // so.. let's split them into an array
            var emailAddresses = emailAddress.Split(new char[] { ',', ';' })
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()).ToArray();

            MailMessage mailMessage = new MailMessage();

            //Recipient address
            foreach (var email in emailAddresses)
            {
                mailMessage.To.Add(new MailAddress(emailAddress));
            }

            // Sender
            mailMessage.Sender = new MailAddress("walkboy0602@gmail.com");

            // From
            mailMessage.From = new MailAddress(emailHostModel.EmailFrom, this.configService.GetValue(ConfigName.WebsiteTitle));

            // Set subject
            mailMessage.Subject = subject;

            // Is it HTML message?
            if (message.Contains("<p>")) // TODO: add more advanced logic here
            {
                mailMessage.IsBodyHtml = true;
            }

            mailMessage.Body = message;

            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.HeadersEncoding = Encoding.UTF8;

            /// Create a thread for sending email
            Thread t = new Thread(() => SendEmailAsyncThread(mailMessage, false, data));
            t.Start();

        }

        /// <summary>
        /// Send email message with a template
        /// </summary>
        /// <param name="emailAddress">ecipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="templateName">Template name. Ex.: "Contact"</param>
        /// <param name="data">Data object for the template. Ex.: new { Name = "John" }</param>
        void IEmailService.SendEmail(SendEmailModel sendEmailModel, string templateName, object data)
        {
            if (String.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentException(String.Format(Global.CannotBeNullOrEmpy, "templateName"), "templateName");
            }

            var viewData = data as ViewDataDictionary ?? new ViewDataDictionary { Model = data };
            viewData["SendEmailModel"] = sendEmailModel;

            var tempData = new TempDataDictionary();

            if (String.IsNullOrWhiteSpace(sendEmailModel.WebsiteURL))
            {
                throw new ApplicationException("Missing Website URL property.");
            }

            using (var stringWriter = new StringWriter())
            {
                var httpResponse = new HttpResponse(stringWriter);
                var newHttpContext = new HttpContext(new HttpRequest("/", sendEmailModel.WebsiteURL, ""), httpResponse) { User = new GenericPrincipal(new GenericIdentity(""), null) };
                var controllerContext = new ControllerContext();
                controllerContext.HttpContext = new HttpContextWrapper(newHttpContext);
                controllerContext.RequestContext = new RequestContext(new HttpContextWrapper(newHttpContext),
                                                                      new RouteData());
                controllerContext.RouteData.Values.Add("controller", "foo");

                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext,
                                                                                  String.Format(
                                                                                      "~/Views/EmailTemplates/{0}.cshtml",
                                                                                      templateName));
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData,
                                                          httpResponse.Output);
                viewResult.View.Render(viewContext, httpResponse.Output);
                httpResponse.Flush();

                var message = httpResponse.Output.ToString().Trim();

                (this as IEmailService).SendEmail(sendEmailModel.EmailAddress, sendEmailModel.Subject, message, data);
            }
        }

        /*
        public Task SendEmailAsync(string emailAddress, string title, string template, object data)
        {
            var task = new Task(() => this.SendEmail(emailAddress, title, template, data), TaskCreationOptions.None);
            task.Start();

            // for testing purposes
            return task;
        }
        */


        private void SendEmailAsyncThread(MailMessage message, bool isSystemEmail, object userState = null, bool isSendAysnc = false)
        {
            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(emailHostModel.EmailHost, Convert.ToInt16(emailHostModel.EmailPort)))
            {
                // Create a network credential with your SMTP user name and password.
                client.Credentials = new System.Net.NetworkCredential(emailHostModel.EmailUserName, emailHostModel.EmailPassword);

                // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                client.EnableSsl = true;

                // Set Complete Handler
                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallBack);

                if (isSendAysnc)
                {
                    client.SendAsync(message, userState);
                }
                else
                {
                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

            }
        }

        /// <summary>
        /// Send Email CallBack
        /// </summary>
        private void SendCompletedCallBack(object sender, AsyncCompletedEventArgs e)
        {
            if (e.UserState != null)
            {
                if (e.Error != null)
                {
                    //error
                }
                else
                {
                    //success
                }
            }
        }

        /// <summary>
        /// System Email 
        /// </summary>
        private void SendSystemEmail(string emailSubject, string emailBody, bool isError)
        {
            //MailMessage message = new MailMessage();
            //{
            //    message.From = new MailAddress(emailHostModel.EmailFrom);
            //    message.To.Add(SystemNotificationEmailTo);
            //    if (isError)
            //        message.Subject = EmailSubjectError + emailSubject;
            //    else
            //        message.Subject = EmailSubjectNotication + emailSubject;
            //    message.Body = emailBody.Replace(Environment.NewLine, "<BR />");
            //    message.IsBodyHtml = true;
            //}

            //SendEmailAsync(message, true);

        }


    }
}
