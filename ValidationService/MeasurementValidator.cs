using Confluent.Kafka;
using System;
using Common.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;

namespace ValidationService
{
    public class MeasurementValidator
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MeasurementValidator> logger;
        private readonly string topic;
        private readonly ConsumerConfig consumerConfig = new ConsumerConfig();
        private readonly ValidMeasurmentProducer validMeasurmentProducer;
        private readonly InvalidMeasurmentProducer invalidMeasurmentProducer;
        private readonly ValidationLogic validationLogic;

        public MeasurementValidator(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            ValidMeasurmentProducer validMeasurmentProducer,
            InvalidMeasurmentProducer invalidMeasurmentProducer,
            ValidationLogic validationLogic)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<MeasurementValidator>();
            this.validMeasurmentProducer = validMeasurmentProducer;
            this.invalidMeasurmentProducer = invalidMeasurmentProducer;
            this.validationLogic = validationLogic;

            configuration.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);
            topic = configuration.GetValue<string>("MeasurementsTopic");
        }

        public void ConsumeMeasurements()
        {

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        logger.LogInformation($"Consumed event from topic {topic}\n| Key: {cr.Message.Key} | Value: {cr.Message.Value} | Timestamp: {cr.Message.Timestamp} |"); // TODO: change to LOG
                        MeasurementDto measurement = JsonSerializer.Deserialize<MeasurementDto>(cr.Message.Value);

                        try
                        {
                            var result = validationLogic.Validate(measurement);

                            if (string.IsNullOrEmpty(result))
                            {
                                logger.LogInformation($"Measurement: {cr.Message.Key} is valid.");
                                validMeasurmentProducer.ProduceValidMsg(cr.Message).Wait();
                            }
                            else
                            {
                                logger.LogInformation($"Measurement: {cr.Message.Key} is NOT valid.");
                                invalidMeasurmentProducer.ProduceInvalidMeasurement(measurement, result).Wait();
                            }

                            try
                            {
                                consumer.Commit(cr);
                            }
                            catch (KafkaException e)
                            {
                                logger.LogError(e, $"Commit error: {e.Error.Reason}");
                            }
                        }
                        catch (Exception e)
                        {
                            //no commit
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl+C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
