using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailAlertService
{
    public class MailSender
    {
        private SmtpClient gmailClient;
        public MailSender(string username, string passwd)
        {
            gmailClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, passwd)
            };
        }

        public bool SendGmail(string subject, string content, List<string> recipients, string from)
        {
            if (recipients == null || recipients.Count == 0)
                Console.WriteLine("throw new ArgumentException('recipients');");

            using (var msg = new MailMessage(from, recipients[0], subject, content))
            {
                for (int i = 1; i < recipients.Count; i++)
                    msg.To.Add(recipients[i]);

                try
                {
                    gmailClient.Send(msg);
                    return true;
                }
                catch (Exception e)
                {
                    // TODO: Handle the exception
                    //return false;
                    throw;
                }
            }
        }
    }
}
