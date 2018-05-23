using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Repository;
using System.Net;
using System.Web.Http;
using System.Linq;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using System.Net.Http;
using System;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*", 
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-SensorTypeCount")]
    [RoutePrefix("api/sensor-types")]
    public class SensorTypesController : ApiController
    {
            SensorTypesRepository sensorTypeRep = new SensorTypesRepository();
            ModelFactory modelFactory = new ModelFactory();    

            [Route("", Name = "AddSensorTypeRoute")]
            [HttpPost]
            public IHttpActionResult AddSensorType(SensorType sensorType)
            {
                if (sensorType == null || ModelState.IsValid == false)
                {
                    return BadRequest();
                }

                var addedSensorType = sensorTypeRep.AddSensorType(sensorType);
                return CreatedAtRoute("GetSensorTypeByIdRoute", new { id = addedSensorType.Id }, addedSensorType);
            }

            [Route("{id:int}", Name = "GetSensorTypeByIdRoute")]
            [HttpGet]
            public HttpResponseMessage GetSensorTypeById(int id)
            {
                var sensorType = modelFactory.CreateSensorTypesModel(sensorTypeRep.GetSensorTypeById(id));
                
               if(sensorType == null)
               {
                return new  HttpResponseMessage(HttpStatusCode.NotFound);
               }
               var response = Request.CreateResponse(HttpStatusCode.OK, sensorType);
               return response;
            }


            [Route("", Name = "GetAllSensorsTypeRoute")]
            [HttpGet]
            public HttpResponseMessage GetAllSensorTypes(int page = 0, int pageSize = 30)
            {
            var totalCount = sensorTypeRep.GetAllSensorTypes().Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);

            var sensorTypes = sensorTypeRep.GetAllSensorTypes()
                    .Skip(pageSize * page)
                    .Take(pageSize)
                    .OrderBy(p => p.Id)
                    .Select(p => modelFactory.CreateSensorTypesModel(p)).ToList();

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
            public void DeleteSensorType(int id)
            {
                sensorTypeRep.DeleteSensorType(id);
            }

            [Route("{id:int}")]
            [HttpPut]
            public IHttpActionResult UpdateSensorType(int id,SensorType sensorType)
            {
                if (sensorType == null || ModelState.IsValid == false || id != sensorType.Id)
                {
                return BadRequest();
                }
                var result = sensorTypeRep.GetSensorTypeById(id);
                if(result == null)
                {
                return NotFound();
                }
                sensorTypeRep.UpdateSensorType(sensorType);
                 return StatusCode(HttpStatusCode.NoContent);
            }

        }

    }

