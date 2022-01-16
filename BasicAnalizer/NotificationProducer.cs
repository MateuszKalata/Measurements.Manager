using Common.Dto;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ValidationService
{
    public class NotificationProducer
    {
        IProducer<string, string> producer;

        public NotificationProducer()
        {
            // TODO: Use config here in the featrure for Producer Config and topic
            ProducerConfig config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceNotification(NotificationDto notification)
        {
            var msg = new Message<string, string>()
            {
                Key = notification.Id.ToString(),
                Value = JsonSerializer.Serialize(notification)
            };
            await producer.ProduceAsync("notifications", msg);
        }
    }
}
