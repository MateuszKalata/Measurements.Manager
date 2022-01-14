using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class SensorEntity : EntityBase
    {
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string Location { get; set; }

        public Guid SensorTypeId { get; set; }
        public SensorTypeEntity SensorType { get; set; }
    }
}
