using Common.Dto;
using Confluent.Kafka;
using DataAccess.Data.Context;
using DataAccess.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
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
        private readonly IConfiguration configuration;
        private readonly ILogger<NotificationSaver> logger;
        private readonly string topic;
        ConsumerConfig consumerConfig = new ConsumerConfig();

        public NotificationSaver(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<NotificationSaver>();
            configuration.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);
            topic = configuration.GetValue<string>("NotificationsTopic");
        }

        public void ConsumeNotifications()
        {

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        logger.LogInformation($"Consumed notification from topic {topic}\n| Key: {cr.Message.Key}|"); // TODO: Logger

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
                            logger.LogInformation($"Notification {cr.Message.Key} is saved in DB"); // TODO: Logger
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, "Something went wrong");
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
