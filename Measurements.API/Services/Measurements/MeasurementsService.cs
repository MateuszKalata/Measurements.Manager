using Common.Dto;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Measurements.API.Services.KafkaProducer;

namespace Measurements.API.Services.Measurements
{
    public class MeasurementsService : IMeasurementsService
    {
        private readonly KafkaDependentProducer<string, string> kafkaDependentProducer;
        private string measurementsStreamTopic;

        public MeasurementsService( KafkaDependentProducer<string, string> kafkaDependentProducer, IConfiguration configuration )
        {
            this.kafkaDependentProducer = kafkaDependentProducer;
            measurementsStreamTopic = configuration.GetValue<string>("MeasurementsTopic");
        }

        public async Task SaveMeasurements(MeasurementDto measurementDto)
        {
            Guid newMeasurementId = Guid.NewGuid();
            measurementDto.Id = newMeasurementId;
            string messageJson = JsonSerializer.Serialize(measurementDto);

            var newMeasurementMsg = new Message<string, string>()
            {
                Key = newMeasurementId.ToString(),
                Value = messageJson,
                Timestamp = new Timestamp(DateTime.UtcNow)
            };

            kafkaDependentProducer.Produce(measurementsStreamTopic, newMeasurementMsg, (deliveryReport) =>
            {
                if (deliveryReport.Error.Code != ErrorCode.NoError)
                {
                    //Log($"Failed to deliver message: {deliveryReport.Error.Reason}");
                }
                if (deliveryReport.Status == PersistenceStatus.NotPersisted)
                {
                    //Log("Failed to persist message");
                }
                else
                {
                    //Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
                }
            });
        }
    }
}
