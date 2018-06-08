using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Repository;
using System.Linq;
using System.Net;
using System.Web.Http;
using System;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using System.Net.Http;
using SensorsManager.Web.Api.Repository.Models;

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
            if (sensorModel == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            if(sensorTypeRep.GetSensorTypeById(sensorModel.SensorTypeId) == null)
            {
                return NotFound();
            }

            if(!sensorModel.AddressValidation(sensorModel.GatewayAddress)
                || !sensorModel.AddressValidation(sensorModel.ClientAddress))
            {
                return BadRequest();
            }


            var compare = sensorRep.GetAllSensors()
                .Where(p => 
                p.ClientAddress == sensorModel.ClientAddress 
                || p.GatewayAddress == sensorModel.ClientAddress
                || p.ClientAddress == sensorModel.GatewayAddress).Count();

            

            if (compare != 0)
            {
                return Content(HttpStatusCode.Conflict,compare.ToString());
            }


            var sensor = modelToEntityMap.MapSensorModelToSensorEntity(sensorModel);
            var addedSensor = sensorRep.AddSensor(sensor);

            return CreatedAtRoute("GetSensorRoute",
                new { id = addedSensor.Id },
                addedSensor);

        }

        [Route("address/{gatewayAddress}/{clientAddress}", Name = "GetSensorByAddressRoute")]
        [HttpGet]
        public HttpResponseMessage GetSensorByAddress(string gatewayAddress, string clientAddress)
        {

            var sensor = sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);

            if (sensor == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var sensorModel = modelFactory.CreateSensorModel(sensor);

            var response = Request.CreateResponse(HttpStatusCode.OK, sensorModel);
            return response; 
        }

        [Route("address/{gatewayAddress}", Name = "GetSensorByGatewayAddressRoute")]
        [HttpGet]
        public HttpResponseMessage GetSensorsBygatewayAddress(string gatewayAddress, int page = 0, int pageSize = 30)
        {
            var totalCount = sensorRep.GetSensosByGatewayAddress(gatewayAddress).Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            var sensorModels = sensorRep.GetSensosByGatewayAddress(gatewayAddress)
               .Skip(pageSize * page)
               .Take(pageSize)
               .OrderByDescending(p => p.Id)
               .Select(p => modelFactory.CreateSensorModel(p))
               .ToList();

            if(sensorModels.Count() == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, sensorModels);

            response.Headers.Add("X-Tracker-Pagination-Page", page.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageSize", pageSize.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageCount", pageCount.ToString());
            response.Headers.Add("X-Tracker-Pagination-SensorCount", totalCount.ToString());

            return response;
        }

        [Route("{id:int}", Name = "GetSensorRoute")]
        [HttpGet]
        public HttpResponseMessage GetSensorById(int id)
        {
            var sensor = sensorRep.GetSensorById(id);
            if (sensor == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var sensorModel = modelFactory.CreateSensorModel(sensor);
            

            var response = Request.CreateResponse(HttpStatusCode.OK, sensorModel);
            return response ;
        }

        [Route("", Name = "GetAllSensorsRoute")]
        [HttpGet]
        public HttpResponseMessage GetAllSensors(int page = 0, int pageSize = 30)
        {

            var totalCount = sensorRep.GetAllSensors().Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            var sensorModels = sensorRep.GetAllSensors()
                .Skip(pageSize * page)
                .Take(pageSize)
                .OrderByDescending(p => p.Id)
                .Select(p => modelFactory.CreateSensorModel(p))
                .ToList();

            if(sensorModels.Count() == 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, sensorModels);
            response.Headers.Add("X-Tracker-Pagination-Page", page.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageSize", pageSize.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageCount", pageCount.ToString());
            response.Headers.Add("X-Tracker-Pagination-SensorCount", totalCount.ToString());

            return response;
        }

        [Route("~/api/users/{id}/sensors", Name = "GetSensorsByUser")]
        [HttpGet]
        public HttpResponseMessage GetSensorsByUser(int id, int page = 0, int pageSize = 30)
        {

            var totalCount = sensorRep.GetAllSensors().Where(p => p.UserId == id).Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            var sensors = sensorRep
                .GetAllSensors().Where(p => p.UserId == id)
                .Skip(pageSize * page)
                .Take(pageSize)
                .OrderByDescending(p => p.Id)
                .Select(p => modelFactory.CreateSensorModel(p))
                .ToList();

            var response = Request.CreateResponse(HttpStatusCode.OK, sensors);
            response.Headers.Add("X-Tracker-Pagination-Page", page.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageSize", pageSize.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageCount", pageCount.ToString());
            response.Headers.Add("X-Tracker-Pagination-SensorCount", totalCount.ToString());

            return response;
        }

        [Route("~/api/sensor-types/{id}/sensors", Name = "GetSensorsBySensorType")]
        [HttpGet]
        public HttpResponseMessage GetSensorsBySensorType(int id, int page = 0, int pageSize = 30)
        {
            var totalCount = sensorRep.GetAllSensors().Where(p => p.SensorTypeId == id).Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            var sensors = sensorRep.GetAllSensors().Where(p => p.SensorTypeId == id)
                .Skip(pageSize * page)
                .Take(pageSize)
                .OrderByDescending(p => p.Id)
                .Select(p => modelFactory.CreateSensorModel(p))
                .ToList();

            var response = Request.CreateResponse(HttpStatusCode.OK, sensors);
            response.Headers.Add("X-Tracker-Pagination-Page", page.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageSize", pageSize.ToString());
            response.Headers.Add("X-Tracker-Pagination-PageCount", pageCount.ToString());
            response.Headers.Add("X-Tracker-Pagination-SensorCount", totalCount.ToString());

            return response;
        }


        [Route("{id:int}")]
        [HttpPut]
        public IHttpActionResult UpdateSensor(int id, SensorModel2 sensorModel)
        {
            if (sensorModel == null || ModelState.IsValid == false)
            {
                return BadRequest();
            }
            
            var sensor = sensorRep.GetSensorById(id);
            if (sensor == null)
            {
                return NotFound();
            }

            var result = modelToEntityMap
                .MapSensorModelToSensorEntity(sensorModel, sensor);

            sensorRep.UpdateSensor(result);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("address/{gatewayAddress}/{clientAddress}")]
        [HttpPut]
        public IHttpActionResult UpdateSensorByAddress(string gatewayAddress, 
            string clientAddress, SensorModel2 sensorModel)
        {
            
            var sensor = sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);

            if (sensor == null)
            {
                return NotFound();
            }

            var rezult = modelToEntityMap
                .MapSensorModelToSensorEntity(sensorModel, sensor);
            sensorRep.UpdateSensor(sensor);

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
        public HttpResponseMessage DeleteSensorById(int id) {

           
            sensorRep.DeleteSensor(id);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [Route("address/{gatewayAddress}/{clientAddress}", Name = "DeleteSensorAddressRoute")]
        [HttpDelete]
        public HttpResponseMessage DeleteSensorByAddress(string gatewayAddress, string clientAddress)
        {
        
            sensorRep.DeleteSensorByAdress(gatewayAddress, clientAddress);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}
