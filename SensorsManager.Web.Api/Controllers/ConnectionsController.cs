using SensorsManager.Web.Api.DependencyBlocks;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
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
        IUserRepository _userRep;
        INetworkRepository _networkRep;
        IGatewayRepository _gatewayRep;
        ISensorRepository _sensorRep;
        IGatewayConnectionRepository _connectionRep;
        ICredentialService _credentials;

        public ConnectionsController(IConnectionsControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _networkRep = dependencyBlock.NetworkRepository;
            _gatewayRep = dependencyBlock.GatewayRepository;
            _sensorRep = dependencyBlock.SensorRepository;
            _connectionRep = dependencyBlock.ConnectionRepository;
            _credentials = dependencyBlock.CredentialService;
        }

        [HttpGet,Route("api/networks/{networkId:int}/gateways/{gatewayId:int}/connections",Name = "GetGatewayConnections")]
        public IHttpActionResult GetGatewayConnections(int networkId, int gatewayId, int page = 1, int pageSize = 30)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            var network = _networkRep.Get(networkId);
            if (network == null || network.User_Id != userId)
            {
                return NotFound();
            }

            var gateway = _gatewayRep.Get(gatewayId);

            if(gateway == null)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

           
            var totalCount = gateway.Connections.Count();

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);


            var results = _sensorRep.GetAll()
                .Join(gateway.Connections, s => s.Id, c => c.Sensor_Id, (s,c) => s)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => ModelFactory.CreateModel(p))
                .ToList();


            return Ok("GetGatewayConnections", page, pageSize, pageCount, totalCount, results);
        }   

        [HttpGet, Route("api/networks/{networkId:int}/sensors/{sensorId:int}/connections", Name = "GetSensorConnections")]
        public IHttpActionResult GetSensorConnections(int networkId, int sensorId, int page = 1, int pageSize = 30)
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

            var connections = _connectionRep.GetAll()
                .Where(c => c.Sensor_Id == sensorId);

            var totalCount = connections.Count();
            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = _gatewayRep.GetAll()
                .Join(connections, g => g.Id, c => c.Gateway_Id, (g,c) => g)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => ModelFactory.CreateModel(p))
                .ToList();


            return Ok("GetGatewayConnections", page, pageSize, pageCount, totalCount, results);
        }
    }
}
