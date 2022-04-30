using Common.Dto;
using Confluent.Kafka;
using DataAccess.Data.Context;
using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationsSaver
{
    public class NotificationSaver
    {
        // TODO: Load config and topic from file
        string topic = "notifications";
        ConsumerConfig configuration = new ConsumerConfig
        {
            BootstrapServers = "localhost:19092,localhost:29092,localhost:39092",
            GroupId = "notifications_saver",
            EnableAutoCommit = false
        };

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
                        Console.WriteLine("Looking for next notification:"); // TODO: Logger
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed notification from topic {topic}\n| Key: {cr.Message.Key}|"); // TODO: Logger

                        NotificationDto notification = JsonSerializer.Deserialize<NotificationDto>(cr.Message.Value);
                        try
                        {
                            var context = MeasurementsContextBuilder.BuildMeasurementsContext();
                            context.Notifications.Add(new NotificationEntity()
                            {
                                Id = notification.Id,
                                MeasurementId = notification.MeasurementId,
                                NotificationType = notification.NotificationType,
                                NotificationMsg = notification.NotificationMsg,
                                TimeStamp = notification.TimeStamp
                            });
                            context.SaveChanges();
                            consumer.Commit(cr);
                            Console.WriteLine($"Notification {cr.Message.Key} is saved in DB"); // TODO: Logger
                        }
                        catch (Exception e)
                        {
                            //no commit
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
    }
}
