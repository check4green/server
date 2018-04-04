using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Repository;

namespace SensorsManager.Web.Api.Controllers
{
    public class SensorReadingsController : ApiController
    {
        SensorReadingRepository readingRep = new SensorReadingRepository();

        [Route("~/api/readings")]
        [HttpPost]
        public IHttpActionResult AddSensorReadings(SensorReading sensorReading)
        {
            if (sensorReading == null)
            {
                return BadRequest();
            }

            sensorReading.InsertDate = DateTime.UtcNow.ToLocalTime();

            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }
            var reading = readingRep.AddSensorReading(sensorReading);

            return CreatedAtRoute("GetSensorReadingsBySensorIdRoute", new { id = reading.SensorId }, reading);
        }

        [Route("~/api/sensors/{id}/readings", Name = "GetSensorReadingsBySensorIdRoute")]
        [HttpGet]
        public List<SensorReading> GetSensorReadingsBySensorId(int id)
        {
            var senorReadings = readingRep.GetSensorReadingBySensorId(id);
            return senorReadings.ToList();
        }
    }
}