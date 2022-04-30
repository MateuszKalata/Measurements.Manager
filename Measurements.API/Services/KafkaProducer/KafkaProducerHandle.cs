using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Measurements.API.Services.KafkaProducer
{
    public class KafkaProducerHandle : IDisposable
    {
        IProducer<byte[], byte[]> kafkaProducer;

        public KafkaProducerHandle(IConfiguration config)
        {
            var producerConfig = new ProducerConfig();
            config.GetSection("Kafka:ProducerSettings").Bind(producerConfig);

            this.kafkaProducer = new ProducerBuilder<byte[], byte[]>(producerConfig).Build();
        }

        public Handle Handle { get => this.kafkaProducer.Handle; }

        public void Dispose()
        {
            kafkaProducer.Flush();
            kafkaProducer.Dispose();
        }
    }
}
