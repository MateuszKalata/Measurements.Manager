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

namespace InvalidMsgSaver
{
    public class InvalidMeasurementsSaver
    {
        // TODO: Load config and topic from file
        string topic = "invalidmeasurements";
        ConsumerConfig configuration = new ConsumerConfig
        {
            BootstrapServers = "localhost:19092,localhost:29092,localhost:39092",
            GroupId = "invalid_measurement_saver",
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
                    Console.WriteLine("Processing started ..."); // TODO: Logger
                    while (true)
                    {
                        Console.WriteLine("Looking for next item:"); // TODO: Logger
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed event from topic {topic}\n| Key: {cr.Message.Key}|"); // TODO: Logger

                        InvalidMeasurementDto measurement = JsonSerializer.Deserialize<InvalidMeasurementDto>(cr.Message.Value);
                        try
                        {
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
                            consumer.Commit(cr);
                            Console.WriteLine($"Measurement {cr.Message.Key} is saved in DB"); // TODO: Logger
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
