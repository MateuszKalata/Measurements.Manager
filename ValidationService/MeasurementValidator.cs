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
            BootstrapServers = "localhost:9092",
            GroupId = "validators",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        ValidMeasurmentProducer validMeasurmentProducer;
        InvalidMeasurmentProducer invalidMeasurmentProducer;
        ValidationLogic validator;

        public MeasurementValidator()
        {
            validMeasurmentProducer = new ValidMeasurmentProducer();
            invalidMeasurmentProducer = new InvalidMeasurmentProducer();
            validator = new ValidationLogic();
        }

        public void ConsumeMeasurements()
        {

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(configuration).Build())
            {
                consumer.Subscribe(measurementsStreamTopic);
                try
                {
                    Console.WriteLine("Processing started ...");
                    while (true)
                    {
                        Console.WriteLine("Looking for next item:");
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed event from topic {measurementsStreamTopic}\n| Key: {cr.Message.Key} | Value: {cr.Message.Value} | Timestamp: {cr.Message.Timestamp} |");
                        MeasurementDto measurement;
                        measurement = JsonSerializer.Deserialize<MeasurementDto>(cr.Message.Value);

                        var result = validator.Validate(measurement);

                        if (string.IsNullOrEmpty(result))
                        {
                            Console.WriteLine($"Measurement: {cr.Message.Key} is valid.");
                            validMeasurmentProducer.ProduceValidMsg(cr.Message);
                        }
                        else
                        {
                            Console.WriteLine($"Measurement: {cr.Message.Key} is NOT valid.");
                            invalidMeasurmentProducer.ProduceInvalidMeasurement(measurement, result);
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
