using Confluent.Kafka;
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

        public KafkaProducerHandle(/*IConfiguration config*/)
        {
            // Use config here in the featrure producer config
            // config.GetSection("Kafka:ProducerSettings").Bind(conf);
            var conf = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",//docker.for.win.
                ClientId = Dns.GetHostName()
            };
            this.kafkaProducer = new ProducerBuilder<byte[], byte[]>(conf).Build();
        }

        public Handle Handle { get => this.kafkaProducer.Handle; }

        public void Dispose()
        {
            // Block until all outstanding produce requests have completed (with or
            // without error).
            kafkaProducer.Flush();
            kafkaProducer.Dispose();
        }
    }
}
