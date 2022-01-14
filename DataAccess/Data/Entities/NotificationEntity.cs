using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class NotificationEntity : EntityBase
    {
        public DateTime TimeStamp { get; set; }
        public NotificationType NotificationType { get; set; }
        public string NotificationMsg { get; set; }
        public Guid MeasurementId { get; set; }
    }
}
