using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class MeasurementDto
    {
        public Guid? Id { get; set; }
        public double? Value { get; set; }
        public string Unit { get; set; }
        public DateTime? TimeStamp { get; set; }
        public Guid? SensorId { get; set; }
    }
}
