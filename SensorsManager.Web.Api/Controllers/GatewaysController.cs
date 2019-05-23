using SensorsManager.Web.Api.DependencyBlocks;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;
using SensorsManager.Web.Api.Validations;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SensorsManager.Web.Api.Controllers
{
    [RoutePrefix("api/networks/{networkId:int}/gateways")]
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]
    
    public class GatewaysController : BaseApiController
    {
        IUserRepository _userRep;
        INetworkRepository _networkRep;
        IGatewayRepository _gatewayRep;
        ICredentialService _credentials;
        IDateTimeService _dateTime;

        public GatewaysController(IGatewaysControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _networkRep = dependencyBlock.NetworkRepository;
            _gatewayRep = dependencyBlock.GatewayRepository;
            _credentials = dependencyBlock.CredentialService;
            _dateTime = dependencyBlock.DateTimeService;
        }

        [SensorsManagerAuthorize]
        [HttpPost, Route(""),ValidateModel]
        public IHttpActionResult Add(int networkId, GatewayModelPost gatewayModel)
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

            if (gatewayModel == null)
            {
                return BadRequest("You have sent an empty object");
            }


            var gateway = _gatewayRep.GetAll()
                .SingleOrDefault(p => p.Name == gatewayModel.Name
                || p.Address == gatewayModel.Address
                );

            if (gateway != null)
            {
                return Conflict("This gateway already exists!");
            }



            gatewayModel.Network_Id = networkId;
            gatewayModel.UploadInterval = 5;

            var newGateway = ModelToEntityMap.MapToEntity(gatewayModel);
            _gatewayRep.Add(newGateway);

            return CreatedAtRoute("GetGateway", new
            {
                networkId,
                id = newGateway.Id
            }, newGateway);
        }

        [SensorsManagerAuthorize]
        [HttpGet, Route("{id:int}", Name = "GetGateway")]
        public IHttpActionResult Get(int networkId, int id)
        {
            var network = _networkRep.Get(networkId);
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (network == null || network.User_Id != userId)
            {
                return NotFound();
            }

            var gateway = _gatewayRep.Get(id);

            if (gateway == null)
            {
                return NotFound();
            }

            var gatewayModel = ModelFactory.CreateModel(gateway);

            return Ok(gatewayModel);
        }

        [SensorsManagerAuthorize]
        [HttpGet, Route("", Name = "GetAllGateways")]
        public IHttpActionResult GetAll(int networkId, int page = 1, int pageSize = 30)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            var network = _networkRep.Get(networkId);

            if (network == null || network.User_Id != userId)
            {
                NotFound();
            }

            var query = _gatewayRep.GetAll().Where(p => p.Network_Id == networkId);
            var totalCount = query.Count();

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = query
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => ModelFactory.CreateModel(p))
                .ToList();


            return Ok("GetAllGateways", page, pageSize, pageCount, totalCount, results);
        }

        //For the physical gateway
        [HttpGet, Route("~/api/gateway/{address}")]
        public IHttpActionResult GetConnections(string address)
        {
            var gateway = _gatewayRep.GetAll()
                .SingleOrDefault(g => g.Address == address);

            if(gateway == null)
            {
                NotFound();
            }
            gateway.Active = true;
            gateway.LastSignalDate = _dateTime.GetDateTime();
            _gatewayRep.Update(gateway);

            var network = _networkRep.Get(gateway.Network_Id);
            var networkModel = ModelFactory.CreateModel(network.Address, network.Sensors);
            

            return Ok(networkModel);
            
        }

        [SensorsManagerAuthorize]
        [HttpPut, Route("{id:int}"),ValidateModel]
        public IHttpActionResult Update(int networkId, int id, GatewayModelPut gatewayModel)
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

            if (gatewayModel == null)
            {
                return BadRequest("You have sent an empty object!");
            }


            var gateway = _gatewayRep.GetAll().SingleOrDefault(
                p => p.Name == gatewayModel.Name && p.Id != id);

            if (gateway != null)
            {
                return Conflict("This gateway already exits!");
            }

            gateway = _gatewayRep.Get(id);

            if (gateway == null)
            {
                return NotFound();
            }

            ModelToEntityMap.MapToEntity(gatewayModel, gateway);
            _gatewayRep.Update(gateway);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [SensorsManagerAuthorize]
        [HttpDelete, Route("{id:int}")]
        public void Delete(int networkId, int id)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            var gateway = _gatewayRep.Get(id);
            if (gateway != null && gateway.Network_Id == networkId && gateway.Network.User_Id == userId)
            {
                _gatewayRep.Delete(id);
            }
        }
    }
}