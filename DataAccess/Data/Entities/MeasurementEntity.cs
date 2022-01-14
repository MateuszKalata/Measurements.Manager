using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Data.Entities
{
    public class MeasurementEntity : EntityBase
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid SensorId { get; set; }
    }
}
