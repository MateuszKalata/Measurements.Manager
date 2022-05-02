using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace MailAlertService
{
    public class MailSender
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MailSender> logger;
        private readonly SmtpClient gmailClient;
        public MailSender(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<MailSender>();
            var mail = configuration.GetValue<string>("Mail");
            var passwd = configuration.GetValue<string>("MailPassword");
            gmailClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, passwd)
            };
        }

        public bool SendGmail(string subject, string content, List<string> recipients, string from)
        {
            if (recipients == null || recipients.Count == 0)
                logger.LogError(new ArgumentException(), "Lack of recipients");

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
                    logger.LogError(e, "Something went wrong");
                    throw;
                }
            }
        }
    }
}
