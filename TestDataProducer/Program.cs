using Common.Dto;
using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading;

namespace TestDataProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };
            string measurementsStreamTopic = "measurements";

            using var measurementsProducer = new ProducerBuilder<string, string>(configuration).Build();

            for (int i = 0; i < 10; i++)
            {
                var measurementDto = new MeasurementDto
                {
                    Id = Guid.NewGuid(),
                    Value = i,
                    Unit = "C",
                    TimeStamp = DateTime.UtcNow,
                    SensorId = Guid.NewGuid()
                };
                string messageJson = JsonSerializer.Serialize(measurementDto);

                var newMeasurementMsg = new Message<string, string>()
                {
                    Key = measurementDto.Id.ToString(),
                    Value = messageJson,
                    Timestamp = new Timestamp(DateTime.UtcNow)
                };

                measurementsProducer.Produce(measurementsStreamTopic, newMeasurementMsg);

                Thread.Sleep(5000);
            }
            
        }
    }
}
