using System;

namespace InvalidMsgSaver
{
    public class Program
    {
        static void Main(string[] args)
        {
            var invalidMeasurementsSaver = new InvalidMeasurementsSaver();
            invalidMeasurementsSaver.ConsumeMeasurements();
        }
    }
}
