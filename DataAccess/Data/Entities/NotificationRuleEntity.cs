using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class NotificationRuleEntity : EntityBase
    {
        public Guid SensorTypeId { get; set; }
        public int RuleType { get; set; }
        public double Value { get; set; }
        public string NotificationMsg { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
