﻿using SensorsManager.Web.Api.Repository;
using System.Linq;
using System.Net;
using System.Web.Http;
using System;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using System.Net.Http;
using SensorsManager.Web.Api.Repository.Models;
using System.Web.Http.Results;
using System.Web;
using SensorsManager.Web.Api.Throttling;
using SensorsManager.Web.Api.Security;

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
        UserRepository userRep = new UserRepository();
        ModelFactory modelFactory = new ModelFactory();
        ModelToEntityMap modelToEntityMap = new ModelToEntityMap();
      

        [SensorsManagerAuthorize]
        [Route("", Name = "AddSensorRoute")]
        [HttpPost]
        public IHttpActionResult AddSensor(SensorModel3 sensorModel)
        {
         
            if (sensorModel == null)
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

            
            if(sensorTypeRep.GetSensorTypeById(sensorModel.SensorTypeId) == null)
            {
                return Content(HttpStatusCode.NotFound,
                    new { Message = String.Format("There is no sensor type with the id:{0}.",
                   sensorModel.SensorTypeId)});
            }

            if (sensorModel.GatewayAddress == sensorModel.ClientAddress)
            {
                return BadRequest("The gateway and client addresses must be distinct.");
            }

            if(sensorRep.GetAllSensors().Where(p => p.ClientAddress == sensorModel.ClientAddress).Count()
                != 0)
            {
                return Content(HttpStatusCode.Conflict,
                  new { Message = String.Format("There already is a sensor with that client address.",
                 sensorModel.SensorTypeId)});
            }

            if(sensorRep.GetAllSensors().Where(p => p.GatewayAddress == sensorModel.ClientAddress).Count()
                != 0)
            {
                return Content(HttpStatusCode.Conflict,
                  new{ Message = String.Format("Your client address matches an existing gateway address.",
                 sensorModel.SensorTypeId)});
            }

            if(sensorRep.GetAllSensors().Where(p => p.ClientAddress== sensorModel.GatewayAddress).Count()
                != 0)
            {
                return Content(HttpStatusCode.Conflict,
                  new{ Message = String.Format("Your gateway address matches an existing client address.",
                 sensorModel.SensorTypeId) });
            }

            var compareName = sensorRep.GetAllSensors()
               .Where(p =>
               p.Name == sensorModel.Name).Count();
            if (compareName != 0)
            {
                return Content(HttpStatusCode.Conflict,
                    new { Message = String.Format("There already is a sensor with that name.")});
            }

            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            var userId = userRep.GetUser(credentials.Email, credentials.Password).Id;

            var sensor = modelToEntityMap.MapSensorModelToSensorEntity(sensorModel, userId);
            var addedSensor = sensorRep.AddSensor(sensor);


            return CreatedAtRoute("GetSensorRoute",
                new { id = addedSensor.Id },
                addedSensor);

        }

        [SensorsManagerAuthorize]
        [Route("address/{gatewayAddress}/{clientAddress}", Name = "GetSensorByAddressRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorByAddress(string gatewayAddress, string clientAddress)
        {

            var sensor = sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);

            if (sensor == null)
            {
                return NotFound();
            }

            var sensorModel = modelFactory.CreateSensorModel(sensor);

            return Ok(sensorModel);
        }

        [SensorsManagerAuthorize]
        [Route("address/{gatewayAddress}", Name = "GetSensorByGatewayAddressRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorsBygatewayAddress(string gatewayAddress, int page = 1, int pageSize = 30)
        {


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
            

            return Ok(sensorModels);
        }

        [SensorsManagerAuthorize]
        [Route("{id:int}", Name = "GetSensorRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorById(int id)
        {
  

            var sensor = sensorRep.GetSensorById(id);
            if (sensor == null)
            {
                return NotFound();
            }

            var sensorModel = modelFactory.CreateSensorModel(sensor);

 ;
            return Ok(sensorModel);
        }

        [SensorsManagerAuthorize]
        [Route("", Name = "GetAllSensorsRoute")]
        [HttpGet]
        public IHttpActionResult GetAllSensors(int page = 1, int pageSize = 30)
        {
 

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
           

            return Ok(sensorModels);
        }

        [SensorsManagerAuthorize]
        [Route("~/api/users/sensors", Name = "GetSensorsByUser")]
        [HttpGet]
        public IHttpActionResult GetSensorsByUser(int page = 1, int pageSize = 30)
        {

            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            var userId = userRep.GetUser(credentials.Email, credentials.Password).Id;
            var totalCount = sensorRep.GetAllSensors().Where(p => p.UserId == userId).Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensors = sensorRep
                .GetAllSensors().Where(p => p.UserId == userId)
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
          

            return Ok(sensors);
        }

        [SensorsManagerAuthorize]
        [Route("~/api/sensor-types/{id}/sensors", Name = "GetSensorsBySensorType")]
        [HttpGet]
        public IHttpActionResult GetSensorsBySensorType(int id, int page = 1, int pageSize = 30)
        {
    
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

            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-Page", page.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageSize", pageSize.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-PageCount", pageCount.ToString());
            HttpContext.Current.Response.AppendHeader("X-Tracker-Pagination-SensorCount", totalCount.ToString());


            return Ok(sensors);
        }

        [SensorsManagerAuthorize]
        [Route("~/api/sensor-types/{id}/users/sensors")]
        [HttpGet]
        public IHttpActionResult GetSensorsSensorTypeAndUser(int id, int page = 1, int pageSize = 30)
        {
            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            var userId = userRep.GetUser(credentials.Email, credentials.Password).Id;
            var totalCount = sensorRep.GetAllSensors().Where(p => p.SensorTypeId == id &&
            p.UserId == userId).Count();

            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensors = sensorRep.GetAllSensors().Where(p => p.SensorTypeId == id && p.UserId == userId)
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

            return Ok(sensors);

        }
  

        [SensorsManagerAuthorize]
        [Route("{id:int}")]
        [HttpPut]
        public IHttpActionResult UpdateSensor(int id, SensorModel2 sensorModel)
        {
         

            if (sensorModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            if (!ModelState.IsValid)
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
        
            return StatusCode(HttpStatusCode.NoContent);
        }

        [SensorsManagerAuthorize]
        [Route("address/{gatewayAddress}/{clientAddress}")]
        [HttpPut]
        public IHttpActionResult UpdateSensorByAddress(string gatewayAddress, 
            string clientAddress, SensorModel2 sensorModel)
        {
           

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
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();

                if (error == null)
                {
                    return BadRequest();
                }

                return BadRequest(error.ErrorMessage); ;
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

            return StatusCode(HttpStatusCode.NoContent);
        }

         [SensorsManagerAuthorize]
        [Route("~/api/registered-sensors/{id}")]
        [HttpPut]
        public IHttpActionResult RegisterSensor(int id)
        {
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        [SensorsManagerAuthorize]
        [Route("{id:int}", Name = "DeleteSensorByIdRoute")]
        [HttpDelete]
        public void DeleteSensorById(int id) {
            sensorRep.DeleteSensor(id);
        }

        [SensorsManagerAuthorize]
        [Route("address/{gatewayAddress}/{clientAddress}", Name = "DeleteSensorAddressRoute")]
        [HttpDelete]
        public void DeleteSensorByAddress(string gatewayAddress, string clientAddress)
        {
                var sensor = sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);
                if (sensor != null)
                {
                    sensorRep.DeleteSensor(sensor.Id);
                }
        }
    }
}
