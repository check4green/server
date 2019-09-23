
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
        IMapper _mapper;
        IMessageService _messages;

        public UsersController(IUsersControllerDependencyBlock dependencyBlock)
        {
            _userRep = dependencyBlock.UserRepository;
            _credentials = dependencyBlock.CredentialService;
            _dateTime = dependencyBlock.DateTimeService;
            _memCache = dependencyBlock.MemCacheService;
            _random = dependencyBlock.RandomService;
            _mailSender = dependencyBlock.MailSenderService;
            _mapper = dependencyBlock.Mapper;
            _messages = dependencyBlock.MessageService;
        }

        [HttpPost,Route(""), ValidateModel]
        public IHttpActionResult Add(UserModelPost userModel)
        {
            if (userModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            var checkedUser = _userRep.Get(userModel.Email, userModel.Password);

            if (checkedUser != null)
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "User", "Email");
                return Conflict(errorMessage);
            }

            var email = _memCache.Get(userModel.Email);
            if (email == null)
            {
                var errorMessage = _messages.GetMessage(Email.InvalidEmail);
                return Unauthorized(errorMessage);
            }

            var user = _mapper.Map<User>(userModel);
            _userRep.Add(user);

            _memCache.Remove(user.Email);

            var createdUser = _mapper.Map<UserModelGet>(user);

            return CreatedAtRoute("GetUser", new { email = user.Email }, createdUser);
        }

        [HttpPost,Route("~/api/demoRequest")]
        public IHttpActionResult RequestDemo(DemoRequestModel requestModel)
        {
            if (requestModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
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
                var message = _messages.GetMessage(Email.MailSucceded);
                return Ok(message);
            }
            catch (Exception)
            {
                var errorMessage = _messages.GetMessage(Email.MailFailed);
                return ExpectationFailed(errorMessage);
            }
        }

        [HttpPost,Route("~/api/contact")]
        public IHttpActionResult SendContactInfo(ContactInfoModel contactInfoModel)
        {
            if (contactInfoModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }
            try
            {

                string body = $"Name: {contactInfoModel.FullName}\n" +
                    $"Email address: {contactInfoModel.Email}\n" +
                    $"Phone: {contactInfoModel.Phone}\n" +
                    $"Message: {contactInfoModel.Message}";

                _mailSender.SendMail("info@check4green.com", "Contact info", body);
                var message = _messages.GetMessage(Email.MailSucceded);
                return Ok(message);
            }
            catch (Exception)
            {
                var errorMessage = _messages.GetMessage(Email.MailFailed);
                return ExpectationFailed(errorMessage);
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
                var errorMessage = _messages.GetMessage(Custom.NotFound, "User", "Email");
                return NotFound(errorMessage);
            }

            var userModel = _mapper.Map<UserModelGet>(user);

            return Ok(userModel);
        }

        [SensorsManagerAuthorize]
        [HttpPost,Route("logIn", Name = "LogIn")]
        public IHttpActionResult LogIn()
        {
            return Ok("Authorization succeded!");
        }

        [SensorsManagerAuthorize, ValidateModel]
        [HttpPut,Route("")]
        public IHttpActionResult Update(UserModelPut userModel)
        {
            if (userModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);

            var user = _userRep.Get(_credentials.Email, _credentials.Password);

            if (user == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "User", "Email");
                return NotFound(errorMessage);
            }

            if (_userRep.GetAll()
                    .Any(u => u.Email == userModel.Email && u.Id != user.Id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "User", "Email");
                return Conflict(errorMessage);
            }

            _mapper.Map(userModel, user);
            _userRep.Update(user);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [SensorsManagerAuthorize]
        [HttpPatch, Route("")]
        public IHttpActionResult PartialUpdate([FromBody] JsonPatchDocument<UserModelPut> patchDoc)
        {
            if (patchDoc == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            _credentials.SetCredentials(Request.Headers.Authorization.Parameter);

            var user = _userRep.Get(_credentials.Email, _credentials.Password);

            if (user == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "User", "Email");
                return NotFound(errorMessage);
            }

            var userModel = _mapper.Map<UserModelPut>(user);
            try
            {
                patchDoc.ApplyTo(userModel);
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

            Validate(userModel);
            if (!ModelState.IsValid)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();
                var errorMessage = error != null ?
                    error.ErrorMessage : _messages.GetMessage(Generic.InvalidRequest);
                return BadRequest(errorMessage);
            }

            if (_userRep.GetAll()
                  .Any(u => u.Email == userModel.Email && u.Id != user.Id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "User", "Email");
                return Conflict(errorMessage);
            }

            _mapper.Map(userModel, user);
            _userRep.Update(user);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPut,Route("resetCode"), ValidateModel]
        public IHttpActionResult SendResetPasswordCode(UserModel_Email userModel)
        {
            if (userModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }
            var user = _userRep.GetAll().Where(u => u.Email == userModel.Email).SingleOrDefault();
            if (user == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "User", "Email");
                return NotFound(errorMessage);
            }
            try
            {
                var code = _random.GetRandomNumber(1000, 9999).ToString();
                _memCache.Add(code, code, _dateTime.GetDateOffSet().AddMinutes(5));

                _mailSender.SendMail(userModel.Email, "Reset password",
                    $"Here is your password reset code: {code}");
                var message = _messages.GetMessage(Email.MailSucceded_Code);
                return Ok(message);
            }
            catch
            {
                var errorMessage = _messages.GetMessage(Email.MailFailed);
                return ExpectationFailed(errorMessage);
            }
        }

        [HttpPut,Route("password"), ValidateModel] 
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
                var errorMessage = _messages.GetMessage(Custom.NotFound, "User", "Email");
                return NotFound(errorMessage);
            }
            user.Password = userModel.Password;
            _userRep.Update(user);
            _memCache.Remove(code.ToString());
            return Ok("Password has been reset.");
        }

        [HttpPut,Route("validationCode"), ValidateModel]
        public IHttpActionResult SendValidationCode(UserModel_Email userModel)
        {

            if (userModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }
            var user = _userRep.GetAll().Where(u => u.Email == userModel.Email).SingleOrDefault();
            if (user != null)
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "User", "Email");
                return Conflict($"There already exists an user with the mail {userModel.Email}.");
            }

            try
            {
                var code = _random.GetRandomNumber(1000, 9999).ToString();
                _memCache.Add(code, code, _dateTime.GetDateOffSet().AddMinutes(5));

                _mailSender.SendMail(userModel.Email, "Validation code",
                    $"Here is your validation code: {code}");
                var errorMessage = _messages.GetMessage(Email.MailSucceded_Code);
                return Ok(errorMessage);
            }
            catch
            {
                var errorMessage = _messages.GetMessage(Email.MailFailed);
                return ExpectationFailed(errorMessage);
            }
        }

        [HttpPut,Route("validation"), ValidateModel]
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
