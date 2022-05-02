using Common.Dto;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace ValidationService
{
    public class InvalidMeasurmentProducer
    {
        private readonly IProducer<string, string> producer;
        private readonly IConfiguration configuration;
        private readonly ILogger<InvalidMeasurmentProducer> logger;
        private readonly string topic;
        ProducerConfig producerConfig = new ProducerConfig();

        public InvalidMeasurmentProducer(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<InvalidMeasurmentProducer>();

            configuration.GetSection("Kafka:ProducerSettings").Bind(producerConfig);
            topic = configuration.GetValue<string>("InvalidMeasurementsTopic");
            producer = new ProducerBuilder<string, string>(producerConfig).Build();
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

            await producer.ProduceAsync(topic, msg);
            logger.LogInformation($"Invalid masurement with Id: {measurement.Id} - produced");
        }
    }
}
