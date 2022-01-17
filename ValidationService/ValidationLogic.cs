using Common.Dto;
using Common.Enums;
using DataAccess.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationService
{
    public class ValidationLogic
    {
        public string Validate( MeasurementDto measurement)
        {
            string validationMsg = "";

            if (!ValidateSensor(measurement.SensorId.Value))
            {
                validationMsg += $"| There is no registered sensor with id: {measurement.SensorId.Value} |";
            }
            else if(!ValidateUnit(measurement.SensorId.Value, measurement.Unit))
            {
                validationMsg += $"| Expected unit from sensor with id ({measurement.SensorId.Value}) is other than {measurement.Unit}. |";
            }
            if (!ValidateTimeStamp(measurement.TimeStamp.Value))
            {
                validationMsg += $"| Measurement was send with future date. |";
            }

            var rules = GetValidationRules(measurement.SensorId.Value);
            foreach (var rule in rules)
            {
                if (!ValidateAgainstRule(rule, measurement.Value.Value))
                {
                    validationMsg += $"| {rule.ErrorMsg} |";
                }                  
            }

            return validationMsg;
        }

        private bool ValidateAgainstRule(ValidationRuleDto rule, double value)
        {
            if (rule.RuleType % 2 == (int)RuleType.Equal)
            {
                if(rule.RuleType - 1 == (int)RuleType.Greater)
                {
                    return value >= rule.Value;
                }
                else if (rule.RuleType - 1 == (int)RuleType.Smaller)
                {
                    return value <= rule.Value;
                }
            }
            else if (rule.RuleType % 2 != (int)RuleType.Equal)
            {
                if (rule.RuleType == (int)RuleType.Greater)
                {
                    return value > rule.Value;
                }
                else if (rule.RuleType == (int)RuleType.Smaller)
                {
                    return value < rule.Value;
                }
            }
            return true;
        }

        private bool ValidateSensor(Guid sensorId)
        {
            using var context = MeasurementsContextBuilder.BuildMeasurementsContext();
            return context.Sensors.Any(x => x.Id == sensorId);
        }

        private bool ValidateUnit(Guid sensorId, string unit)
        {
            using var context = MeasurementsContextBuilder.BuildMeasurementsContext();
            var sensorTypeId = context.Sensors.First(x => x.Id == sensorId).SensorTypeId;
            var expectedUnit = context.SensorTypes.Find(sensorTypeId)?.Unit;
            return expectedUnit == unit;
        }

        private bool ValidateTimeStamp(DateTime timestamp)
        {
            return DateTime.UtcNow > timestamp;
        }

        private IList<ValidationRuleDto> GetValidationRules(Guid sensorId)
        {
            using var context = MeasurementsContextBuilder.BuildMeasurementsContext();

            var sensorTypeId = context.Sensors.FirstOrDefault(x => x.Id == sensorId)?.SensorTypeId;
            if (sensorTypeId == null)
                return new List<ValidationRuleDto>();
            var rules = context.ValidationRules
                .Where(x => x.SensorTypeId == sensorTypeId)
                .Select(r => new ValidationRuleDto
                {
                    Id = r.Id,
                    RuleType = r.RuleType,
                    ErrorMsg = r.ErrorMsg,
                    Value = r.Value,
                    SensorTypeId = r.SensorTypeId
                }).ToList();

            return rules;
        }


    }
}
