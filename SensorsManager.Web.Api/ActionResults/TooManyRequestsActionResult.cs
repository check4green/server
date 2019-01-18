using SensorsManager.Web.Api.Throttling;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace SensorsManager.Web.Api.ActionResults
{
    public class TooManyRequestsActionResult : IHttpActionResult
    {
        private HttpRequestMessage _request;
        private string _message;
        private Throttler _throttler;

        public TooManyRequestsActionResult(HttpRequestMessage request,
            string message, Throttler throttler)
        {
            _request = request;
            _message = message;
            _throttler = throttler;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var msg = _request.CreateResponse(
                (HttpStatusCode)429, new { Message = _message });
            

            msg.Headers.Add("X-RateLimit-RequestLimit",
                       _throttler.RequestLimit.ToString());
            msg.Headers.Add("X-RateLimit-RequestsRemaining",
              _throttler.RequestsRemaining.ToString());
            msg.Headers.Add("X-RateLimit-ExpiresAt",
                               _throttler.ExpiresAt.ToString());

            return Task.FromResult(msg);

        }
    }
}