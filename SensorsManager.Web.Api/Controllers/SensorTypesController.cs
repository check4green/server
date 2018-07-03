using SensorsManager.DomainClasses;
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
                if (sensorTypeModel == null)
                {
                    return BadRequest("You have sent an empty object.");
                }
                if (ModelState.IsValid == false)
                {
                    var message = ModelState.SelectMany(m => m.Value.Errors)
                   .FirstOrDefault().ErrorMessage
                   .ToString();
                    return BadRequest(message);
            }
                var measure = measureRep.GetMeasurementById(sensorTypeModel.MeasureId);
                if (measure == null)
                {
                    return Content(HttpStatusCode.NotFound,
                       new
                       {
                         Message =  String.Format("There is no measurement with the id:{0}",
                       sensorTypeModel.MeasureId)
                       });
                }

                var sensorType = modelToEntityMap.MapSensorTypeModelToSensorTypeEnrity(sensorTypeModel);
                var addedSensorType = sensorTypeRep.AddSensorType(sensorType);

                return CreatedAtRoute("GetSensorTypeByIdRoute", new { id = addedSensorType.Id }, addedSensorType);
            }

            [Route("{id:int}", Name = "GetSensorTypeByIdRoute")]
            [HttpGet]
            public HttpResponseMessage GetSensorTypeById(int id)
            {

            var sensorType = sensorTypeRep.GetSensorTypeById(id);
               if(sensorType == null)
               {
                  return new  HttpResponseMessage(HttpStatusCode.NotFound);
               }
            var sensorTypeModel = modelFactory.CreateSensorTypeModel(sensorType);
            var response = Request.CreateResponse(HttpStatusCode.OK, sensorTypeModel);
               return response;
            }


            [Route("", Name = "GetAllSensorsTypeRoute")]
            [HttpGet]
            public HttpResponseMessage GetAllSensorTypes(int page = 1, int pageSize = 30)
            {
            var totalCount = sensorTypeRep.GetAllSensorTypes().Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);

            if(page < 1) { page = 1; }
            if(pageSize < 1) { pageSize = 30; }

            var sensorTypes = sensorTypeRep.GetAllSensorTypes()
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize)
                    .OrderBy(p => p.Id)
                    .Select(p => modelFactory.CreateSensorTypeModel(p)).ToList();

            if(totalCount == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, sensorTypes);
            response.Headers.Add("X-Tracker-Pagination-Page", page.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageSize", pageSize.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageCount", totalPages.ToString());
            response.Headers.Add("X-Tracker-Pagination-SensorTypeCount", totalCount.ToString());

            return response;
        }

            

            [Route("{id:int}", Name = "DeleteSensorTypeRoute")]
            [HttpDelete]
            public HttpResponseMessage DeleteSensorType(int id)
            {
            sensorTypeRep.DeleteSensorType(id);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            [Route("{id:int}")]
            [HttpPut]
            public IHttpActionResult UpdateSensorType(int id,SensorTypeModel sensorTypeModel)
            {
                if (sensorTypeModel == null || ModelState.IsValid == false || id != sensorTypeModel.Id)
                {
                    return BadRequest();
                }

                var result = sensorTypeRep.GetSensorTypeById(id);

                if(result == null)
                {
                    return NotFound();
                }
                 var sensorType = modelToEntityMap.MapSensorTypeModelToSensorTypeEntity(sensorTypeModel, result);

                 sensorTypeRep.UpdateSensorType(sensorType);

                 return StatusCode(HttpStatusCode.NoContent);
            }

        }

    }

