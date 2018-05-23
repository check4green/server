using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Models;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*", 
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-SensorMeasurementCount")]
    [RoutePrefix("api/measurements")]
    public class MeasurementsController : ApiController
    {
        MeasurementRepository measureRep = new MeasurementRepository();
        ModelFactory modelFactory = new ModelFactory();

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


        [Route("{id:int}", Name = "GetMeasurementByIdRoute")]
        [HttpGet]
        public HttpResponseMessage GetMeasurementById(int id)
        {
            var measurement = modelFactory.CreateMeasurementsModel(measureRep.GetMeasurementById(id));

            if (measurement == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, measurement);
            return response;
        }

        [Route("", Name = "GetAllMeasurementsRoute")]
        [HttpGet]
        public HttpResponseMessage GetAllMeasurements(int page = 0, int pageSize = 30)
        {

            var totalCount = measureRep.GetAllMeasurements().Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);

            var measurements = measureRep.GetAllMeasurements()
                .Skip(pageSize * page)
                .Take(pageSize).Select(p => modelFactory.CreateMeasurementsModel(p))
                .OrderBy(p => p.Id).ToList();

            if(totalCount == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, measurements);
            response.Headers.Add("X-Tracker-Pagination-Page", page.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageSize", pageSize.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageCount", totalPages.ToString());
            response.Headers.Add("X-Tracker-Pagination-SensorMeasurementCount", totalCount.ToString());

            return response;

        }

        [Route("{id:int}", Name = "DeleteMeasurementRoute")]
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