using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public NotificationType NotificationType { get; set; }
        public string NotificationMsg { get; set; }
        public Guid MeasurementId { get; set; }
    }
}
