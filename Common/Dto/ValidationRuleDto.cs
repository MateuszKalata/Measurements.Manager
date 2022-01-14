using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class ValidationRuleDto
    {
        public Guid Id { get; set; }
        public Guid SensorTypeId { get; set; }
        public int RuleType { get; set; }
        public double Value { get; set; }
        public string ErrorMsg { get; set; }
    }
}
