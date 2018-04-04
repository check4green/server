using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;

namespace SensorsManager.Web.Api.Controllers
{
    [RoutePrefix("api/measurements")]
    public class MeasurementsController : ApiController
    {
        MeasurementRepository measureRep = new MeasurementRepository();

        [Route("", Name = "AddMeasurementRoute")]
        [HttpPost]
        public IHttpActionResult AddMeasurement(Measurement newMeasure)
        {
            if (newMeasure == null || ModelState.IsValid == false)
            {
                return BadRequest();
            }
            var addedMeasure = measureRep.AddMeasurement(newMeasure);
            return CreatedAtRoute("GetMeasurementByIdRoute", new { id = addedMeasure.Id }, addedMeasure);
        }

        [Route("{id}", Name = "GetMeasurementByIdRoute")]
        [HttpGet]
        public Measurement GetMeasurementById(int id)
        {
            var measurement = measureRep.GetMeasurementById(id);
            return measurement;
        }

        [Route("", Name = "GetAllMeasurementsRoute")]
        [HttpGet]
        public List<Measurement> GetMeasurements()
        {
            var measurements = measureRep.GetAllMeasurements();
            return measurements.OrderBy(p => p.Id).ToList();
        }

        [Route("{id}", Name = "DeleteMeasurementRoute")]
        [HttpDelete]
        public void DeleteMeasurement(int id)
        {
            measureRep.DeleteMeasurement(id);
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult UpdateMeasurement(int id, Measurement measurement)
        {
            if (measurement == null || ModelState.IsValid == false || id != measurement.Id)
            {
                return BadRequest();
            }
            var result = measureRep.GetMeasurementById(id);
            if (result == null)
            {
                return NotFound();
            }
            measureRep.UpdateMeasurement(measurement);
            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}