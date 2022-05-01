using Confluent.Kafka;
using System;
using Common.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace BasicAnalizer
{
    public class MeasurementAnalyzer
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MeasurementAnalyzer> logger;
        private readonly string topic;
        ConsumerConfig consumerConfig = new ConsumerConfig();
        NotificationProducer notificationProducer;
        AnalyzerLogic analyzer;

        public MeasurementAnalyzer(
            IConfiguration configuration, 
            ILoggerFactory loggerFactory, 
            NotificationProducer notificationProducer, 
            AnalyzerLogic analyzer)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<MeasurementAnalyzer>();
            this.notificationProducer = notificationProducer;
            this.analyzer = analyzer;

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
                        try
                        {
                            logger.LogInformation($"Consumed valid measurement from topic {topic}, partition: {cr.Partition} | Key: {cr.Message.Key} |");
                            MeasurementDto measurement = JsonSerializer.Deserialize<MeasurementDto>(cr.Message.Value);

                            var result = analyzer.Analyze(measurement);

                            if (result.Count > 0)
                            {
                                logger.LogInformation($"Measurement {cr.Message.Key} create {result.Count} notifications.");
                                var tasks = new List<Task>();
                                result.ForEach(n => tasks.Add(notificationProducer.ProduceNotification(n)));
                                Task.WaitAll(tasks.ToArray());
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
                            // dont commit
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl+C was pressed.
                }
                finally
                {
                    consumer.Unsubscribe();
                    consumer.Dispose();
                }
            }
        }
    }
}
