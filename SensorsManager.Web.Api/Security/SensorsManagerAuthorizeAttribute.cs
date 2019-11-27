using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Security.Principal;
using System.Threading;
            
namespace SensorsManager.Web.Api.Security
{
    public class SensorsManagerAuthorizeAttribute : AuthorizationFilterAttribute
    {
        private ICredentialService _credentials = new CredentialService();
        private IUserLogIn _userLogIn = new UserLogIn();

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                return;
            }
            var authHeader = actionContext.Request.Headers.Authorization;
            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(authHeader.Parameter) && (authHeader.Parameter.Length % 4 == 0))
                {
                    _credentials.SetCredentials(authHeader.Parameter);

                    if (_userLogIn.LogIn(_credentials.Email, _credentials.Password))
                    {
                      
                            var principal = new GenericPrincipal(new GenericIdentity(_credentials.Email), null);
                            Thread.CurrentPrincipal = principal;
                            return;
                    }
                }

            }
            HandleUnauthorized(actionContext);

        }

        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request
                .CreateResponse(HttpStatusCode.Unauthorized, new { Message = "Authentification failed!" });
            actionContext.Response.Headers.Add("WWW-Reg", "Basic Scheme='SensorsManager' location='.../api/users'");
        }
    }
}
