using System;

namespace BasicAnalizer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var measurementsAnalyzer = new MeasurementAnalyzer();
            measurementsAnalyzer.ConsumeMeasurements();
        }
    }
}
