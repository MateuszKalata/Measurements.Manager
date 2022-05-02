using Common.Dto;
using Confluent.Kafka;
using DataAccess.Data.Context;
using DataAccess.Data.Entities;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace ValidatedMsgSaver
{
    public class MeasurementsSaver
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MeasurementsSaver> logger;
        private readonly string topic;
        ConsumerConfig consumerConfig = new ConsumerConfig();

        public MeasurementsSaver(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<MeasurementsSaver>();

            configuration.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);
            topic = configuration.GetValue<string>("ValidMeasurementsTopic");
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
                        logger.LogInformation($"Consumed event from topic {topic}\n| Key: {cr.Message.Key}|");

                        MeasurementDto measurement = JsonSerializer.Deserialize<MeasurementDto>(cr.Message.Value);

                        try
                        {
                            var context = MeasurementsContextBuilder.BuildMeasurementsContext();
                            context.Measurements.Add(new MeasurementEntity()
                            {
                                Id = measurement.Id,
                                Value = measurement.Value.Value,
                                Unit = measurement.Unit,
                                TimeStamp = measurement.TimeStamp.Value,
                                SensorId = measurement.SensorId.Value
                            });
                            context.SaveChanges();
                            logger.LogInformation($"Measurement {cr.Message.Key} is saved in DB");

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
                            // no commit
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
