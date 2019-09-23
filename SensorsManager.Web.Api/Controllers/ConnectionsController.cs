using AutoMapper;
using SensorsManager.Web.Api.DependencyBlocks;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
    exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
    "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
    "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]
    [SensorsManagerAuthorize]
    public class ConnectionsController : BaseApiController
    {
        private readonly IUserRepository _userRep;
        private readonly INetworkRepository _networkRep;
        private readonly IGatewayRepository _gatewayRep;
        private readonly ISensorRepository _sensorRep;
        private readonly IGatewayConnectionRepository _connectionRep;
        private readonly ICredentialService _credentials;
        private readonly IMapper _mapper;
        private readonly IMessageService _messages;

        public ConnectionsController(IConnectionsControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _networkRep = dependencyBlock.NetworkRepository;
            _gatewayRep = dependencyBlock.GatewayRepository;
            _sensorRep = dependencyBlock.SensorRepository;
            _connectionRep = dependencyBlock.ConnectionRepository;
            _credentials = dependencyBlock.CredentialService;
            _mapper = dependencyBlock.Mapper;
            _messages = dependencyBlock.MessageService;
        }

        [HttpGet,Route("api/networks/{networkId:int}/gateways/{gatewayId:int}/connections",Name = "GetGatewayConnections")]
        public IHttpActionResult GetGatewayConnections(int networkId, int gatewayId, int page = 1, int pageSize = 30)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            var gateway = _gatewayRep.Get(gatewayId);

            if(gateway == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Gateway", "Id");
                return NotFound(errorMessage);
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

           
            var totalCount = gateway.Connections.Count();

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);


            var results = _sensorRep.GetAll()
                .Join(gateway.Connections, s => s.Id, c => c.Sensor_Id, (s,c) => s)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => _mapper.Map<SensorModelGet>(p))
                .ToList();


            return Ok("GetGatewayConnections", page, pageSize, pageCount, totalCount, results);
        }   

        [HttpGet, Route("api/networks/{networkId:int}/sensors/{sensorId:int}/connections", Name = "GetSensorConnections")]
        public IHttpActionResult GetSensorConnections(int networkId, int sensorId, int page = 1, int pageSize = 30)
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

            var connections = _connectionRep.GetAll()
                .Where(c => c.Sensor_Id == sensorId);

            var totalCount = connections.Count();
            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = _gatewayRep.GetAll()
                .Join(connections, g => g.Id, c => c.Gateway_Id, (g,c) => g)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => _mapper.Map<GatewayModelGet>(p))
                .ToList();


            return Ok("GetGatewayConnections", page, pageSize, pageCount, totalCount, results);
        }
    }
}
