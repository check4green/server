using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Repository.Models;
using SensorsManager.Web.Api.Security;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
       exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
       "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-SensorMeasurementCount")]
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        UserRepository userRep = new UserRepository();
        ModelFactory modelFactory = new ModelFactory();
        ModelToEntityMap modelToEntityMap = new ModelToEntityMap();

        [Route("", Name = "AddUserRoute")]
        [HttpPost]
        public IHttpActionResult AddUser(UserModel2 userModel)
        {

            if (userModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }
            if (ModelState.IsValid == false)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();

                if (error == null)
                {
                    return BadRequest();
                }

                return BadRequest(error.ErrorMessage);
            }

            var compareName = userRep.GetAllUsers()
               .Where(p =>
               p.Email == userModel.Email);
            if (compareName.Count() != 0)
            {
                return Content(HttpStatusCode.Conflict,
                    new
                    {
                        Message = String.Format("There already is a user with that email.")
                    });
            }

            var user = modelToEntityMap.MapUserModel2ToUserEntity(userModel);
            var addedUser = userRep.AddUser(user);


            return CreatedAtRoute("GetUser", new { email = addedUser.Email}, addedUser);

        }

        [SensorsManagerAuthorize]
        [Route("", Name = "GetUser")]
        [HttpGet]
        public IHttpActionResult GetUser()
        {
            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            var user = userRep.GetUser(credentials.Email, credentials.Password);
            if(user == null)
            {
                return NotFound();
            }
 
            var userModel = modelFactory.CreateUserModel(user);

            return Ok(userModel);
        }

        [SensorsManagerAuthorize]
        [Route("")]
        [HttpPut]
        public IHttpActionResult UpdateMeasurement(UserModel2 userModel)
        {
            if (userModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }
            if (ModelState.IsValid == false)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();

                if (error == null)
                {
                    return BadRequest();
                }

                return BadRequest(error.ErrorMessage);
            }
            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            var result = userRep.GetUser(credentials.Email, credentials.Password);

            if (result == null)
            {
                return NotFound();
            }

            var compareName = userRep.GetAllUsers()
              .Where(p =>
              p.Email == userModel.Email
              && p.Id != result.Id).Count();
            if (compareName != 0)
            {
                return Content(HttpStatusCode.Conflict,
                    new
                    {
                        Message = String.Format("There already is a user with that email.")
                    });
            }

            var user = modelToEntityMap.MapUserModel2ToUserEntity(userModel, result);
            userRep.UpdateMeasurement(user);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [SensorsManagerAuthorize]
        [Route("")]
        [HttpDelete]
        public void DeleteMeasurement()
        {
            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            userRep.DeleteUser(credentials.Email, credentials.Password);
        }

    }
}
