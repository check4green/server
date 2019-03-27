using SensorsManager.Web.Api.ServiceInterfaces;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SensorsManager.Web.Api.ActionResults
{
    public class TooManyRequestsActionResult : IHttpActionResult
    {
        private HttpRequestMessage _request;

        private IThrottlerService _throttler;

        public TooManyRequestsActionResult(HttpRequestMessage request, IThrottlerService throttler)
        {
            _request = request;
            _throttler = throttler;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var msg = _request.CreateResponse(
                (HttpStatusCode)429,
                new
                {
                    Message = $"Too many requests!" +
                    $" You have exceded the limit of {_throttler.RequestLimit} request(s) per {_throttler.TimeoutInSeconds} second(s)!"
                });
            
            return Task.FromResult(msg);

        }
    }
}