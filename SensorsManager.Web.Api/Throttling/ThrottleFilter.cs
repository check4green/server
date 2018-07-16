using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SensorsManager.Web.Api.Throttling
{
    public class ThrottleFilter : ActionFilterAttribute
    {
        private Throttler _throttler;
        private string _throttleGroup;

        public ThrottleFilter(int RequestLimit = 5,
            int TimeoutInSeconds = 10,
            string ThrottleGroup = "")
        {
            _throttleGroup = ThrottleGroup;
            _throttler = new Throttler(ThrottleGroup, RequestLimit, TimeoutInSeconds);
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            setIdentityAsThrottleGroup();

            if (_throttler.RequestShouldBeThrottled())
            {
                actionContext.Response =
                    actionContext.Request.CreateResponse(
                        (HttpStatusCode)429,
                        new { Message = "Too many requests."}
                        );
                addThrottleHeaders(actionContext.Response);
            }
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

            setIdentityAsThrottleGroup();

            addThrottleHeaders(actionExecutedContext.Response);
            base.OnActionExecuted(actionExecutedContext);
        }

        private void addThrottleHeaders(HttpResponseMessage response)
        {
            if (response == null) return;

            response.Headers.Add("X-RateLimit-RequestLimit", _throttler.RequestLimit.ToString());
            response.Headers.Add("X-RateLimit-RequestCount", _throttler.RequestCount.ToString());
            response.Headers.Add("X-RateLimit-ExpiresInSeconds", _throttler.ExpiresAt.ToString());
        }

        private void setIdentityAsThrottleGroup()
        {
            if (_throttleGroup == "identity")
                _throttler.ThrottleGroup = HttpContext.Current.User.Identity.Name;

            if (_throttleGroup == "ipaddress")
                _throttler.ThrottleGroup = HttpContext.Current.Request.UserHostAddress;
        }

      
    }
}