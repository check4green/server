using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System;

namespace SensorsManager.Web.Api.Controllers
{
    [RoutePrefix("api/sensors")]
    public class SensorsController : ApiController
    {
        SensorRepository sensorRep = new SensorRepository();

        [Route("", Name = "AddSensorRoute")]
        [HttpPost]
        public IHttpActionResult AddSensor(Sensor sensor)
        {
            if(sensor == null)
            {
                return BadRequest();
            }
            sensor.UserId = 1;
            if (ModelState.IsValid == false)
            {
                return BadRequest();
            }
            var addedSensorReading = sensorRep.AddSensor(sensor);
            return CreatedAtRoute("GetAllSensorsRoute", new {id = addedSensorReading.Id }, addedSensorReading);
            
        }

        [Route("{id:int}", Name = "GetSensorRoute")]
        [HttpGet]
        public Sensor GetSensor(int id)
        {
            var sensor = sensorRep.GetSensorById(id);
            return sensor;
        }

        [Route("", Name = "GetAllSensorsRoute")]
        [HttpGet]
        public List<Sensor> GetAllSensors()
        {
            var sensors = sensorRep.GetAllSensors();
            return sensors.OrderBy(p => p.Id).ToList();
        }

        [Route("~/api/users/{id}/sensors", Name = "GetSensorsByUser")]
        [HttpGet]
        public List<Sensor> GetSensorsByUser(int id)
        {
            var sensors = sensorRep.GetAllSensors().Where(p => p.UserId == id);
            return sensors.OrderBy(p => p.Id).ToList();
        }

        [Route("{id}", Name = "DeleteSensorRoute")]
        [HttpDelete]
        public void DeleteSensor(int id)
        {
            sensorRep.DeleteSensor(id);
        }

        [Route("{id}")]
        [HttpPut]
        public IHttpActionResult UpdateSensor(int id, Sensor sensor)
        {
            if(sensor == null || ModelState.IsValid == false || id != sensor.Id)
            {
                return BadRequest();
            }
            var result = sensorRep.GetSensorById(id);
            if(result == null)
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
        
        //ReadingsController 
       
    }
}
