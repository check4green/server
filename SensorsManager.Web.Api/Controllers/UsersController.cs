
using SensorsManager.Web.Api.DependencyBlocks;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.Services;
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
        ICredentialService _credentials;
        IDateTimeService _dateTime;
        IMemCacheService _memCache;
        IRandomService _random;
        IMailSenderService _mailSender;


        public UsersController(IUsersControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _credentials = dependencyBlock.CredentialService;
            _dateTime = dependencyBlock.DateTimeService;
            _memCache = dependencyBlock.MemCacheService;
            _random = dependencyBlock.RandomService;
            _mailSender = dependencyBlock.MailSenderService;
        }

        [HttpPost,Route("")]
        public IHttpActionResult Add(UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            var checkedUser = _userRep.Get(userModel.Email, userModel.Password);


            if (checkedUser != null)
            {
                return Conflict("There already is a user with that email.");
            }

            var email = _memCache.Get(userModel.Email);
            if (email == null)
            {
                return Unauthorized("Your email has not been validated!");
            }


            var user = ModelToEntityMap.MapToEntity(userModel);
            _userRep.Add(user);

            _memCache.Remove(user.Email);

            return CreatedAtRoute("GetUser", new { email = user.Email }, user);
        }

        [HttpPost,Route("~/api/demoRequest")]
        public IHttpActionResult RequestDemo(DemoRequestModel requestModel)
        {
            if (requestModel == null)
            {
                return BadRequest("You have sent an empty object.");
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

        [HttpPost,Route("~/api/contact")]
        public IHttpActionResult SendContactInfo(ContactInfoModel contactInfoModel)
        {
            if (contactInfoModel == null)
            {
                return BadRequest("You have sent an empty object.");
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
        [HttpGet,Route("", Name = "GetUser")]
        public IHttpActionResult Get()
        {
            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);
            var user = _userRep.Get(_credentials.Email, _credentials.Password);
            if (user == null)
            {
                return NotFound();
            }

            var userModel = ModelFactory.CreateModel(user);

            return Ok(userModel);
        }

        [SensorsManagerAuthorize]
        [HttpPost,Route("logIn", Name = "LogIn")]
        public IHttpActionResult LogIn()
        {
            return Ok("Authorization succeded!");
        }

        [SensorsManagerAuthorize]
        [HttpPut,Route("")]
        public IHttpActionResult Update(UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);//<---Remov

            var user = _userRep.Get(_credentials.Email, _credentials.Password);

            if (user == null)
            {
                return NotFound();
            }

            var compareName = _userRep.GetAll()
              .Where(u =>
              u.Email == userModel.Email
              && u.Id != user.Id).Count();
            if (compareName != 0)
            {
                return Conflict("There already is a user with that email.");
            }

            ModelToEntityMap.MapToEntity(userModel, user);
            _userRep.Update(user);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPut,Route("getResetCode")]
        public IHttpActionResult SendResetPasswordCode(UserModel_Email userModel)
        {
            if (userModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }
            var user = _userRep.GetAll().Where(u => u.Email == userModel.Email).SingleOrDefault();
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

        [HttpPut,Route("resetPassword")]
        public IHttpActionResult ResetPassword(UserModel_Code userModel)
        {
            var code = _memCache.Get(userModel.Code);
            if (code == null)
            {
                return Unauthorized("Invalid reset code!");
            }
            var user = _userRep.GetAll()
                .Where(u => u.Email == userModel.Email).SingleOrDefault();
            if (user == null)
            {
                return NotFound($"There is no user with the email {userModel.Email}");
            }
            user.Password = userModel.Password;
            _userRep.Update(user);
            _memCache.Remove(code.ToString());
            return Ok("Password has been reset.");
        }

        [HttpPut,Route("getValidationCode")]
        public IHttpActionResult SendValidationCode(UserModel_Email userModel)
        {

            if (userModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }
            var user = _userRep.GetAll().Where(u => u.Email == userModel.Email).SingleOrDefault();
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

        [HttpPut,Route("validation")]
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
    }
}
