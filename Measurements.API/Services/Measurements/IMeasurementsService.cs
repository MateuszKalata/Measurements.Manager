using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Measurements.API.Services.Measurements
{
    public interface IMeasurementsService
    {
        Task SaveMeasurements( MeasurementDto measurementDto);
    }
}
