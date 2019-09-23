using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.DependencyBlocks;
using SensorsManager.Web.Api.Hubs;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;
using SensorsManager.Web.Api.Validations;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]


    public class ReadingsController : ApiControllerWithHub<ReadingsHub>
    {
        private readonly IUserRepository _userRep;
        private readonly INetworkRepository _networkRep;
        private readonly ISensorRepository _sensorRep;
        private readonly IGatewayRepository _gatewayRep;
        private readonly IGatewayConnectionRepository _connectionRep;
        private readonly ISensorReadingRepository _readingRep;
        private readonly ICredentialService _credentials;
        private readonly IThrottlerService _throttler;
        private readonly IGatewayConnectionService _connectionService;
        private readonly IVibrationFilter _vibrationFilter;
        private readonly IDateTimeService _dateTime;
        private readonly IMapper _mapper;
        private readonly IMessageService _messages;
        public ReadingsController(IReadingsControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _networkRep = dependencyBlock.NetworkRepository;
            _gatewayRep = dependencyBlock.GatewayRepository;
            _sensorRep = dependencyBlock.SensorRepository;
            _readingRep = dependencyBlock.ReadingRepository;
            _connectionRep = dependencyBlock.ConnectionRepository;
            _credentials = dependencyBlock.CredentialService;
            _throttler = dependencyBlock.ThrottlerService;
            _connectionService = dependencyBlock.ConnectionService;
            _vibrationFilter = dependencyBlock.VibrationFilter;
            _dateTime = dependencyBlock.DateTimeService;
            _mapper = dependencyBlock.Mapper;
            _messages = dependencyBlock.MessageService;
        }

        [HttpPost,Route("~/api/readings/address"),ValidateModel]
        public IHttpActionResult Add(SensorReadingModelPost sensorReadingModel)
        {
            if (sensorReadingModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            _throttler.ThrottlerSetup(sensorReadingModel.SensorAddress, 1, 3);
            if (_throttler.RequestShouldBeThrottled())
            {
                return TooManyRequests(_throttler);
            }

            var sensor = _sensorRep.GetAll()
                .Where(s => s.Address == sensorReadingModel.SensorAddress)
                .SingleOrDefault();         

            if (sensor != null)
            {
                var sensorReading = _mapper.Map<SensorReading>(sensorReadingModel);
                sensorReading.Sensor_Id = sensor.Id;
                sensorReading.InsertDate = _dateTime.GetDateOffSet(); 
                _readingRep.Add(sensorReading);

                var pending =
                         TheSensorIntervalPending
                         .GetPendingMember(sensor.Id);

                //Check if the penging exists
                if (pending != null)
                {
                    _mapper.Map(pending, sensor);
                    TheSensorIntervalPending.ClearPending(pending);
                }

                sensor.Active = true;
                sensor.LastReadingDate = sensorReading.ReadingDate;
                sensor.LastInsertDate = sensorReading.InsertDate;
                _sensorRep.Update(sensor);

                var address = sensorReadingModel.SensorAddress;
                Hub.Clients.Group(address).refreshReadings();

                var gateway = _gatewayRep.GetAll()
                    .SingleOrDefault(g => g.Address == sensorReadingModel.GatewayAddress);

                //add the gateway connections
                if (gateway != null)
                {
                    if (!_connectionRep.GetAll()
                                      .Any(
                                            c => c.Gateway_Id == gateway.Id
                                            && c.Sensor_Id == sensor.Id)
                                      )
                    {
                        var connection = _connectionService.Create(gateway.Id, sensor.Id);
                        _connectionRep.Add(connection);
                    }

                    gateway.LastSensorDate = sensorReading.ReadingDate;
                    _gatewayRep.Update(gateway);
                }

                var createdReading = _mapper.Map<SensorReadingModelGet>(sensorReading);
                return Created($"api/networks/{sensor.Network_Id}/sensors/{sensor.Id}/readings", createdReading);
            }
            else
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor", "Address");
                return NotFound(errorMessage);
            }
        }

        [SensorsManagerAuthorize]
        [HttpGet,Route("~/api/networks/{networkId:int}/sensors/{sensorId:int}/readings", Name = "GetSensorReadings")]
        public IHttpActionResult Get(int networkId, int sensorId, int page = 1, int pageSize = 30)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            var sensor = _sensorRep.Get(sensorId);

            if(sensor == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor", "Id");
                return NotFound(errorMessage);
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }
           
            //This needs to go!!!!
            var sensorReadings =
                sensor.SensorType.Name.ToLower() != "vibration" ?
                _readingRep.Get(sensor.Id)
                : _readingRep.Get(sensor.Id).Where(r => _vibrationFilter.ValidValues().Contains((int)r.Value));

            var totalCount = sensorReadings.Count();

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = sensorReadings.OrderByDescending(r => r.InsertDate)
                                        .Skip(pageSize * (page - 1))
                                        .Take(pageSize)
                                        .Select(p => _mapper.Map<SensorReadingModelGet>(p))
                                        .ToList();

            return Ok("GetSensorReadings", page, pageSize, pageCount, totalCount, results);
        }
    }
}