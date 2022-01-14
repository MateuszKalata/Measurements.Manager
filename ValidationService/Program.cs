using Common.Dto;
using Confluent.Kafka;
using System;
using System.Threading;

namespace ValidationService
{
    class Program
    {
        static void Main(string[] args)
        {
            var validationConsumer = new MeasurementValidator();
            validationConsumer.ConsumeMeasurements();
        }
    }
}
