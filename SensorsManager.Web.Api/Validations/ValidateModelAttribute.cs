using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SensorsManager.Web.Api.Validations
{
    public class ValidateModelAttribute : ActionFilterAttribute 
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if(actionContext.ModelState.IsValid == false)
            {
                var error = actionContext.ModelState
                    .SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();
                var errorMessage = error != null? error.ErrorMessage : "Invalid request.";
                actionContext.Response = actionContext.Request
                    .CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }
        }
    }
}