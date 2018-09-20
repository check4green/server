using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Repository.Models;
using SensorsManager.Web.Api.Security;
using System;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
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
        IModelFactory modelFactory = new ModelFactory();
        IModelToEntityMap modelToEntityMap = new ModelToEntityMap();
        MemoryCache memCache = MemoryCache.Default;

        [Route("", Name = "AddUserRoute")]
        [HttpPost]
        public IHttpActionResult AddUser(UserModel userModel)
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

            var email = memCache.Get(userModel.Email);
            if (email == null)
            {
                return Content(HttpStatusCode.Unauthorized,
                    new { Message = "Your email has not been validated!" });
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

            var user = modelToEntityMap.MapUserModelToUserEntity(userModel);
            var addedUser = userRep.AddUser(user);

            memCache.Remove(user.Email);

            return CreatedAtRoute("GetUser", new { email = addedUser.Email }, addedUser);
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
        [Route("logIn", Name = "LogIn")]
        [HttpGet]
        public IHttpActionResult LogIn()
        {
            return Ok(new { Message = "Authorization succeded!" });
        }

        [SensorsManagerAuthorize]
        [Route("")]
        [HttpPut]
        public IHttpActionResult UpdateUser(UserModel userModel)
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

            var user = modelToEntityMap.MapUserModelToUserEntity(userModel, result);
            userRep.UpdateUser(user);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("getResetCode")]
        [HttpPut]
        public IHttpActionResult SendResetPasswordCode(UserModel_Email userModel)
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
            var user = userRep.GetAllUsers().Where(u => u.Email == userModel.Email).SingleOrDefault();
            if (user == null)
            {
                return Content(HttpStatusCode.NotFound,
                    new { Message = $"There is no user with the email {userModel.Email} ." });
            }
            try
            {
                Random random = new Random();
                var code = random.Next(1000, 9999).ToString();
                memCache.Add(code, code, DateTimeOffset.UtcNow.AddMinutes(5));
                var mailSender = new MailSender();
                mailSender.SendMail(userModel.Email, "Reset password",
                    $"Here is your password reset code: {code}");
                return Ok(new { Message = "Check your mail, you have been sent a reset code." });
            }
            catch
            {
                return Content(HttpStatusCode.ExpectationFailed,
                    new { Message = "Failed to send mail. Please try again." });
            }
        }

        [Route("resetPassword")]
        [HttpPut]
        public IHttpActionResult ResetPassowrd(UserModel_Code userModel)
        {
            var code = memCache.Get(userModel.Code);
            if(code == null)
            {
                return Content(HttpStatusCode.Unauthorized,
                    new { Message = "Invalid reset code!" });
            }
            var user = userRep.GetAllUsers()
                .Where(u => u.Email == userModel.Email).SingleOrDefault();
            if(user == null)
            {
                return Content(HttpStatusCode.NotFound,
                    new { Message = $"There is no user with the email {userModel.Email}" });
            }
            user.Password = userModel.Password;
            userRep.UpdateUser(user);
            memCache.Remove(code.ToString());
            return Ok(new { Message = "Password has been reset." });
        }

        [Route("getValidationCode")]
        [HttpPut]
        public IHttpActionResult SendValidationCode(UserModel_Email userModel)
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
            var user = userRep.GetAllUsers().Where(u => u.Email == userModel.Email).SingleOrDefault();
            if (user != null)
            {
                return Content(HttpStatusCode.Conflict,
                    new { Message = $"There already exists an user with the mail {userModel.Email}." });
            }
   
            try
            {
                Random random = new Random();
                var code = random.Next(1000, 9999).ToString();
                memCache.Add(code, code, DateTimeOffset.UtcNow.AddMinutes(5));
                var mailSender = new MailSender();
                mailSender.SendMail(userModel.Email, "Validation code",
                    $"Here is your validation code: {code}");
                return Ok(new { Message = "Check your mail, you have been sent a validation code." });
            }
            catch
            {
                return Content(HttpStatusCode.ExpectationFailed,
                    new { Message = "Failed to send mail. Please try again." });
            }
        }

        [Route("validation")]
        [HttpPut]
        public IHttpActionResult ValidateUser(UserModel_Validation userModel)
        {
            var code = memCache.Get(userModel.Code);
            if (code == null)
            {
                return Content(HttpStatusCode.Unauthorized,
                    new { Message = "Invalid validation code!" });
            }

            memCache.Add(userModel.Email, userModel.Email, DateTimeOffset.UtcNow.AddMinutes(10));
            memCache.Remove(code.ToString());
            return Ok(new { Message = "Your email has been validated, you have 10 minutes to create your account." });
        }



        [SensorsManagerAuthorize]
        [Route("")]
        [HttpDelete]
        public void DeleteUser()
        {
            var credentials = new Credentials(Request.Headers.Authorization.Parameter);
            userRep.DeleteUser(credentials.Email, credentials.Password);
        }

    }
}
