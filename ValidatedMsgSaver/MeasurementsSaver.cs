using Common.Dto;
using Confluent.Kafka;
using DataAccess.Data.Context;
using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ValidatedMsgSaver
{
    public class MeasurementsSaver
    {
        string topic = "validmeasurements";
        ConsumerConfig configuration = new ConsumerConfig
        {
            BootstrapServers = "localhost:19092,localhost:29092,localhost:39092",
            GroupId = "measurement_saver",
            EnableAutoCommit = false
        };

        public void ConsumeMeasurements()
        {

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(configuration).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    Console.WriteLine("Processing started ...");
                    while (true)
                    {
                        Console.WriteLine("Looking for next item:");
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed event from topic {topic}\n| Key: {cr.Message.Key}|");

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
                            //consumer.Commit(cr);
                            consumer.Commit(new List<TopicPartitionOffset>(){ cr.TopicPartitionOffset});
                            Console.WriteLine($"Measurement {cr.Message.Key} is saved in DB");
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
