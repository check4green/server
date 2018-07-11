using SensorsManager.Web.Api.Repository;
using System.Linq;
using System.Net;
using System.Web.Http;
using System;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using System.Net.Http;
using SensorsManager.Web.Api.Repository.Models;
using System.Web.Http.Results;
using SensorsManager.Web.Api.Validations;
using System.Web;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-SensorCount")]
    [RoutePrefix("api/sensors")]
    public class SensorsController : ApiController
    {
        SensorRepository sensorRep = new SensorRepository();
        SensorTypesRepository sensorTypeRep = new SensorTypesRepository();
        ModelFactory modelFactory = new ModelFactory();
        ModelToEntityMap modelToEntityMap = new ModelToEntityMap();

        [Route("", Name = "AddSensorRoute")]
        [HttpPost]
        public IHttpActionResult AddSensor(SensorModel3 sensorModel)
        {
            var throttler = new Throttler("newreading", 1, 3);
            if (throttler.RequestShouldBeThrottled())
            {
                return new System.Web.Http.Results.ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("To many requests"))
                    );
            }


            if (sensorModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            if (ModelState.IsValid == false)
            {
               
                var message = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "").FirstOrDefault().ErrorMessage.ToString();
       
                return BadRequest(message);
            }

            
            if(sensorTypeRep.GetSensorTypeById(sensorModel.SensorTypeId) == null)
            {
                return Content(HttpStatusCode.NotFound,
                    new { Message = String.Format("There is no sensor type with the id:{0}",
                   sensorModel.SensorTypeId)});
            }


            if (sensorModel.GatewayAddress == sensorModel.ClientAddress)
            {
                return BadRequest("The gateway and sensor addresses must be distinct.");
            }

            var compareAddress = sensorRep.GetAllSensors()
                .Where(p => 
                p.ClientAddress == sensorModel.ClientAddress 
                || p.GatewayAddress == sensorModel.ClientAddress
                || p.ClientAddress == sensorModel.GatewayAddress).Count();

            var compareName = sensorRep.GetAllSensors()
                .Where(p =>
                p.Name == sensorModel.Name).Count();        

            if (compareAddress != 0)
            {
                return Content(HttpStatusCode.Conflict,
                  new { Message = String.Format("There already is a sensor with that address",
                 sensorModel.SensorTypeId)});
            }

            if (compareName != 0)
            {
                return Content(HttpStatusCode.Conflict,
                    new { Message = String.Format("There already is a sensor with that name.",
                   sensorModel.SensorTypeId)});
            }



            var sensor = modelToEntityMap.MapSensorModelToSensorEntity(sensorModel);
            var addedSensor = sensorRep.AddSensor(sensor);

            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit",
                    throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout",
                               throttler.TimeoutInSeconds.ToString());

            return CreatedAtRoute("GetSensorRoute",
                new { id = addedSensor.Id },
                addedSensor);

        }

        [Route("address/{gatewayAddress}/{clientAddress}", Name = "GetSensorByAddressRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorByAddress(string gatewayAddress, string clientAddress)
        {
            var throttler = new Throttler("getSensor",1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("To many requests"))
                    );
            }
            var sensor = sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);

            if (sensor == null)
            {
                return NotFound();
            }

            var sensorModel = modelFactory.CreateSensorModel(sensor);
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit",
                    throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout",
                               throttler.TimeoutInSeconds.ToString());
            return Ok(sensorModel);
        }

        [Route("address/{gatewayAddress}", Name = "GetSensorByGatewayAddressRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorsBygatewayAddress(string gatewayAddress, int page = 1, int pageSize = 30)
        {
            var throttler = new Throttler("getSensors", 1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("To many requests"))
                    );
            }

            var totalCount = sensorRep.GetSensosByGatewayAddress(gatewayAddress).Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensorModels = sensorRep.GetSensosByGatewayAddress(gatewayAddress)
               .OrderByDescending(p => p.Id)
               .Skip(pageSize * (page - 1))
               .Take(pageSize)
               .Select(p => modelFactory.CreateSensorModel(p))
               .ToList();

            if(sensorModels.Count() == 0)
            {
                return NotFound();
            }


            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-Page", page.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageSize", pageSize.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageCount", pageCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-SensorCount", totalCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit",throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout",throttler.TimeoutInSeconds.ToString());

            return Ok(sensorModels);
        }

        [Route("{id:int}", Name = "GetSensorRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorById(int id)
        {
            var throttler = new Throttler("getSensor", 1, 2);

            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests"))
                    );
            }

            var sensor = sensorRep.GetSensorById(id);
            if (sensor == null)
            {
                return NotFound();
            }

            var sensorModel = modelFactory.CreateSensorModel(sensor);

            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit",
                    throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout",
                               throttler.TimeoutInSeconds.ToString());
            return Ok(sensorModel);
        }

        [Route("", Name = "GetAllSensorsRoute")]
        [HttpGet]
        public IHttpActionResult GetAllSensors(int page = 1, int pageSize = 30)
        {
            var throttler = new Throttler("newreading", 1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("To many requests"))
                    );
            }

            var totalCount = sensorRep.GetAllSensors().Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensorModels = sensorRep.GetAllSensors()
                .OrderByDescending(p => p.Id)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => modelFactory.CreateSensorModel(p))
                .ToList();

            if(sensorModels.Count() == 0)
            {
                return NotFound();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, sensorModels);
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-Page", page.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageSize", pageSize.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageCount", pageCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-SensorCount", totalCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());

            return Ok(sensorModels);
        }

        [Route("~/api/users/{id}/sensors", Name = "GetSensorsByUser")]
        [HttpGet]
        public IHttpActionResult GetSensorsByUser(int id, int page = 1, int pageSize = 30)
        {
            var throttler = new Throttler("getSensors", 1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests"))
                    );
            }

            var totalCount = sensorRep.GetAllSensors().Where(p => p.UserId == id).Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensors = sensorRep
                .GetAllSensors().Where(p => p.UserId == id)
                .OrderByDescending(p => p.Id)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => modelFactory.CreateSensorModel(p))
                .ToList();

            if (sensors.Count() == 0)
            {
                return NotFound();
            }

            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-Page", page.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageSize", pageSize.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageCount", pageCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-SensorCount", totalCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit",throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout",throttler.TimeoutInSeconds.ToString());

            return Ok(sensors);
        }

        [Route("~/api/sensor-types/{id}/sensors", Name = "GetSensorsBySensorType")]
        [HttpGet]
        public IHttpActionResult GetSensorsBySensorType(int id, int page = 1, int pageSize = 30)
        {
            var throttler = new Throttler("getSensors", 1, 2);
            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }

            var totalCount = sensorRep.GetAllSensors().Where(p => p.SensorTypeId == id).Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensors = sensorRep.GetAllSensors().Where(p => p.SensorTypeId == id)
                .OrderByDescending(p => p.Id)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => modelFactory.CreateSensorModel(p))
                .ToList();

            if (sensors.Count() == 0)
            {
                return NotFound();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, sensors);
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-Page", page.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageSize", pageSize.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageCount", pageCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-SensorCount", totalCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());

            return Ok(sensors);
        }


        [Route("{id:int}")]
        [HttpPut]
        public IHttpActionResult UpdateSensor(int id, SensorModel2 sensorModel)
        {
            var throttler = new Throttler("updateSensor", 1, 3);
            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }

            if (sensorModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            if (!ModelState.IsValid)
            {
                var message = ModelState.SelectMany(m => m.Value.Errors)
                   .Where(m => m.ErrorMessage != "").FirstOrDefault().ErrorMessage.ToString();

                return BadRequest(message);
            }
            

            var sensor = sensorRep.GetSensorById(id);
            if (sensor == null)
            {
                return NotFound();
            }


            var compareName = sensorRep.GetAllSensors()
                .Where(p =>
                p.Name == sensorModel.Name
                && p.Id != id).Count();

            if(compareName != 0)
            {
                return Content(HttpStatusCode.Conflict,
                   new
                   {
                       Message = String.Format("There already is a sensor with that name.")
                   });
            }

            var result = modelToEntityMap
                .MapSensorModelToSensorEntity(sensorModel, sensor);

            sensorRep.UpdateSensor(result);
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("address/{gatewayAddress}/{clientAddress}")]
        [HttpPut]
        public IHttpActionResult UpdateSensorByAddress(string gatewayAddress, 
            string clientAddress, SensorModel2 sensorModel)
        {
            var throttler = new Throttler("updateSensor", 1, 3);
            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }

            if (sensorModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            var sensor = sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);

            if (sensor == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var message = ModelState.SelectMany(m => m.Value.Errors)
                   .Where(m => m.ErrorMessage != "").FirstOrDefault().ErrorMessage.ToString();

                return BadRequest(message);
            }

            var compareName = sensorRep.GetAllSensors()
               .Where(p =>
               p.Name == sensorModel.Name
               && !String.Equals(p.GatewayAddress + p.ClientAddress
               , gatewayAddress + clientAddress)).Count();

            if (compareName != 0)
            {
                return Content(HttpStatusCode.Conflict,
                   new
                   {
                       Message = String.Format("There already is a sensor with that name.")
                   });
            }

            var rezult = modelToEntityMap
                .MapSensorModelToSensorEntity(sensorModel, sensor);
            sensorRep.UpdateSensor(rezult);

            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("~/api/registered-sensors/{id}")]
        [HttpPut]
        public IHttpActionResult RegisterSensor(int id)
        {

            return StatusCode(HttpStatusCode.NotImplemented);
        }

        [Route("{id:int}", Name = "DeleteSensorByIdRoute")]
        [HttpDelete]
        public IHttpActionResult DeleteSensorById(int id) {

            var throttler = new Throttler("newreading", 1, 1);
            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }


            sensorRep.DeleteSensor(id);
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("address/{gatewayAddress}/{clientAddress}", Name = "DeleteSensorAddressRoute")]
        [HttpDelete]
        public IHttpActionResult DeleteSensorByAddress(string gatewayAddress, string clientAddress)
        {
            var throttler = new Throttler("newreading", 1, 3);
            if (throttler.RequestShouldBeThrottled())
            {
                return new ResponseMessageResult(
                        Request.CreateResponse((HttpStatusCode)429,
                        new HttpError("Too many requests."))
                    );
            }

            sensorRep.DeleteSensorByAdress(gatewayAddress, clientAddress);
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Limit", throttler.RequestLimit.ToString());
            HttpContext.Current.Response.AppendHeader("X-RateLimit-Timeout", throttler.TimeoutInSeconds.ToString());
            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}
