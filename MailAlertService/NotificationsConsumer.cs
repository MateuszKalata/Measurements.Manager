using Common.Dto;
using Common.Enums;
using Confluent.Kafka;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DataAccess.Data.Context;
using System.Linq;

namespace MailAlertService
{
    public class NotificationsConsumer
    {
        // TODO: Load config and topic from file
        private readonly IConfiguration configuration;
        private readonly ILogger<NotificationsConsumer> logger;
        private readonly string topic;
        ConsumerConfig consumerConfig = new ConsumerConfig();
        MailSender mailSender;

        public NotificationsConsumer(IConfiguration configuration, ILoggerFactory loggerFactory, MailSender mailSender)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<NotificationsConsumer>();
            this.mailSender = mailSender;

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
                            if (notification.NotificationType == NotificationType.Emergency)
                            {
                                var recipients = GetRecipients();
                                mailSender.SendGmail("Emergency", notification.NotificationMsg, recipients, "proswbfh@gmail.com");
                                logger.LogInformation($"Emergency notification {cr.Message.Key} is sent to users"); // TODO: Logger
                            }

                            try
                            {
                                consumer.Commit(cr);
                            }
                            catch (KafkaException e)
                            {
                                logger.LogError(e, $"Commit error: {e.Error.Reason}");
                            }
                        }
                        catch (Exception e)
                        {
                            // no commit
                            logger.LogInformation($"Emergency notification {cr.Message.Key} is NOT sent to users");
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
