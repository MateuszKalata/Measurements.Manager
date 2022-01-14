using Common.Dto;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationService
{
    public class InvalidMeasurmentProducer
    {
        IProducer<string, string> producer;

        public InvalidMeasurmentProducer()
        {
            ProducerConfig config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceInvalidMeasurement(MeasurementDto measurement, string errorMsg)
        {
            //Map Dto to InvalidMeasurement

            //persist invalis measurement
            await producer.ProduceAsync("invalidmeasurements", new Message<string, string>() { Key = "", Value = "" });
        }
    }
}
