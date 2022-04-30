using Common.Dto;
using Common.Enums;
using DataAccess.Data.Context;
using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicAnalizer
{
    public class AnalyzerLogic
    {
        public List<NotificationDto> Analyze( MeasurementDto measurement)
        {
            var result = new List<NotificationDto>();
            var rules = GetAnalyzeRules(measurement.SensorId.Value);
            foreach (var rule in rules)
            {
                if (AnalizeByRule(rule, measurement.Value.Value))
                {
                    result.Add(new NotificationDto 
                    {
                        Id = Guid.NewGuid(),
                        MeasurementId = measurement.Id,
                        NotificationMsg = rule.NotificationMsg, // TODO: maybe more?
                        NotificationType = rule.NotificationType,
                        TimeStamp = measurement.TimeStamp.Value
                    });
                }                
            }
            return result;
        }

        private bool AnalizeByRule(NotificationRuleDto rule, double value)
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

        private IList<NotificationRuleDto> GetAnalyzeRules(Guid sensorId)
        {
            using var context = MeasurementsContextBuilder.BuildMeasurementsContext();

            var sensorTypeId = context.Sensors.FirstOrDefault(x => x.Id == sensorId).SensorTypeId;
            if (sensorTypeId == Guid.Empty)
                return new List<NotificationRuleDto>();
            var rules = context.NotificationRules
                .Where(x => x.SensorTypeId == sensorTypeId)
                .Select(r => new NotificationRuleDto
                {
                    Id = r.Id,
                    RuleType = r.RuleType,
                    NotificationMsg = r.NotificationMsg,
                    Value = r.Value,
                    SensorTypeId = r.SensorTypeId,
                    NotificationType = r.NotificationType
                }).ToList();

            return rules;
        }


    }
}
