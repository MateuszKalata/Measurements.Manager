using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Common.Dto;

namespace ValidationService
{
    public class MeasurementAnalyzer
    {
        string measurementsStreamTopic = "validmeasurements";
        ConsumerConfig configuration = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "analyzers",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        NotificationProducer notificationProducer;
        AnalyzerLogic analyzer;

        public MeasurementAnalyzer()
        {
            // TODO: Use config here in the featrure for topic & Consumer Config
            notificationProducer = new NotificationProducer();
            analyzer = new AnalyzerLogic();
        }

        public void ConsumeMeasurements()
        {

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(configuration).Build())
            {
                consumer.Subscribe(measurementsStreamTopic);
                try
                {
                    Console.WriteLine("Validation started ..."); // TODO: change to LOG
                    while (true)
                    {
                        Console.WriteLine("Looking for next valid measurement:");
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed valid measurement from topic {measurementsStreamTopic}\n| Key: {cr.Message.Key} | Value: {cr.Message.Value} | Timestamp: {cr.Message.Timestamp} |"); // TODO: change to LOG
                        MeasurementDto measurement = JsonSerializer.Deserialize<MeasurementDto>(cr.Message.Value);

                        var result = analyzer.Analyze(measurement);

                        if (result.Count > 0)
                        {
                            Console.WriteLine($"Measurement {cr.Message.Key} create {result.Count} notifications."); // TODO: change to LOG
                            result.ForEach(n => notificationProducer.ProduceNotification(n));
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
