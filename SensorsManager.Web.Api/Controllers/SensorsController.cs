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
using AutoMapper;
using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Pending;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;

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
        IMapper _mapper;
        IMessageService _messages;

        public SensorsController(IUserRepository userRep, INetworkRepository networkRep, 
            ISensorTypesRepository typeRep, ISensorRepository sensorRep, IGatewayConnectionRepository connetionRep, 
            ICredentialService credentials, IDateTimeService dateTime, IMapper mapper, IMessageService messages)
        {
            _userRep = userRep;
            _networkRep = networkRep;
            _typeRep = typeRep;
            _sensorRep = sensorRep;
            _connetionRep = connetionRep;
            _credentials = credentials;
            _dateTime = dateTime;
            _mapper = mapper;
            _messages = messages;
        }

        [SensorsManagerAuthorize]
        [HttpPost,Route(""),ValidateModel]
        public IHttpActionResult Add(int networkId, SensorModelPost sensorModel)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if(!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            if (sensorModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            if (!_typeRep.Exists(sensorModel.SensorTypeId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor Type", "Id");
                return NotFound(errorMessage);
            }

            if (_sensorRep.GetAll()
                .Any(p => p.Address == sensorModel.Address))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Sensor", "Address");
                return Conflict(errorMessage);
            }

            if (_sensorRep.GetAll()
               .Any(p => p.Name == sensorModel.Name))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Sensor", "Name");
                return Conflict(errorMessage);
            }

            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            
            var sensor = _mapper.Map<Sensor>(sensorModel);
            sensor.ProductionDate = _dateTime.GetDateTime();
            sensor.Network_Id = networkId;
            _sensorRep.Add(sensor);

            var createdSensor = _mapper.Map<SensorModelGet>(sensor);

            return CreatedAtRoute("GetSensor",
                new {id = createdSensor.Id},
                createdSensor);

        }
        [SensorsManagerAuthorize]
        [HttpGet,Route("{id:int}", Name = "GetSensor")]
        public IHttpActionResult Get(int networkId, int id)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            var sensor = _sensorRep.Get(id);
            if(sensor == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor");
                return NotFound(errorMessage);
            }

            var sensorModel = _mapper.Map<SensorModelGet>(sensor);

            return Ok(sensorModel);
        }

        //for device
        [HttpGet, Route("~/api/sensors/{address}", Name = "GetSensorByAddress")]
        public IHttpActionResult Get(string address)
        {
            var sensor = _sensorRep.GetAll().SingleOrDefault(s => s.Address == address);
            if (sensor == null)
            {
                var errorMssage = _messages.GetMessage(Custom.NotFound, "Sensor");
                return NotFound(errorMssage);
            }

            var sensorModel = _mapper.Map<SensorModelGet>(sensor);

            return Ok(sensorModel);
        }

        [SensorsManagerAuthorize]
        [HttpGet, Route("", Name = "GetAllSensors")]
        public IHttpActionResult GetAll(int networkId, int page = 1, int pageSize = 30)
        {

            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
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
                .Select(p => _mapper.Map<SensorModelGet>(p))
                .ToList();


            return Ok("GetAllSensors", page, pageSize, pageCount, totalCount, results);
        }

        [SensorsManagerAuthorize]
        [HttpGet, Route("bytype/{typeId:int}", Name = "GetSensorsBySensorType")]
        public IHttpActionResult GetAll(int networkId, int typeId, int page = 1, int pageSize = 30)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            var sensorType = _typeRep.Get(typeId);

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            if (!_typeRep.GetAll().Any(st => st.Id == typeId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor Type", "Id");
                return NotFound(errorMessage);
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
                .Select(s => _mapper.Map<SensorModelGet>(s))
                .ToList();

            return Ok("GetSensorsBySensorType", page, pageSize, pageCount, totalCount, results);
        }

        [SensorsManagerAuthorize]
        [HttpPut,Route("{id:int}"),ValidateModel]
        public IHttpActionResult Update(int networkId, int id, SensorModelPut sensorModel)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            if(sensorModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            if(_sensorRep.GetAll().Any(
                    p => p.Name == sensorModel.Name && p.Id != id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Sensor", "Name");
                return Conflict(errorMessage);
            }

            var sensor = _sensorRep.Get(id);
            if (sensor == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor");
                return NotFound(errorMessage);
            }


            //Pending
            if (sensorModel.UploadInterval < sensor.UploadInterval
                && sensor.Active == true)
            {
                var lastInsertDate = sensor.LastInsertDate.Value;
                var waitTime = (uint)
                    (
                        lastInsertDate.AddMinutes(sensor.UploadInterval) - _dateTime.GetDateTime()
                    ).TotalMinutes;

                if (waitTime == 0)
                {
                    waitTime = 1;
                }
                var s = waitTime > 1 ? "s" : "";

                var pendingModel = _mapper.Map<Sensor, SensorPendingModel>(sensor,
                    opt => opt.AfterMap((dest, src) =>
                        src.UploadInterval = sensorModel.UploadInterval));

                TheSensorIntervalPending.AddToPending(pendingModel);

                _mapper.Map(sensorModel, sensor,
                    opts => opts.BeforeMap((src, dest) => {
                        src.UploadInterval = dest.UploadInterval;
                    }));
                _sensorRep.Update(sensor);
                   
                return Ok($"It will take {waitTime} minute{s} to change the upload interval");
            }

            _mapper.Map(sensorModel, sensor);
            _sensorRep.Update(sensor);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [SensorsManagerAuthorize]
        [HttpPatch, Route("{id:int}")]
        public IHttpActionResult PartialUpdate(int networkId, int id, JsonPatchDocument<SensorModelPut> patchDoc)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            if(patchDoc == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            var sensor = _sensorRep.Get(id);
            if(sensor == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor");
                return NotFound(errorMessage);
            }

            var sensorModel = _mapper.Map<SensorModelPut>(sensor);

            try
            {
                patchDoc.ApplyTo(sensorModel);
            }
            catch (JsonPatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            Validate(sensorModel);

            if (!ModelState.IsValid)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                     .Where(m => m.ErrorMessage != "")
                     .FirstOrDefault();
                var errorMessage = error != null ?
                    error.ErrorMessage : _messages.GetMessage(Generic.InvalidRequest);
                return BadRequest(errorMessage);
            }

            if (_sensorRep.GetAll().Any(
                    p => p.Name == sensorModel.Name && p.Id != id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Sensor", "Name");
                return Conflict(errorMessage);
            }


            if (sensorModel.UploadInterval < sensor.UploadInterval
              && sensor.Active == true)
            {
                var lastInsertDate = sensor.LastInsertDate.Value;
                var waitTime = (uint)
                    (
                        lastInsertDate.AddMinutes(sensor.UploadInterval) - _dateTime.GetDateTime()
                    ).TotalMinutes;

                if (waitTime == 0)
                {
                    waitTime = 1;
                }
                var s = waitTime > 1 ? "s" : "";

                var pendingModel = _mapper.Map<Sensor, SensorPendingModel>(sensor,
                    opt => opt.AfterMap((dest, src) =>
                        src.UploadInterval = sensorModel.UploadInterval));

                TheSensorIntervalPending.AddToPending(pendingModel);

                _mapper.Map(sensorModel, sensor,
                    opts => opts.BeforeMap((src, dest) => {
                        src.UploadInterval = dest.UploadInterval;
                    }));
                _sensorRep.Update(sensor);

                return Ok($"It will take {waitTime} minute{s} to change the upload interval");
            }

            _mapper.Map(sensorModel, sensor);
            _sensorRep.Update(sensor);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [SensorsManagerAuthorize]
        [HttpDelete,Route("{id:int}")]
        public IHttpActionResult Delete(int networkId, int id)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            if (_sensorRep.Exists(id))
            {
                _sensorRep.Delete(id);
                if (_connetionRep.Exists(id))
                {
                    _connetionRep.Delete(id);
                }  
            }         
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
