
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.ServiceInterfaces;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/users")]
    public class UsersController : BaseApiController
    {
        IUserRepository _userRep;
        IMemCacheService _memCache;
        IMailSenderService _mailSender;
        IRandomService _random;
        IDateTimeService _dateTime;
        ICredentialService _credentials;

        public UsersController(
             IUserRepository userRep,
             IMemCacheService memCache,
             IMailSenderService mailSender,
             IRandomService random,
             IDateTimeService dateTime,
             ICredentialService credentials
             )
        {
            _userRep = userRep;
            _memCache = memCache;
            _mailSender = mailSender;
            _random = random;
            _dateTime = dateTime;
            _credentials = credentials;
        }

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

            var checkedUser = _userRep.GetAllUsers()
               .Where(u =>
               u.Email == userModel.Email)
               .SingleOrDefault();

            if (checkedUser != null)
            {
                return Conflict("There already is a user with that email.");
            }

            var email = _memCache.Get(userModel.Email);
            if (email == null)
            {
                return Unauthorized("Your email has not been validated!");
            }


            var user = TheModelToEntityMap.MapUserModelToUserEntity(userModel);
            _userRep.AddUser(user);

            _memCache.Remove(user.Email);

            return CreatedAtRoute("GetUser", new { email = user.Email }, user);
        }

        [Route("~/api/demoRequest")]
        [HttpPost]
        public IHttpActionResult RequestDemo(DemoRequestModel requestModel)
        {
            if (requestModel == null)
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

            try
            {
                
                string body = $"Name: {requestModel.FullName}\n" +
                    $"Buisness email address: {requestModel.Email}\n" +
                    $"Company: {requestModel.Company}\n";

                if (requestModel.JobTitle != null)
                {
                    body += $"Job title: {requestModel.JobTitle}\n";
                }
                if (requestModel.CompanySize != null)
                {
                    body += $"Company size: {requestModel.CompanySize}\n";
                }
                if (requestModel.PhoneNumber != null)
                {
                    body += $"Phone number: {requestModel.PhoneNumber}\n";
                }
                if (requestModel.Message != null)
                {
                    body += $"Message: {requestModel.Message}";
                }

                _mailSender.SendMail("info@check4green.com", "Request demo", body);
                return Ok("Mail has been sent.");
            }
            catch (Exception e)
            {
                return ExpectationFailed(e.Message);
            }
        }

        [Route("~/api/contact")]
        [HttpPost]
        public IHttpActionResult SendContactInfo(ContactInfoModel contactInfoModel)
        {
            if (contactInfoModel == null)
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
            try
            {
                
                string body = $"Name: {contactInfoModel.FullName}\n" +
                    $"Email address: {contactInfoModel.Email}\n" +
                    $"Phone: {contactInfoModel.Phone}\n" +
                    $"Message: {contactInfoModel.Message}";

                _mailSender.SendMail("info@check4green.com", "Contact info", body);
                return Ok("Mail has been sent.");
            }
            catch (Exception e)
            {
                return ExpectationFailed(e.Message);
            }
        }

        [SensorsManagerAuthorize]
        [Route("", Name = "GetUser")]
        [HttpGet]
        public IHttpActionResult GetUser()
        {
             _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var user = _userRep.GetUser(_credentials.Email, _credentials.Password);
            if (user == null)
            {
                return NotFound();
            }

            var userModel = TheModelFactory.CreateUserModel(user);

            return Ok(userModel);
        }

        [SensorsManagerAuthorize]
        [Route("logIn", Name = "LogIn")]
        [HttpPost]
        public IHttpActionResult LogIn()
        {
            return Ok("Authorization succeded!");
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
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);//<---Remove

            var user = _userRep.GetUser(_credentials.Email, _credentials.Password);

            if (user == null)
            {
                return NotFound();
            }

            var compareName = _userRep.GetAllUsers()
              .Where(u =>
              u.Email == userModel.Email
              && u.Id != user.Id).Count();
            if (compareName != 0)
            {
                return Conflict("There already is a user with that email.");
            }

            TheModelToEntityMap.MapUserModelToUserEntity(userModel, user);
            _userRep.UpdateUser(user);

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
            var user = _userRep.GetAllUsers().Where(u => u.Email == userModel.Email).SingleOrDefault();
            if (user == null)
            {
                return NotFound($"There is no user with the email {userModel.Email}.");
            }
            try
            {
                
                var code = _random.Next(1000, 9999).ToString();
                _memCache.Add(code, code, _dateTime.GetDateOffSet().AddMinutes(5));

                _mailSender.SendMail(userModel.Email, "Reset password",
                    $"Here is your password reset code: {code}");
                return Ok("Check your mail, you have been sent a reset code.");
            }
            catch
            {
                return ExpectationFailed("Failed to send mail. Please try again.");
            }
        }

        [Route("resetPassword")]
        [HttpPut]
        public IHttpActionResult ResetPassword(UserModel_Code userModel)
        {
            var code = _memCache.Get(userModel.Code);
            if (code == null)
            {
                return Unauthorized("Invalid reset code!");
            }
            var user = _userRep.GetAllUsers()
                .Where(u => u.Email == userModel.Email).SingleOrDefault();
            if (user == null)
            {
                return NotFound($"There is no user with the email {userModel.Email}");
            }
            user.Password = userModel.Password;
            _userRep.UpdateUser(user);
            _memCache.Remove(code.ToString());
            return Ok("Password has been reset.");
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
            var user = _userRep.GetAllUsers().Where(u => u.Email == userModel.Email).SingleOrDefault();
            if (user != null)
            {
                return Conflict($"There already exists an user with the mail {userModel.Email}.");
            }

            try
            {
                var code = _random.Next(1000, 9999).ToString();
                _memCache.Add(code, code, _dateTime.GetDateOffSet().AddMinutes(5));
                
                _mailSender.SendMail(userModel.Email, "Validation code",
                    $"Here is your validation code: {code}");
                return Ok("Check your mail, you have been sent a validation code.");
            }
            catch
            {
                return ExpectationFailed("Failed to send mail. Please try again.");
            }
        }

        [Route("validation")]
        [HttpPut]
        public IHttpActionResult ValidateUser(UserModel_Validation userModel)
        {
            var code = _memCache.Get(userModel.Code);
            if (code == null)
            {
                return Unauthorized("Invalid validation code!");
            }

            _memCache.Add(userModel.Email, userModel.Email, DateTimeOffset.UtcNow.AddMinutes(10));
            _memCache.Remove(code.ToString());
            return Ok("Your email has been validated, you have 10 minutes to create your account.");
        }

        [SensorsManagerAuthorize]
        [Route("")]
        [HttpDelete]
        public void DeleteUser()
        {
             _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            _userRep.DeleteUser(_credentials.Email, _credentials.Password);
        }

    }
}
