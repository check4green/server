using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
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
        IUserRepository _userRep;
        INetworkRepository _networkRep;
        ISensorRepository _sensorRep;
        IGatewayRepository _gatewayRep;
        IGatewayConnectionRepository _connectionRep;
        ISensorReadingRepository _readingRep;
        ICredentialService _credentials;
        IThrottlerService _throttler;
        IGatewayConnectionService _connectionService;


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
        }

        [HttpPost,Route("~/api/readings/address"),ValidateModel]
        public IHttpActionResult Add(SensorReadingModelPost sensorReadingModel)
        {
            if (sensorReadingModel == null)
            {
                return BadRequest("You have sent an empty object");
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
                var sensorReading = ModelToEntityMap
                    .MapToEntity(sensorReadingModel, sensor.Id);

                var reading = _readingRep.Add(sensorReading);

                var pending =
                         TheSensorIntervalPending
                         .GetPendingMember(sensor.Id);

                //Check if the penging exists
                if (pending != null)
                {
                    ModelToEntityMap
                       .MapToEntity(pending, sensor);
                    TheSensorIntervalPending.ClearPending(pending);
                }

                sensor.Active = true;
                sensor.LastReadingDate = reading.ReadingDate;
                sensor.LastInsertDate = reading.InsertDate;
                _sensorRep.Update(sensor);

                var address = sensorReadingModel.SensorAddress;
                Hub.Clients.Group(address).refreshReadings();

                var gateway = _gatewayRep.GetAll()
                    .SingleOrDefault(g => g.Address == sensorReadingModel.GatewayAddress);

                if (gateway != null)
                {
                    var oldConnection = _connectionRep.GetAll()
                                      .SingleOrDefault(
                        c => c.Gateway_Id == gateway.Id
                        && c.Sensor_Id == sensor.Id);

                    if (oldConnection == null)
                    {
                        var connection = _connectionService.Create(gateway.Id, sensor.Id);
                        _connectionRep.Add(connection);
                    }

                    gateway.LastSensorDate = reading.ReadingDate;
                    _gatewayRep.Update(gateway);
                }


                return Created($"api/networks/{sensor.Network_Id}/sensors/{sensor.Id}/readings", sensorReading);
            }
            else
            {
                return NotFound();
            }


        }

        [SensorsManagerAuthorize]
        [HttpGet,Route("~/api/networks/{networkId:int}/sensors/{sensorId:int}/readings", Name = "GetSensorReadings")]
        public IHttpActionResult Get(int networkId, int sensorId, int page = 1, int pageSize = 30)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            var network = _networkRep.Get(networkId);
            if (network == null || network.User_Id != userId)
            {
                return NotFound();
            }

            var sensor = _sensorRep.Get(sensorId);

            if(sensor == null)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensorReadings =
                sensor.SensorType.Name != "vibration" ?
                _readingRep.Get(sensor.Id)
                : _readingRep.Get(sensor.Id).Where(r => r.Value != 0);

            var totalCount = sensorReadings.Count();

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = sensorReadings.OrderByDescending(r => r.InsertDate)
                                        .Skip(pageSize * (page - 1))
                                        .Take(pageSize)
                                        .Select(p => ModelFactory.CreateModel(p))
                                        .ToList();

            return Ok("GetSensorReadings", page, pageSize, pageCount, totalCount, results);

        }
    }
}