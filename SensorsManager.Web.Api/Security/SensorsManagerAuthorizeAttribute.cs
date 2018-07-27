using System;
using System.Text;
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
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                return;
            }
            var authHeader = actionContext.Request.Headers.Authorization;
            if(authHeader != null)
            {
                if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase)
                    && !string.IsNullOrWhiteSpace(authHeader.Parameter) && (authHeader.Parameter.Length % 4 == 0))
                {
                    var credentials = new Credentials(authHeader.Parameter);

                    if (UserLogIn.LogIn(credentials.Email, credentials.Password))
                    {
                        var principal = new GenericPrincipal(new GenericIdentity(credentials.Email), null);
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
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='SensorsManager' location='.../api/users'");
        }
    }
}
