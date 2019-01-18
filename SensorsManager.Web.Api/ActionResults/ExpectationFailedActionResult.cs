using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SensorsManager.Web.Api.ActionResults
{
    internal class ExpectationFailedActionResult : IHttpActionResult
    {
        private HttpRequestMessage _request;
        private string _message;

        public ExpectationFailedActionResult(HttpRequestMessage request, string message)
        {
            _request = request;
            _message = message;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var msg = _request.CreateResponse(
                HttpStatusCode.ExpectationFailed, new { Message = _message });
            return Task.FromResult(msg);
        }
    }
}