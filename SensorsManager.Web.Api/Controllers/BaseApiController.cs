using SensorsManager.Web.Api.ActionResults;
using SensorsManager.Web.Api.Pending;
using SensorsManager.Web.Api.Services;
using System.Web.Http;

namespace SensorsManager.Web.Api.Controllers
{
    public class BaseApiController : ApiController
    {
        private static SensorIntervalPending sensorIntervalPending;
        protected SensorIntervalPending TheSensorIntervalPending
        {
            get
            {
                if (sensorIntervalPending == null)
                {
                    sensorIntervalPending = new SensorIntervalPending();
                }
                return sensorIntervalPending;
            }
        }

        protected IHttpActionResult Ok(string message)
        {
            return new OkActionResult(Request, message);
        }

        protected IHttpActionResult Ok<T>(string route,
            int page, int pageSize, int pageCount, int totalCount, T body)
            where T : class
        {
            return new PagedActionResult<T>(Request, route,
            page, pageSize, pageCount, totalCount, body);
        }

        protected IHttpActionResult Conflict(string message)
        {
            return new ConflictActionResult(Request, message);
        }

        protected IHttpActionResult NotFound(string message)
        {
            return new NotFoundActionResult(Request, message);
        }

        protected IHttpActionResult TooManyRequests(IThrottlerService throttler)
        {
            return new TooManyRequestsActionResult(Request, throttler);
        }

        protected IHttpActionResult Unauthorized(string message)
        {
            return new UnauthorizedActionResult(Request, message);
        }

        protected IHttpActionResult ExpectationFailed(string message)
        {
            return new ExpectationFailedActionResult(Request, message);
        }

    }
}
