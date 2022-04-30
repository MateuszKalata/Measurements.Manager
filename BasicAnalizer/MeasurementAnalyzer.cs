using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Common.Dto;
using System.Globalization;

namespace BasicAnalizer
{
    public class MeasurementAnalyzer
    {
        string measurementsStreamTopic = "validmeasurements";
        ConsumerConfig configuration = new ConsumerConfig
        {
            BootstrapServers = "localhost:19092,localhost:29092,localhost:39092",
            GroupId = "analyzers",
            EnableAutoCommit = false,
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
                    
                    Console.WriteLine($"[{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture)}]-INFO-Validation started ..."); // TODO: change to LOG
                    while (true)
                    {
                        Console.WriteLine($"[{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture)}]-INFO-Looking for next valid measurement:");
                        var cr = consumer.Consume(cts.Token);
                        try
                        {
                            Console.WriteLine($"[{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture)}]-INFO-Consumed valid measurement from topic {measurementsStreamTopic}, partition: {cr.Partition} | Key: {cr.Message.Key} |"); // TODO: change to LOG
                            MeasurementDto measurement = JsonSerializer.Deserialize<MeasurementDto>(cr.Message.Value);

                            var result = analyzer.Analyze(measurement);

                            if (result.Count > 0)
                            {
                                Console.WriteLine($"[{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture)}]-INFO-Measurement {cr.Message.Key} create {result.Count} notifications."); // TODO: change to LOG
                                var tasks = new List<Task>();
                                result.ForEach(n => tasks.Add(notificationProducer.ProduceNotification(n)));//problem wielu wątków dodać awaitAll albo to co w finally
                                Task.WaitAll(tasks.ToArray());
                            }

                            try
                            {
                                consumer.Commit(cr);
                            }
                            catch (KafkaException e)
                            {
                                Console.WriteLine($"[{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.fff", CultureInfo.InvariantCulture)}]-ERROR-Commit error: {e.Error.Reason}");
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
