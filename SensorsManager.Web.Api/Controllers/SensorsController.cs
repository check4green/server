using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Repository;
using System.Linq;
using System.Net;
using System.Web.Http;
using System;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using System.Net.Http;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-SensorCount")]
    [RoutePrefix("api/sensors")]
    public class SensorsController : ApiController
    {
        SensorRepository sensorRep = new SensorRepository();
        ModelFactory modelFactory = new ModelFactory();

        [Route("", Name = "AddSensorRoute")]
        [HttpPost]
        public IHttpActionResult AddSensor(Sensor sensor)
        {
            if (sensor == null)
            {
                return BadRequest();
            }

            sensor.UserId = 1;

            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }

            var compare = sensorRep.GetAllSensors()
                .Where(p => 
                p.ClientAddress == sensor.ClientAddress 
                || p.GatewayAddress == sensor.ClientAddress
                || p.ClientAddress == sensor.GatewayAddress).Count();

            

            if (compare != 0)
            {
                return Content(HttpStatusCode.Conflict,compare.ToString());
            }

            sensor.GatewayAddress = sensor.GatewayAddress.ToLower();
            sensor.ClientAddress = sensor.ClientAddress.ToLower();

            var addedSensorReading = sensorRep.AddSensor(sensor);

            return CreatedAtRoute("GetSensorByAdressRoute",
                new { gatewayAdress = addedSensorReading.GatewayAddress, clientAdress = addedSensorReading.ClientAddress },
                addedSensorReading);

        }

        [Route("adress/{gatewayAdress}/{clientAdress}", Name = "GetSensorByAdressRoute")]
        [HttpGet]
        public HttpResponseMessage GetSensorByAdress(string gatewayAdress, string clientAdress)
        {
            var sensor = modelFactory.CreateSensorsModel
                (sensorRep.GetSensorByAdress(gatewayAdress, clientAdress));

            if (sensor == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, sensor);
            return response; 
        }

        [Route("adress/{gatewayAdress}", Name = "GetSensorByGatewayAdressRoute")]
        [HttpGet]
        public HttpResponseMessage GetSensorsBygatewayAdress(string gatewayAdress, int page = 0, int pageSize = 30)
        {
            var totalCount = sensorRep.GetSensosByGatewayAdress(gatewayAdress).Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            var sensors = sensorRep.GetSensosByGatewayAdress(gatewayAdress)
               .Skip(pageSize * page)
               .Take(pageSize)
               .OrderBy(p => p.Id)
               .Select(p => modelFactory.CreateSensorsModel(p))
               .ToList();

            var response = Request.CreateResponse(HttpStatusCode.OK, sensors);

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
            var sensor = modelFactory.CreateSensorsModel(sensorRep.GetSensorById(id));
            if(sensor == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, sensor);
            return response ;
        }

        [Route("", Name = "GetAllSensorsRoute")]
        [HttpGet]
        public HttpResponseMessage GetAllSensors(int page = 0, int pageSize = 30)
        {

            var totalCount = sensorRep.GetAllSensors().Count();
            var pageCount = Math.Ceiling((float)totalCount / pageSize);

            var sensors = sensorRep.GetAllSensors()
                .Skip(pageSize * page)
                .Take(pageSize)
                .OrderBy(p => p.Id)
                .Select(p => modelFactory.CreateSensorsModel(p))
                .ToList();

            var response = Request.CreateResponse(HttpStatusCode.OK, sensors);
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
                .OrderBy(p => p.Id)
                .Select(p => modelFactory.CreateSensorsModel(p))
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
                .OrderBy(p => p.Id)
                .Select(p => modelFactory.CreateSensorsModel(p))
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
        public IHttpActionResult UpdateSensor(int id, Sensor sensor)
        {
            if (sensor == null || ModelState.IsValid == false || id != sensor.Id)
            {
                return BadRequest();
            }
            var result = sensorRep.GetSensorById(id);
            if (result == null)
            {
                return NotFound();
            }
            sensorRep.UpdateSensor(sensor);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("adress/{gatewayAdress}/{clientAdress}")]
        [HttpPut]
        public IHttpActionResult UpdateSensorByAdress(string gatewayAdress, string clientAdress, Sensor sensor)
        {
            if (sensor == null || ModelState.IsValid == false
                || gatewayAdress != sensor.GatewayAddress
                || clientAdress != sensor.GatewayAddress)
            {
                return BadRequest();
            }
            var result = sensorRep.GetSensorByAdress(gatewayAdress, clientAdress);
            if (result == null)
            {
                return NotFound();
            }
            sensorRep.UpdateSensor(sensor);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("~/api/registered-sensors/{id}")]
        [HttpPut]
        public IHttpActionResult RegisterSensor(int id)
        {

            return StatusCode(HttpStatusCode.NotImplemented);
        }



    }
}
