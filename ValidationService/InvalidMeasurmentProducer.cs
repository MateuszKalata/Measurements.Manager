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
    public class InvalidMeasurmentProducer
    {
        IProducer<string, string> producer;

        public InvalidMeasurmentProducer()
        {
            // TODO: Use config here in the featrure for Producer Config and topic
            ProducerConfig config = new ProducerConfig()
            {
                BootstrapServers = "localhost:19092,localhost:29092,localhost:39092"
            };
            producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceInvalidMeasurement(MeasurementDto measurement, string errorMsg)
        {
            var invalidMeasurement = new InvalidMeasurementDto()
            {
                Id = measurement.Id,
                SensorId = measurement.SensorId,
                TimeStamp = measurement.TimeStamp,
                Value = measurement.Value,
                Unit = measurement.Unit,
                ErrorMessage = errorMsg
            };

            var msg = new Message<string, string>()
            {
                Key = invalidMeasurement.Id.ToString(),
                Value = JsonSerializer.Serialize(invalidMeasurement)
            };

            await producer.ProduceAsync("invalidmeasurements", msg); // TODO: handle dalivery
        }
    }
}
