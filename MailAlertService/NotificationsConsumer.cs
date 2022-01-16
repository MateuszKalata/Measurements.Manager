﻿using Common.Dto;
using Common.Enums;
using Confluent.Kafka;
using DataAccess.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MailAlertService
{
    public class NotificationsConsumer
    {
        // TODO: Load config and topic from file
        string topic = "notifications";
        ConsumerConfig configuration = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "notification_alerts",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        MailSender mailSender;

        public NotificationsConsumer()
        {
            mailSender = new MailSender("user", "passwd");
        }

        public void ConsumeNotifications()
        {

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(configuration).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    Console.WriteLine("Processing started ..."); // TODO: Logger
                    while (true)
                    {
                        Console.WriteLine("Looking for next item:"); // TODO: Logger
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed event from topic {topic}\n| Key: {cr.Message.Key}|"); // TODO: Logger

                        NotificationDto notification = JsonSerializer.Deserialize<NotificationDto>(cr.Message.Value);
                        
                        if(notification.NotificationType == NotificationType.Emergency)
                        {
                            var recipients = GetRecipients();
                            mailSender.SendGmail("Emergency", notification.NotificationMsg, recipients , "user");
                            Console.WriteLine($"Emergency notification {cr.Message.Key} is sent to users"); // TODO: Logger
                        }   
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

        private List<string> GetRecipients()
        {
            using var context = MeasurementsContextBuilder.BuildMeasurementsContext();
            var recipients = context.AlertsConfigs.Select(x => x.Mail).ToList();
            return recipients;
        }
    }
}