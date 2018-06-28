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
using SensorsManager.Web.Api.Repository.Models;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-SensorReadingsCount")]

    public class SensorReadingsController : ApiController
    {
  
        SensorReadingRepository readingRep = new SensorReadingRepository();
        SensorRepository sensorRep = new SensorRepository();
        SensorTypesRepository typeRep = new SensorTypesRepository();

        ModelFactory modelFactory = new ModelFactory();
        ModelToEntityMap modelToEntityMap = new ModelToEntityMap();

        [Route("~/api/readings")]
        [HttpPost]
        public IHttpActionResult AddSensorReadings(SensorReadingModel3 sensorReadingModel)
        {
            if (sensorReadingModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            if (ModelState.IsValid == false)
            {
                var message = ModelState.SelectMany(m => m.Value.Errors)
                    .SingleOrDefault().ErrorMessage
                    .ToString();
                return BadRequest(message);
            }

            var sensor = sensorRep.GetSensorById(sensorReadingModel.SensorId);
         
            if(sensor == null)
            {
                return Content(HttpStatusCode.NotFound,
                   new
                   {
                     Message =  String.Format("There is no sensor with the id:{0}",
                    sensorReadingModel.SensorId)
                   });
            }

            var sensorType = typeRep.GetSensorTypeById(sensor.SensorTypeId);
           
            if(sensorReadingModel.Value < sensorType.MinValue 
                || sensorReadingModel.Value > sensorType.MaxValue)
            {
                return BadRequest(String.Format("Reading must be between {0} and {1}",
                    sensorType.MinValue, sensorType.MinValue));
            }

            var sensorReading = modelToEntityMap
                .MapSensorReadingModelToSensorReadingEntity(sensorReadingModel);

            var reading = readingRep.AddSensorReading(sensorReading);

            sensor.Active = true;
            sensorRep.UpdateSensor(sensor);


            return CreatedAtRoute("GetSensorReadingsBySensorIdRoute", new { id = reading.SensorId }, reading);
        }

        [Route("~/api/readings/address")]
        [HttpPost]
        public IHttpActionResult AddSensorReadingsByAddress(SensorReadingModel2 sensorReadingModel)
        {
            if (sensorReadingModel == null)
            {
                return BadRequest("You have sent an empty object");
            }
            try
            {

                if (ModelState.IsValid == false)
                {
                    var message = ModelState.SelectMany(m => m.Value.Errors)
                   .SingleOrDefault().ErrorMessage
                   .ToString();
                    return BadRequest(message);
                }

                var sensor = sensorRep.
                    GetSensorByAddress(sensorReadingModel.SensorGatewayAddress,
                    sensorReadingModel.SensorClientAddress);

                var sensorType = typeRep.GetSensorTypeById(sensor.SensorTypeId);

                if (sensorReadingModel.Value < sensorType.MinValue
                    || sensorReadingModel.Value > sensorType.MaxValue)
                {
                    return BadRequest(String.Format("Reading must be between {0} and {1}",
                    sensorType.MinValue, sensorType.MinValue));
                }

                var sensorReading = modelToEntityMap
                    .MapSensorReadingModelToSensorReadingEntity(sensorReadingModel, sensor.Id);

               
                var reading = readingRep.AddSensorReading(sensorReading);

                sensor.Active = true;
                sensorRep.UpdateSensor(sensor);

                return CreatedAtRoute("GetSensorReadingsBySensorAddressRoute", 
                    new { gatewayAddress = sensorReadingModel.SensorGatewayAddress,
                    clientAddress = sensorReadingModel.SensorClientAddress}, sensorReadingModel);

            }
            catch (System.NullReferenceException)
            {
                return Content(HttpStatusCode.NotFound,
                    String.Format("There is no sensor with the address:{0}/{1}",
                    sensorReadingModel.SensorGatewayAddress, sensorReadingModel.SensorClientAddress));
            }
          

        }


        [Route("~/api/sensors/{id:int}/readings", Name = "GetSensorReadingsBySensorIdRoute")]
        [HttpGet]
        public HttpResponseMessage GetSensorReadingsBySensorId(int id, int page = 0, int pageSize = 30)
        {
            var totalCount = readingRep.GetSensorReadingBySensorId(id).Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);



            var senorReadings = readingRep.GetSensorReadingBySensorId(id)
                                .OrderByDescending(p => p.Id)
                                .Skip(pageSize * page)
                                .Take(pageSize)
                                .Select(p => modelFactory.CreateSensorReadingModel(p))
                                .ToList();

           if(totalCount == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, senorReadings);
            
            response.Headers.Add("X-Tracker-Pagination-Page", page.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageSize", pageSize.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageCount", totalPages.ToString());
            response.Headers.Add("X-Tracker-Pagination-SensorReadingsCount", totalCount.ToString());

            

            return response;
        }

        [Route("~/api/sensors/address/{gatewayAddress}/{clientAddress}/readings", Name = "GetSensorReadingsBySensorAddressRoute")]
        [HttpGet]
        public HttpResponseMessage GetSensorReadingsBySensorAddress(string gatewayAddress, string clientAddress, int page = 0, int pageSize = 30)
        {
            var id = sensorRep.GetSensorByAddress(gatewayAddress, clientAddress).Id;
            if(id == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            return GetSensorReadingsBySensorId(id, page, pageSize);
           
        }
    }
}