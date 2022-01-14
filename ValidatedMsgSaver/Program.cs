using DataAccess.Data.Context;
using DataAccess.Data.Entities;
using System;

namespace ValidatedMsgSaver
{
    class Program
    {
        static void Main(string[] args)
        {
            MeasurementsSaver measurementsSaver = new MeasurementsSaver();
            measurementsSaver.ConsumeMeasurements();
        }
    }
}
