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

namespace BasicAnalizer
{
    public class NotificationProducer
    {
        private readonly IProducer<string, string> producer;
        private readonly IConfiguration configuration;
        private readonly ILogger<NotificationProducer> logger;
        private readonly string topic;
        ProducerConfig producerConfig = new ProducerConfig();

        public NotificationProducer(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<NotificationProducer>();

            configuration.GetSection("Kafka:ProducerSettings").Bind(producerConfig);
            topic = configuration.GetValue<string>("NotificationsTopic");
            producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task ProduceNotification(NotificationDto notification)
        {
            var msg = new Message<string, string>()
            {
                Key = notification.Id.ToString(),
                Value = JsonSerializer.Serialize(notification)
            };
            await producer.ProduceAsync(topic, msg);
            logger.LogInformation($"Notification for masurement ({notification.MeasurementId}) - produced");
        }
    }
}
