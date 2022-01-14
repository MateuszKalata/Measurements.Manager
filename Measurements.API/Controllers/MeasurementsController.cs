using Common.Dto;
using Confluent.Kafka;
using Measurements.API.Services.KafkaProducer;
using Measurements.API.Services.Measurements;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Measurements.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        IMeasurementsService measurementsService;

        public MeasurementsController(IMeasurementsService measurementsService)
        {
            this.measurementsService = measurementsService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterMeasurement([FromBody] MeasurementDto measurementDto)
        {
            if (measurementDto is null)
                return BadRequest();

            if (measurementDto.SensorId == null || measurementDto.Value == null || measurementDto.TimeStamp == null)
                return BadRequest("Incomplete request. One or more required values are null. ");
       
            try
            {
                await measurementsService.SaveMeasurements(measurementDto);
            }
            catch (Exception e)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            return Ok();
        }
    }
}
