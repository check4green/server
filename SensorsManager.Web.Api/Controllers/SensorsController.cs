using SensorsManager.Web.Api.Repository;
using System.Linq;
using System.Net;
using System.Web.Http;
using System;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Security;
using System.Web.Http.Cors;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]
    [RoutePrefix("api/sensors")]
    [SensorsManagerAuthorize]
    public class SensorsController : BaseApiController
    {
        ISensorRepository _sensorRep;
        ISensorTypesRepository _sensorTypeRep;
        ISensorReadingRepository _readingRep;
        IUserRepository _userRep;


        public SensorsController(
            ISensorRepository sensorRep,
            ISensorTypesRepository sensorTypeRep,
            ISensorReadingRepository readingRep,
            IUserRepository userRep
            )
        {
            _sensorRep = sensorRep;
            _sensorTypeRep = sensorTypeRep;
            _readingRep = readingRep;
            _userRep = userRep;
        }


        [Route("", Name = "AddSensorRoute")]
        [HttpPost]
        public IHttpActionResult AddSensor(SensorModelPost sensorModel)
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


            if (_sensorTypeRep.GetSensorTypeById(sensorModel.SensorTypeId) == null)
            {
                return NotFound($"There is no sensor type with the id:{sensorModel.SensorTypeId}.");
            }

            if (sensorModel.GatewayAddress == sensorModel.ClientAddress)
            {
                return BadRequest("The gateway and client addresses must be distinct.");
            }

            if (_sensorRep.GetAllSensors()
                .Where(p => p.ClientAddress == sensorModel.ClientAddress).
                SingleOrDefault() != null)
            {
                return Conflict("There already is a sensor with that client address.");
            }

            if (_sensorRep.GetAllSensors()
                .Where(p => p.GatewayAddress == sensorModel.ClientAddress)
                .SingleOrDefault() != null)
            {
                return Conflict("Your client address matches an existing gateway address.");
            }

            if (_sensorRep.GetAllSensors()
                .Where(p => p.ClientAddress == sensorModel.GatewayAddress)
                .SingleOrDefault() != null)
            {
                return Conflict("Your gateway address matches an existing client address.");
            }

            if (_sensorRep.GetAllSensors()
               .Where(p => p.Name == sensorModel.Name)
               .SingleOrDefault() != null)
            {
                return Conflict("There already is a sensor with that name.");
            }

            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.GetUser(credentials.Email, credentials.Password).Id;

            var sensor = TheModelToEntityMap.MapSensorModelToSensorEntity(sensorModel, userId);
            var addedSensor = _sensorRep.AddSensor(sensor);


            return CreatedAtRoute("GetSensorByAddressRoute",
                new
                {
                    gatewayAddress = addedSensor.GatewayAddress,
                    clientAddress = addedSensor.ClientAddress
                },
                addedSensor);

        }


        [Route("address/{gatewayAddress}/{clientAddress}", Name = "GetSensorByAddressRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorByAddress(string gatewayAddress, string clientAddress)
        {

            var sensor = _sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);

            if (sensor == null)
            {
                return NotFound();
            }

            var sensorModel = TheModelFactory.CreateSensorModel(sensor);

            return Ok(sensorModel);
        }


        [Route("address/{gatewayAddress}", Name = "GetSensorsByGatewayAddressRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorsByGatewayAddress(string gatewayAddress, int page = 1, int pageSize = 30)
        {
            var query = _sensorRep.GetSensosByGatewayAddress(gatewayAddress);
            var totalCount = query.Count();

            if (totalCount == 0)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);


            var results = _sensorRep.GetSensosByGatewayAddress(gatewayAddress)
               .OrderByDescending(p => p.Id)
               .Skip(pageSize * (page - 1))
               .Take(pageSize)
               .Select(p => TheModelFactory.CreateSensorModel(p))
               .ToList();

            if (results.Count == 0)
            {
                return NotFound();
            }

            return Ok("GetSensorsByGatewayAddressRoute", page, pageSize, pageCount, totalCount, results);
        }

        [Route("", Name = "GetAllSensorsRoute")]
        [HttpGet]
        public IHttpActionResult GetAllSensors(int page = 1, int pageSize = 30)
        {
            var query = _sensorRep.GetAllSensors();
            var totalCount = query.Count();

            if (totalCount == 0)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);



            var results = _sensorRep.GetAllSensors()
                .OrderByDescending(p => p.Id)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => TheModelFactory.CreateSensorModel(p))
                .ToList();

            if (results.Count == 0)
            {
                return NotFound();
            }

            return Ok("GetAllSensorsRoute", page, pageSize, pageCount, totalCount, results);
        }

        [Route("~/api/users/sensors", Name = "GetSensorsByUser")]
        [HttpGet]
        public IHttpActionResult GetSensorsByUser(int page = 1, int pageSize = 30)
        {

            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.GetUser(credentials.Email, credentials.Password).Id;
            var query = _sensorRep.GetAllSensors().Where(s => s.UserId == userId);
            var totalCount = query.Count();

            if (totalCount == 0)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = _sensorRep
                .GetAllSensors().Where(s => s.UserId == userId)
                .OrderByDescending(s => s.Id)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => TheModelFactory.CreateSensorModel(s))
                .ToList();

            if (results.Count == 0)
            {
                return NotFound();
            }

            return Ok("GetSensorsByUser", page, pageSize, pageCount, totalCount, results);
        }

        [Route("~/api/sensor-types/{id}/sensors", Name = "GetSensorsBySensorType")]
        [HttpGet]
        public IHttpActionResult GetSensorsBySensorType(int id, int page = 1, int pageSize = 30)
        {
            var query = _sensorRep.GetAllSensors().Where(p => p.SensorTypeId == id);
            var totalCount = query.Count();

            if (totalCount == 0)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = _sensorRep.GetAllSensors().Where(s => s.SensorTypeId == id)
                .OrderByDescending(s => s.Id)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => TheModelFactory.CreateSensorModel(s))
                .ToList();

            if (results.Count == 0)
            {
                return NotFound();
            }

            return Ok("GetSensorsBySensorType", page, pageSize, pageCount, totalCount, results);
        }


        [Route("~/api/sensor-types/{id}/users/sensors", Name = "GetSensorsBySensorTypeAndUser")]
        [HttpGet]
        public IHttpActionResult GetSensorsBySensorTypeAndUser(int id, int page = 1, int pageSize = 30)
        {
            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.GetUser(credentials.Email, credentials.Password).Id;

            var query = _sensorRep.GetAllSensors().Where(p => p.SensorTypeId == id &&
                            p.UserId == userId);

            var totalCount = query.Count();

            if (totalCount == 0)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);


            var results = _sensorRep.GetAllSensors().Where(p => p.SensorTypeId == id && p.UserId == userId)
                .OrderByDescending(p => p.Id)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => TheModelFactory.CreateSensorModel(p))
                .ToList();

            if (results.Count == 0)
            {
                return NotFound();
            }

            return Ok("GetSensorsBySensorTypeAndUser", page, pageSize, pageCount, totalCount, results);

        }


        [Route("address/{gatewayAddress}/{clientAddress}")]
        [HttpPut]
        public IHttpActionResult UpdateSensorByAddress(string gatewayAddress,
            string clientAddress, SensorModelPut sensorModel)
        {

            if (sensorModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            var sensor = _sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);

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

                return BadRequest(error.ErrorMessage);
            }

            var compareName = _sensorRep.GetAllSensors()
               .Where(p =>
               p.Name == sensorModel.Name
               && !String.Equals(p.GatewayAddress + p.ClientAddress
               , gatewayAddress + clientAddress))
               .SingleOrDefault();

            if (compareName != null)
            {
                return Conflict("There already is a sensor with that name.");
            }



            if (sensorModel.UploadInterval < sensor.UploadInterval
                && sensor.Active == true)
            {

                var lastReading = _readingRep
                    .GetSensorReadingBySensorId(sensor.Id)
                    .OrderByDescending(r => r.InsertDate)
                    .FirstOrDefault();
                //Calculate the wait time
                uint waitTime = (uint)(
                   (lastReading.InsertDate
                   .AddMinutes(sensor.UploadInterval)
                   - DateTime.UtcNow
                   ).TotalMinutes);
                if (waitTime == 0)
                {
                    waitTime = 1;
                }
                var s = waitTime > 1 ? "s" : "";

                //Store the model
                var pendingModel =
                    TheModelFactory
                    .CreateSensorModel(sensor.Id, sensorModel.UploadInterval);

                TheSensorIntervalPending.AddToPending(pendingModel);

                sensorModel.UploadInterval = sensor.UploadInterval;
                TheModelToEntityMap
                          .MapSensorModelToSensorEntity(sensorModel, sensor);

                _sensorRep.UpdateSensor(sensor);

                return Ok($"It will take {waitTime} minute{s} to change the upload interval");
            }


            TheModelToEntityMap
                         .MapSensorModelToSensorEntity(sensorModel, sensor);
            _sensorRep.UpdateSensor(sensor);


            return StatusCode(HttpStatusCode.NoContent);
        }


        [SensorsManagerAuthorize]
        [Route("address/{gatewayAddress}/{clientAddress}", Name = "DeleteSensorAddressRoute")]
        [HttpDelete]
        public  void DeleteSensorByAddress(string gatewayAddress, string clientAddress)
        {

            var sensor = _sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);
            if (sensor != null)
            {
                _sensorRep.DeleteSensor(sensor.Id);
            }
        }

        //[SensorsManagerAuthorize]
        //[Route("~/api/registered-sensors/{id}")]
        //[HttpPut]
        //public IHttpActionResult RegisterSensor(int id)
        //{
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}
    }
}
