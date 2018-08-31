using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Repository.Models;
using SensorsManager.Web.Api.Throttling;

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
               
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();

                if (error == null)
                {
                    return BadRequest();
                }

                return BadRequest(error.ErrorMessage);
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
            var throttler = new Throttler("newSensorType", 1, 3);
            if (throttler.RequestShouldBeThrottled())
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }
            if (sensorReadingModel == null)
            {
                return BadRequest("You have sent an empty object");
            }
            try
            {

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

                var sensor = sensorRep.
                    GetSensorByAddress(sensorReadingModel.SensorGatewayAddress,
                    sensorReadingModel.SensorClientAddress);

                

                var sensorReading = modelToEntityMap
                    .MapSensorReadingModelToSensorReadingEntity(sensorReadingModel, sensor.Id);


                


                var reading = readingRep.AddSensorReading(sensorReading);

                sensor.Active = true;
                sensorRep.UpdateSensor(sensor);

                HttpContext.Current.Response.AppendHeader("X-RateLimit-RequestLimit",
                   throttler.RequestLimit.ToString());

                HttpContext.Current.Response.AppendHeader("X-RateLimit-RequestsRemaining",
                  throttler.RequestsRemaining.ToString());

                HttpContext.Current.Response.AppendHeader("X-RateLimit-ExpiresAt",
                                   throttler.ExpiresAt.ToString());

                return CreatedAtRoute("GetSensorReadingsBySensorAddressRoute", 
                    new { gatewayAddress = sensorReadingModel.SensorGatewayAddress,
                    clientAddress = sensorReadingModel.SensorClientAddress}, sensorReadingModel);

            }
            catch (NullReferenceException)
            {
                return Content(HttpStatusCode.NotFound,
                    String.Format("There is no sensor with the address:{0}/{1}",
                    sensorReadingModel.SensorGatewayAddress, sensorReadingModel.SensorClientAddress));
            }
          

        }


        [Route("~/api/sensors/{id:int}/readings", Name = "GetSensorReadingsBySensorIdRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorReadingsBySensorId(int id, int page = 1, int pageSize = 30)
        {
   

            var totalCount = readingRep.GetSensorReadingBySensorId(id).Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensorReadings = readingRep.GetSensorReadingBySensorId(id)
                                .OrderByDescending(p => p.Id)
                                .Skip(pageSize * (page - 1))
                                .Take(pageSize)
                                .Select(p => modelFactory.CreateSensorReadingModel(p))
                                .ToList();

           if(totalCount == 0)
            {
                return NotFound();
            }



            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-Page", page.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageSize", pageSize.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageCount", totalPages.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-SensorReadingsCount", totalCount.ToString());
           


            return Ok(sensorReadings);
        }

        [Route("~/api/sensors/address/{gatewayAddress}/{clientAddress}/readings", Name = "GetSensorReadingsBySensorAddressRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorReadingsBySensorAddress(string gatewayAddress, string clientAddress, int page = 1, int pageSize = 30)
        {
            var id = sensorRep.GetSensorByAddress(gatewayAddress, clientAddress).Id;
            if(id == 0)
            {
                return NotFound();
            }

            var totalCount = readingRep.GetSensorReadingBySensorId(id).Count();
            var totalPages = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensorReadings = readingRep.GetSensorReadingBySensorId(id)
                                .OrderByDescending(p => p.Id)
                                .Skip(pageSize * (page - 1))
                                .Take(pageSize)
                                .Select(p => modelFactory.CreateSensorReadingModel(p))
                                .ToList();

            if (totalCount == 0)
            {
                return NotFound();
            }

            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-Page", page.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageSize", pageSize.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageCount", totalPages.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-SensorReadingsCount", totalCount.ToString());
            

            return Ok(sensorReadings);

        }
    }
}