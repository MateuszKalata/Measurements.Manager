using System;

namespace NotificationsSaver
{
    class Program
    {
        static void Main(string[] args)
        {
            NotificationSaver notificationSaver = new NotificationSaver();
            notificationSaver.ConsumeNotifications();
        }
    }
}
