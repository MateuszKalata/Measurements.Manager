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
    public class MeasurementValidator
    {
        string measurementsStreamTopic = "measurements";
        ConsumerConfig configuration = new ConsumerConfig
        {
            BootstrapServers = "localhost:19092,localhost:29092,localhost:39092",
            GroupId = "validators",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        ValidMeasurmentProducer validMeasurmentProducer;
        InvalidMeasurmentProducer invalidMeasurmentProducer;
        ValidationLogic validator;

        public MeasurementValidator()
        {
            // TODO: Use config here in the featrure for topic & Consumer Config
            validMeasurmentProducer = new ValidMeasurmentProducer();
            invalidMeasurmentProducer = new InvalidMeasurmentProducer();
            validator = new ValidationLogic();
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
                        Console.WriteLine("Looking for next item:");
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed event from topic {measurementsStreamTopic}\n| Key: {cr.Message.Key} | Value: {cr.Message.Value} | Timestamp: {cr.Message.Timestamp} |"); // TODO: change to LOG
                        MeasurementDto measurement = JsonSerializer.Deserialize<MeasurementDto>(cr.Message.Value);

                        var result = validator.Validate(measurement);

                        if (string.IsNullOrEmpty(result))
                        {
                            Console.WriteLine($"Measurement: {cr.Message.Key} is valid."); // TODO: change to LOG
                            validMeasurmentProducer.ProduceValidMsg(cr.Message);
                        }
                        else
                        {
                            Console.WriteLine($"Measurement: {cr.Message.Key} is NOT valid."); // TODO: change to LOG
                            invalidMeasurmentProducer.ProduceInvalidMeasurement(measurement, result);
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
