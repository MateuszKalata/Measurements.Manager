using Common.Dto;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationService
{
    public class ValidMeasurmentProducer
    {
        IProducer<string, string> producer;

        public ValidMeasurmentProducer()
        {
            // TODO: Use config here in the featrure for Producer Config and topic
            ProducerConfig config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceValidMsg(Message<string, string> measurementMsg)
        {
            await producer.ProduceAsync("validmeasurements", measurementMsg);
        }
    }
}
