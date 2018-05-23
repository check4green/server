using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-SensorReadingsCount")]

    public class SensorReadingsController : ApiController
    {
  
        SensorReadingRepository readingRep = new SensorReadingRepository();
        ModelFactory modelFactory = new ModelFactory();

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

        [Route("~/api/sensors/{id:int}/readings", Name = "GetSensorReadingsBySensorIdRoute")]
        [HttpGet]
        public HttpResponseMessage GetSensorReadingsBySensorId(int id, int page = 0, int pageSize = 30)
        {
            var totalCount = readingRep.GetSensorReadingBySensorId(id).Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);



            var senorReadings = readingRep.GetSensorReadingBySensorId(id)
                                .Skip(pageSize * page)
                                .Take(pageSize)
                                .Select(p => modelFactory.CreateSensorReadingModel(p))
                                .ToList();

           if(totalCount == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, senorReadings);
            
            response.Headers.Add("X-Tracker-Pagination-Page", page.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageSize", pageSize.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageCount", totalPages.ToString());
            response.Headers.Add("X-Tracker-Pagination-SensorReadingsCount", totalCount.ToString());

            

            return response;
        }

        [Route("~/api/sensors/adress/{gatewayAdress}/{clientAdress}/readings", Name = "GetSensorReadingsBySensorAdressRoute")]
        [HttpGet]
        public HttpResponseMessage GetSensorReadingsBySensorAdressId(string gatewayAdress, string clientAdress, int page = 0, int pageSize = 30)
        {

            var totalCount = readingRep.GetSensorReadingBySensorAdress(gatewayAdress, clientAdress).Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);

            var sensorReadings = readingRep.GetSensorReadingBySensorAdress(gatewayAdress, clientAdress)
                                .Skip(pageSize * page)
                                .Take(pageSize)
                                .Select(p => modelFactory.CreateSensorReadingModel(p))
                                .ToList();

            if (totalCount == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, sensorReadings);
            response.Headers.Add("X-Tracker-Pagination-Page", page.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageSize", pageSize.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageCount", totalPages.ToString());
            response.Headers.Add("X-Tracker-Pagination-SensorReadingsCount", totalCount.ToString());

            return response;
        }
    }
}