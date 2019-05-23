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
        IUserRepository _userRep;
        INetworkRepository _networkRep;
        ICredentialService _credentials;
        IGuidService _guid;

        public NetworksController(INetworksControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _networkRep = dependencyBlock.NetworkRepository;
            _credentials = dependencyBlock.CredentialService;
            _guid = dependencyBlock.GuidService;
        }

        [HttpPost, Route(""),ValidateModel]
        public IHttpActionResult Add(NetworkModelPost networkModel)
        {
            if (networkModel == null)
            {
                return BadRequest("You have sent an empty object");
            }

            var network = _networkRep.GetAll()
                .SingleOrDefault(p => p.Name == networkModel.Name);

            if (network != null)
            {
                return Conflict("This network already exists");
            }

            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;

            networkModel.User_Id = userId;
            networkModel.Address = _guid.GetAddress();

            var newNetwork = ModelToEntityMap
                .MapToEntity(networkModel);

            _networkRep.Add(newNetwork);
            return CreatedAtRoute("GetNetwork",
                new { id = newNetwork.Id }, newNetwork);


        }

        [HttpGet, Route("{id:int}", Name = "GetNetwork")]
        public IHttpActionResult Get(int id)
        {
            var network = _networkRep.Get(id);

            if (network == null)
            {
                return NotFound();
            }

            var networkModel = ModelFactory.CreateModel(network);

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
                .Select(p => ModelFactory.CreateModel(p))
                .ToList();


            return Ok("GetAllNetworks", page, pageSize, pageCount, totalCount, results);

        }

        [HttpPut, Route("{id:int}"),ValidateModel]
        public IHttpActionResult Update(int id, NetworkModelPut networkModel)
        {
            if (networkModel == null)
            {
                return BadRequest("You have sent an empty object");
            }

            var network = _networkRep.GetAll()
                .SingleOrDefault(p => p.Name == networkModel.Name && p.Id != id);

            if (network != null)
            {
                return Conflict("This network already exists");
            }

            network = _networkRep.Get(id);
            if (network == null)
            {
                return NotFound();
            }

            ModelToEntityMap.MapToEntity(networkModel, network);
            _networkRep.Update(network);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete, Route("{id:int}")]
        public void Delete(int id)
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var userId = _userRep.Get(_credentials.Email, _credentials.Password).Id;
            var network = _networkRep.Get(id);
            if (network != null && network.User_Id == userId)
            {
                _networkRep.Delete(network.Id);
            }
        }
    }
}
