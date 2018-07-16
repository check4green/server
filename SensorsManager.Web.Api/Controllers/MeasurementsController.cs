using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using SensorsManager.DataLayer;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository.Models;
using SensorsManager.Web.Api.Validations;

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
        ModelToEntityMap modelToEntityMap = new ModelToEntityMap();

        [Route("", Name = "AddMeasurementRoute")]
        [HttpPost]
        public IHttpActionResult AddMeasurement(MeasurementModel newMeasureModel)
        {
            var throttler = new Throttler("newreading", 1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }


            if (newMeasureModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }
            if(ModelState.IsValid == false)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();

                if (error == null)
                {
                    return BadRequest();
                }

                return BadRequest(error.ErrorMessage);
            }
            var newMeasure = modelToEntityMap.MapMeasurementModelToMeasurementEntity(newMeasureModel);
            var addedMeasure = measureRep.AddMeasurement(newMeasure);

            HttpContext.Current.Response.AppendHeader("X-RateLimit-RequestCount",
                    throttler.RequestCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-ExpiresAt",
                               throttler.ExpiresAt.ToString());
            return CreatedAtRoute("GetMeasurementByIdRoute", new { id = addedMeasure.Id }, addedMeasure);
        }

        [Route("{id:int}", Name = "GetMeasurementByIdRoute")]
        [HttpGet]
        public IHttpActionResult GetMeasurementById(int id)
        {
       
            var measurement = measureRep.GetMeasurementById(id);
            if (measurement == null)
            {
                return NotFound();
            }

            var measurementModel = modelFactory.CreateMeasurementModel(measurement);

           
            return Ok(measurementModel);
        }

        [Route("", Name = "GetAllMeasurementsRoute")]
        [HttpGet]
        public IHttpActionResult GetAllMeasurements(int page = 1, int pageSize = 30)
        {
    

            var totalCount = measureRep.GetAllMeasurements().Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var measurements = measureRep.GetAllMeasurements()
                .Skip(pageSize * (page - 1))
                .Take(pageSize).Select(p => modelFactory.CreateMeasurementModel(p))
                .OrderBy(p => p.Id).ToList();

            if(totalCount == 0)
            {
                return Ok(measurements);
            }

            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-Page", page.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageSize", pageSize.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageCount", totalPages.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-SensorMeasurementCount", totalCount.ToString());
           

            return Ok(measurements);

        }

        [Route("{id:int}")]
        [HttpPut]
        public IHttpActionResult UpdateMeasurement(int id, MeasurementModel measurementModel)
        {

            var throttler = new Throttler("updateMeasurement", 1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }

            if (measurementModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }
            if (ModelState.IsValid == false)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();

                if (error == null)
                {
                    return BadRequest();
                }

                return BadRequest(error.ErrorMessage);
            }

            var result = measureRep.GetMeasurementById(id);
            if (result == null)
            {
                return NotFound();
            }

            var measurement = modelToEntityMap.MapMeasurementModelToMeasurementEntity(measurementModel, result);
            measureRep.UpdateMeasurement(measurement);

            HttpContext.Current.Response.AppendHeader("X-RateLimit-RequestCount",
                     throttler.RequestCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-ExpiresAt",
                               throttler.ExpiresAt.ToString());
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id:int}", Name = "DeleteMeasurementRoute")]
        [HttpDelete]
        public void DeleteMeasurement(int id)
        {
            measureRep.DeleteMeasurement(id);
        }

    }
}