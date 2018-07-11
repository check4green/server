using SensorsManager.Web.Api.Repository;
using System.Net;
using System.Web.Http;
using System.Linq;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using System.Net.Http;
using System;
using SensorsManager.Web.Api.Repository.Models;
using SensorsManager.DataLayer;
using SensorsManager.Web.Api.Validations;
using System.Web;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-SensorTypeCount")]
    [RoutePrefix("api/sensor-types")]
    public class SensorTypesController : ApiController
    {
        SensorTypesRepository sensorTypeRep = new SensorTypesRepository();
        MeasurementRepository measureRep = new MeasurementRepository();
        ModelFactory modelFactory = new ModelFactory();
        ModelToEntityMap modelToEntityMap = new ModelToEntityMap();

        [Route("", Name = "AddSensorTypeRoute")]
        [HttpPost]
        public IHttpActionResult AddSensorType(SensorTypeModel sensorTypeModel)
        {
            var throttler = new Throttler("newSensorType", 1, 3);
            if (throttler.RequestShouldBeThrottled())
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }

            if (sensorTypeModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }
            if (ModelState.IsValid == false)
            {
                var message = ModelState.SelectMany(m => m.Value.Errors)
                .Where(m => m.ErrorMessage != "").FirstOrDefault().ErrorMessage.ToString();
                return BadRequest(message);
            }
            var measure = measureRep.GetMeasurementById(sensorTypeModel.MeasureId);
            if (measure == null)
            {
                return Content(HttpStatusCode.NotFound,
                   new
                   {
                       Message = String.Format("There is no measurement with the id:{0}",
                   sensorTypeModel.MeasureId)
                   });
            }
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit",
                    throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout",
                               throttler.TimeoutInSeconds.ToString());
            var sensorType = modelToEntityMap.MapSensorTypeModelToSensorTypeEnrity(sensorTypeModel);
            var addedSensorType = sensorTypeRep.AddSensorType(sensorType);



            return CreatedAtRoute("GetSensorTypeByIdRoute", new { id = addedSensorType.Id }, addedSensorType);
        }

        [Route("{id:int}", Name = "GetSensorTypeByIdRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorTypeById(int id)
        {
            var throttler = new Throttler("getSensorType", 1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }


            var sensorType = sensorTypeRep.GetSensorTypeById(id);
            if (sensorType == null)
            {
                return NotFound();
            }
            var sensorTypeModel = modelFactory.CreateSensorTypeModel(sensorType);

            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());

            return Ok(sensorTypeModel);
        }


        [Route("", Name = "GetAllSensorsTypeRoute")]
        [HttpGet]
        public IHttpActionResult GetAllSensorTypes(int page = 1, int pageSize = 30)
        {
            var throttler = new Throttler("getSensorTypes", 1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }

            var totalCount = sensorTypeRep.GetAllSensorTypes().Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensorTypes = sensorTypeRep.GetAllSensorTypes()
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize)
                    .OrderBy(p => p.Id)
                    .Select(p => modelFactory.CreateSensorTypeModel(p)).ToList();

            if (totalCount == 0)
            {
                return NotFound();
            }

            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-Page", page.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageSize", pageSize.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageCount", totalPages.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-SensorTypeCount", totalCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());

            return Ok(sensorTypes);
        }

        [Route("{id:int}")]
        [HttpPut]
        public IHttpActionResult UpdateSensorType(int id, SensorTypeModel sensorTypeModel)
        {
            var throttler = new Throttler("updateSensorType", 1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }

            if (sensorTypeModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            if (ModelState.IsValid == false)
            {
                var message = ModelState.SelectMany(m => m.Value.Errors)
                .Where(m => m.ErrorMessage != "").FirstOrDefault().ErrorMessage.ToString();

                return BadRequest(message);
            }
            if (id != sensorTypeModel.Id)
            {
                return BadRequest("The SensorTypeId and the address id do not match.");
            }

            var result = sensorTypeRep.GetSensorTypeById(id);

            if (result == null)
            {
                return NotFound();
            }
            var sensorType = modelToEntityMap.MapSensorTypeModelToSensorTypeEntity(sensorTypeModel, result);

            sensorTypeRep.UpdateSensorType(sensorType);

            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id:int}", Name = "DeleteSensorTypeRoute")]
        [HttpDelete]
        public IHttpActionResult DeleteSensorType(int id)
        {
            var throttler = new Throttler("deleteSensorType", 1, 1);
            if (throttler.RequestShouldBeThrottled())
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }

            sensorTypeRep.DeleteSensorType(id);
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}