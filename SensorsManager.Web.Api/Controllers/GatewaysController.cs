using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using SensorsManager.DomainClasses;
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
        private readonly IUserRepository _userRep;
        private readonly INetworkRepository _networkRep;
        private readonly IGatewayRepository _gatewayRep;
        private readonly ICredentialService _credentials;
        private readonly IDateTimeService _dateTime;
        private readonly IMapper _mapper;
        private readonly IMessageService _messages;

        public GatewaysController(IGatewaysControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _networkRep = dependencyBlock.NetworkRepository;
            _gatewayRep = dependencyBlock.GatewayRepository;
            _credentials = dependencyBlock.CredentialService;
            _dateTime = dependencyBlock.DateTimeService;
            _mapper = dependencyBlock.Mapper;
            _messages = dependencyBlock.MessageService;
        }

        [SensorsManagerAuthorize]
        [HttpPost, Route(""),ValidateModel]
        public IHttpActionResult Add(int networkId, GatewayModelPost gatewayModel)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            if (gatewayModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            if (_gatewayRep.GetAll().Any(g => g.Name == gatewayModel.Name))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Gateway", "Name");
                return Conflict(errorMessage);
            }

            if(_gatewayRep.GetAll().Any(g => g.Address == gatewayModel.Address))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Gateway", "Address");
                return Conflict(errorMessage);
            }
            
            gatewayModel.Network_Id = networkId;
            gatewayModel.ProductionDate = _dateTime.GetDateTime();

            var newGateway = _mapper.Map<Gateway>(gatewayModel);
            _gatewayRep.Add(newGateway);

            var createdGateway = _mapper.Map<GatewayModelGet>(newGateway);

            return CreatedAtRoute("GetGateway", new
            {
                networkId,
                id = newGateway.Id
            }, createdGateway);
        }

        [SensorsManagerAuthorize]
        [HttpGet, Route("{id:int}", Name = "GetGateway")]
        public IHttpActionResult Get(int networkId, int id)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            var gateway = _gatewayRep.Get(id);

            if (gateway == null)
            {
                var errorMesssage = _messages.GetMessage(Custom.NotFound, "Gateway");
                return NotFound(errorMesssage);
            }

            var gatewayModel = _mapper.Map<GatewayModelGet>(gateway);

            return Ok(gatewayModel);
        }

        [SensorsManagerAuthorize]
        [HttpGet, Route("", Name = "GetAllGateways")]
        public IHttpActionResult GetAll(int networkId, int page = 1, int pageSize = 30)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                NotFound(errorMessage);
            }

            var query = _gatewayRep.GetAll().Where(p => p.Network_Id == networkId);
            var totalCount = query.Count();

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = query
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => _mapper.Map<GatewayModelGet>(p))
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
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Gateway");
                return NotFound(errorMessage);
            }

            gateway.Active = true;
            gateway.LastSignalDate = _dateTime.GetDateTime();
            _gatewayRep.Update(gateway);

            var network = _networkRep.Get(gateway.Network_Id);
            var networkModel = _mapper.Map<NetworkWithSensorsModel>(network);
           
            return Ok(networkModel);    
        }

        [SensorsManagerAuthorize]
        [HttpPut, Route("{id:int}"),ValidateModel]
        public IHttpActionResult Update(int networkId, int id, GatewayModelPut gatewayModel)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            if (gatewayModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            if (_gatewayRep.GetAll().Any(
                g => g.Name == gatewayModel.Name && g.Id != id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Gateway", "Name");
                return Conflict(errorMessage);
            }

            var gateway = _gatewayRep.Get(id);

            if (gateway == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Gateway");
                return NotFound(errorMessage);
            }

            _mapper.Map(gatewayModel, gateway);
            _gatewayRep.Update(gateway);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [SensorsManagerAuthorize]
        [HttpPatch, Route("{id:int}"), ValidateModel]
        public IHttpActionResult PartialUpdate(int networkId, int id, JsonPatchDocument<GatewayModelPut> patchDoc)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            if (patchDoc == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            var gateway = _gatewayRep.Get(id);
            if(gateway == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Gateway");
                return NotFound(errorMessage);
            }

            var gatewayModel = _mapper.Map<GatewayModelPut>(gateway);
            try
            {
                patchDoc.ApplyTo(gatewayModel);
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

            Validate(gatewayModel);

            if (!ModelState.IsValid)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();
                var errorMessage = error != null ?
                    error.ErrorMessage : _messages.GetMessage(Generic.InvalidRequest);
                return BadRequest(errorMessage);
            }

            if (_gatewayRep.GetAll().Any(
                g => g.Name == gatewayModel.Name && g.Id != id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Gateway", "Name");
                return Conflict(errorMessage);
            }

            _mapper.Map(gatewayModel, gateway);
            _gatewayRep.Update(gateway);


            return StatusCode(HttpStatusCode.NoContent);
        }

        [SensorsManagerAuthorize]
        [HttpDelete, Route("{id:int}")]
        public IHttpActionResult Delete(int networkId, int id)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            if (!_networkRep.GetAll().Any(n => n.Id == networkId && n.User_Id == userId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network", "Id");
                return NotFound(errorMessage);
            }

            if (_gatewayRep.Exists(id))
            {
                _gatewayRep.Delete(id);
            }
            
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}