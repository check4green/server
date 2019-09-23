using AutoMapper;
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
    [RoutePrefix("api/networks")]
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]
    [SensorsManagerAuthorize]
    public class NetworksController : BaseApiController
    {
        private readonly IUserRepository _userRep;
        private readonly INetworkRepository _networkRep;
        private readonly ICredentialService _credentials;
        private readonly IGuidService _guid;
        private readonly IDateTimeService _dateTime;
        private readonly IMapper _mapper;
        private readonly IMessageService _messages;

        public NetworksController(INetworksControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _networkRep = dependencyBlock.NetworkRepository;
            _credentials = dependencyBlock.CredentialService;
            _guid = dependencyBlock.GuidService;
            _dateTime = dependencyBlock.DateTimeService;
            _mapper = dependencyBlock.Mapper;
            _messages = dependencyBlock.MessageService;
        }

        [HttpPost, Route(""), ValidateModel]
        public IHttpActionResult Add(NetworkModelPost networkModel)
        {
            if (networkModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            if (_networkRep.GetAll()
                .Any(p => p.Name == networkModel.Name))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Network", "Name");
                return Conflict(errorMessage);
            }

            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            networkModel.User_Id = userId;
            networkModel.Address = _guid.GetAddress();
            networkModel.ProductionDate = _dateTime.GetDateTime();

            var newNetwork = _mapper.Map<Network>(networkModel);
            _networkRep.Add(newNetwork);

            var createdNetwork = _mapper.Map<NetworkModelGet>(newNetwork);
            return CreatedAtRoute("GetNetwork",
                new { id = newNetwork.Id }, createdNetwork);
        }

        [HttpGet, Route("{id:int}", Name = "GetNetwork")]
        public IHttpActionResult Get(int id)
        {
            var network = _networkRep.Get(id);

            if (network == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network");
                return NotFound(errorMessage);
            }

            var networkModel = _mapper.Map<NetworkModelGet>(network);

            return Ok(networkModel);
        }

        [HttpGet, Route("", Name = "GetAllNetworks")]
        public IHttpActionResult GetAll(int page = 1, int pageSize = 30)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            var query = _networkRep.GetAll().Where(p => p.User_Id == userId);
            var totalCount = query.Count();

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = query
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(p => _mapper.Map<NetworkModelGet>(p))
                .ToList();


            return Ok("GetAllNetworks", page, pageSize, pageCount, totalCount, results);

        }

        [HttpPut, Route("{id:int}"),ValidateModel]
        public IHttpActionResult Update(int id, NetworkModelPut networkModel)
        {
            if (networkModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            if (_networkRep.GetAll()
                .Any(p => p.Name == networkModel.Name && p.Id != id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Network", "Name");
                return Conflict(errorMessage);
            }

            var network = _networkRep.Get(id);
            if (network == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Network");
                return NotFound(errorMessage);
            }

            _mapper.Map(networkModel, network);
            _networkRep.Update(network);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete, Route("{id:int}")]
        public void Delete(int id)
        {
            if (_networkRep.Exists(id))
            {
                _networkRep.Delete(id);
            }        
        }
    }
}
