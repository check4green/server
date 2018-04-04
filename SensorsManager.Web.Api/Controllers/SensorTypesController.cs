using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Repository;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Linq;

namespace SensorsManager.Web.Api.Controllers
{
    [RoutePrefix("api/sensor-types")]
    public class SensorTypesController : ApiController
    {
            SensorTypesRepository sensorTypeRep = new SensorTypesRepository();//Dependecy injection

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
            public SensorType GetSensorTypeById(int id)
            {
                var sensorType = sensorTypeRep.GetSensorTypeById(id);
                return sensorType;
            }

            [Route("", Name = "GetAllSensorsTypeRoute")]
            [HttpGet]
            public List<SensorType> GetAllSensorTypes()
            {
                var sensorsType = sensorTypeRep.GetAllSensorTypes();
                return sensorsType.OrderBy(p => p.Id).ToList();
            }

            [Route("{id}", Name = "DeleteSensorTypeRoute")]
            [HttpDelete]
            public void DeleteSensorType(int id)
            {
                sensorTypeRep.DeleteSensorType(id);
            }

            [Route("{id}")]
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

