using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class InvalidMeasurementDto : MeasurementDto
    {
        public string ErrorMessage { get; set; }
    }
}
