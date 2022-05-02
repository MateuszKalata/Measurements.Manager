using Common.Dto;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;


namespace ValidationService
{
    public class ValidMeasurmentProducer
    {
        private readonly IProducer<string, string> producer;
        private readonly IConfiguration configuration;
        private readonly ILogger<ValidMeasurmentProducer> logger;
        private readonly string topic;
        ProducerConfig producerConfig = new ProducerConfig();

        public ValidMeasurmentProducer(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<ValidMeasurmentProducer>();

            configuration.GetSection("Kafka:ProducerSettings").Bind(producerConfig);
            topic = configuration.GetValue<string>("ValidMeasurementsTopic");
            producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task ProduceValidMsg(Message<string, string> measurementMsg)
        {
            await producer.ProduceAsync(topic, measurementMsg);
            logger.LogInformation($"Masurement with key: {measurementMsg.Key} - produced");
        }
    }
}
