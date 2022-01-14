using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class MeasurementModel
    {
        public Guid Id { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime TimeStamp { get; set; }
        public long SourceId { get; set; }

    }
}
