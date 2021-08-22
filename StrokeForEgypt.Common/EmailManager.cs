using System;
using System.Collections.Generic;
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
                string _email = "support@mail.com";
                string _epass = "Password123";
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
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
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
