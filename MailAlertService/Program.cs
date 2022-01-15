using System;
using System.Net;
using System.Net.Mail;

namespace MailAlertService
{
    class Program
    {
        static void Main(string[] args)
        {
            var notificationAlertsSender = new NotificationsConsumer();
            notificationAlertsSender.ConsumeNotifications();
        }

        
    }
}
