using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class InvalidMesurementEntity : EntityBase
    {
        public double? Value { get; set; }
        public string? Unit { get; set; }
        public DateTime? TimeStamp { get; set; }
        public Guid? SensorId { get; set; }
        public string ErrorMessage { get; set; } 
    }
}
