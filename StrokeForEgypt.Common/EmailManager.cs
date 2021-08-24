using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace StrokeForEgypt.Common
{
    public static class EmailManager
    {
        public static async Task SendMail(string ToEmail, string Subject, string Message)
        {
            await SendMail(new List<string> { ToEmail }, Subject, Message);
        }

        public static async Task SendMail(List<string> ToEmails, string Subject, string Message)
        {
            try
            {
                string _email = "verificationcode@strokeforegypt.com";
                string _epass = "&1^rOz#gpFgl";
                string _dispName = "Stroke For Egypt";

                MailMessage myMessage = new()
                {
                    From = new MailAddress(_email, _dispName),
                    Subject = Subject,
                    Body = Message,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8
                };

                ToEmails.ForEach(mail => myMessage.To.Add(mail));

                using SmtpClient smtp = new()
                {
                    EnableSsl = false,
                    Host = "mail.strokeforegypt.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_email, _epass),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                smtp.SendCompleted += (s, e) => { smtp.Dispose(); };
                await smtp.SendMailAsync(myMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task SendMailWithTemplate(string ToEmail, string Subject, string Message)
        {
            await SendMailWithTemplate(new List<string> { ToEmail }, Subject, Message);
        }

        public static async Task SendMailWithTemplate(List<string> ToEmails, string Subject, string Message)
        {
            try
            {
                string _email = "verificationcode@strokeforegypt.com";
                string _epass = "&1^rOz#gpFgl";
                string _dispName = "Stroke For Egypt";

                string pathToFile = AppMainData.WebRootPath
                           + Path.DirectorySeparatorChar.ToString()
                           + "wwwroot"
                           + Path.DirectorySeparatorChar.ToString()
                           + "Templates"
                           + Path.DirectorySeparatorChar.ToString()
                           + "EmailTemplate"
                           + Path.DirectorySeparatorChar.ToString()
                           + "VerifyEmail.html";

                BodyBuilder builder = new BodyBuilder();
                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                {
                    builder.HtmlBody = SourceReader.ReadToEnd();
                }

                string messageBody = builder.HtmlBody.Replace("{0}", Message);

                MailMessage myMessage = new()
                {
                    From = new MailAddress(_email, _dispName),
                    Subject = Subject,
                    Body = messageBody,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8
                };

                ToEmails.ForEach(mail => myMessage.To.Add(mail));

                using SmtpClient smtp = new()
                {
                    EnableSsl = false,
                    Host = "mail.strokeforegypt.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_email, _epass),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                smtp.SendCompleted += (s, e) => { smtp.Dispose(); };
                await smtp.SendMailAsync(myMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
