using SensorsManager.Web.Api.Repository;
using System.Linq;
using System.Web.Http;
using System;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Security;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Services;
using System.Net;
using SensorsManager.Web.Api.Validations;
using SensorsManager.Web.Api.DependencyBlocks;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]
    [RoutePrefix("api/networks/{networkId:int}/sensors")]
   
    public class SensorsController : BaseApiController
    {
        IUserRepository _userRep;
        INetworkRepository _networkRep;
        ISensorTypesRepository _typeRep;
        ISensorRepository _sensorRep;
        IGatewayConnectionRepository _connetionRep;
        ICredentialService _credentials;
        IDateTimeService _dateTime;


        public SensorsController(ISensorsControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _networkRep = dependencyBlock.NetworkRepository;
            _typeRep = dependencyBlock.TypesRepository;
            _sensorRep = dependencyBlock.SensorRepository;
            _connetionRep = dependencyBlock.ConnectionRepository;
            _credentials = dependencyBlock.CredentialService;
            _dateTime = dependencyBlock.DateTimeService;
        }

        [SensorsManagerAuthorize]
        [HttpPost,Route(""),ValidateModel]
        public IHttpActionResult Add(int networkId, SensorModelPost sensorModel)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            var network = _networkRep.Get(networkId);
            if(network == null || network.User_Id != userId)
            {
                return NotFound();
            }

            if (sensorModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            if (_typeRep.Get(sensorModel.SensorTypeId) == null)
            {
                return NotFound($"There is no sensor type with the id:{sensorModel.SensorTypeId}.");
            }

            if (_sensorRep.GetAll()
                .Where(p => p.Address == sensorModel.Address).
                SingleOrDefault() != null)
            {
                return Conflict("There already is a sensor with that address.");
            }

            if (_sensorRep.GetAll()
               .Where(p => p.Name == sensorModel.Name)
               .SingleOrDefault() != null)
            {
                return Conflict("There already is a sensor with that name.");
            }

            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            sensorModel.ProductionDate = _dateTime.GetDateTime();
            sensorModel.NetworkId = networkId;
            var sensor = ModelToEntityMap.MapToEntity(sensorModel);
            _sensorRep.Add(sensor);


            return CreatedAtRoute("GetSensor",
                new {id = sensor.Id},
                sensor);

        }
        [SensorsManagerAuthorize]
        [HttpGet,Route("{id:int}", Name = "GetSensor")]
        public IHttpActionResult Get(int networkId, int id)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            var network = _networkRep.Get(networkId);
            if (network == null || network.User_Id != userId)
            {
                return NotFound();
            }

            var sensor = _sensorRep.Get(id);
            if(sensor == null)
            {
                return NotFound();
            }

            var sensorModel = ModelFactory.CreateModel(sensor);

            return Ok(sensorModel);
        }

        //for device
        [HttpGet, Route("~/api/sensors/{address}", Name = "GetSensorByAddress")]
        public IHttpActionResult Get(string address)
        {
            var sensor = _sensorRep.GetAll().SingleOrDefault(s => s.Address == address);
            if (sensor == null)
            {
                return NotFound();
            }

            var sensorModel = ModelFactory.CreateModel(sensor);

            return Ok(sensorModel);
        }

        [SensorsManagerAuthorize]
        [HttpGet,Route("", Name = "GetAllSensors")]
        public IHttpActionResult GetAll(int networkId,int page = 1, int pageSize = 30)
        {

            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            var network = _networkRep.Get(networkId);
            if (network == null || network.User_Id != userId)
            {
                return NotFound();
            }

            var query = _sensorRep.GetAll().Where(s => s.Network_Id == networkId);
            var totalCount = query.Count();

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = query
                .OrderByDescending(p => p.Id)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => ModelFactory.CreateModel(p))
                .ToList();


            return Ok("GetAllSensors", page, pageSize, pageCount, totalCount, results);
        }

        [SensorsManagerAuthorize]
        [HttpGet,Route("", Name = "GetSensorsBySensorType")]
        public IHttpActionResult GetAll(int networkId,int typeId, int page = 1, int pageSize = 30)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            var network = _networkRep.Get(networkId);
            var sensorType = _typeRep.Get(typeId);
            if (network == null || network.User_Id != userId || sensorType == null)
            {
                return NotFound();
            }

            var query = _sensorRep.GetAll()
                .Where(p => p.SensorType_Id == typeId && p.Network_Id == networkId);
            var totalCount = query.Count();

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = _sensorRep.GetAll()
                .Where(s => s.SensorType_Id == typeId && s.Network_Id == networkId)
                .OrderByDescending(s => s.Id)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => ModelFactory.CreateModel(s))
                .ToList();

            return Ok("GetSensorsBySensorType", page, pageSize, pageCount, totalCount, results);
        }

        [SensorsManagerAuthorize]
        [HttpPut,HttpPatch,Route("{id:int}"),ValidateModel]
        public IHttpActionResult Update(int networkId, int id, SensorModelPut sensorModel)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            var network = _networkRep.GetAll()
                .SingleOrDefault(p => p.Id == networkId
                && p.User_Id == userId);

            if (network == null)
            {
                return NotFound();
            }

            if(sensorModel == null)
            {
                return BadRequest("You have sent an empty object!");
            }


            var sensor = _sensorRep.GetAll().SingleOrDefault(
                    p => p.Name == sensorModel.Name && p.Id != id
                );

            if(sensor != null)
            {
                return Conflict("Sensor already exists");
            }

            sensor = _sensorRep.Get(id);
            if (sensor == null)
            {
                return NotFound();
            }

            ModelToEntityMap.MapToEntity(sensorModel, sensor);
            _sensorRep.Update(sensor);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [SensorsManagerAuthorize]
        [HttpDelete,Route("{id:int}")]
        public void Delete(int networkId, int id)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            var sensor = _sensorRep.Get(id);
            if (sensor != null && sensor.Network_Id == networkId && sensor.Network.User_Id == userId)
            {
                _sensorRep.Delete(id);
                _connetionRep.Delete(id);
            }
        }
    }
}
