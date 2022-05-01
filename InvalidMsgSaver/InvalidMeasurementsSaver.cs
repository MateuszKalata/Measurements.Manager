using Confluent.Kafka;
using System;
using Common.Dto;
using DataAccess.Data.Context;
using DataAccess.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;

namespace InvalidMsgSaver
{
    public class InvalidMeasurementsSaver
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<InvalidMeasurementsSaver> logger;
        private readonly string topic;
        ConsumerConfig consumerConfig = new ConsumerConfig();

        public InvalidMeasurementsSaver(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<InvalidMeasurementsSaver>();

            configuration.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);
            topic = configuration.GetValue<string>("InvalidMeasurementsTopic");
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
                        
                        try
                        {
                            logger.LogInformation($"Consumed event from topic {topic} | Key: {cr.Message.Key}|");
                            InvalidMeasurementDto measurement = JsonSerializer.Deserialize<InvalidMeasurementDto>(cr.Message.Value);

                            var context = MeasurementsContextBuilder.BuildMeasurementsContext();
                            context.InvalidMesurements.Add(new InvalidMesurementEntity()
                            {
                                Id = measurement.Id,
                                Value = measurement.Value,
                                Unit = measurement.Unit,
                                TimeStamp = measurement.TimeStamp,
                                SensorId = measurement.SensorId
                            });
                            context.SaveChanges();

                            try
                            {
                                consumer.Commit(cr);
                                logger.LogInformation($"Measurement {cr.Message.Key} is saved in DB");
                            }
                            catch (KafkaException e)
                            {
                                logger.LogError(e, $"Commit error: {e.Error.Reason}");
                            }                          
                        }
                        catch (Exception e)
                        {
                            // no commmit
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
