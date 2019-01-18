using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace SensorsManager.Web.Api.ActionResults
{
    public class PagedActionResult<T> : IHttpActionResult where T : class
    {
        private HttpRequestMessage _request;
        private string _route;
        private int _page;
        private int _pageSize;
        private int _pageCount;
        private int _totalCount;
        private T _body;

        public PagedActionResult(
            HttpRequestMessage request, string route,
            int page, int pageSize, int pageCount, int totalCount, T body)
        {
            _request = request;
            _route = route;
            _page = page;
            _pageSize = pageSize;
            _pageCount = pageCount;
            _totalCount = totalCount;
            _body = body;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var msg = _request.CreateResponse(_body);
            var helper = new UrlHelper(_request);
            var prevUrl = _page > 1 ? helper.Link(_route, new { page = _page - 1, pageSize = _pageSize }) : "-";
            var nextUrl = _page < _pageCount ? helper.Link(_route, new { page = _page + 1, pageSize = _pageSize }) : "-";

            msg.Headers.Add("X-Tracker-Pagination-PrevPage", prevUrl);
            msg.Headers.Add("X-Tracker-Pagination-NextPage", nextUrl);
            msg.Headers.Add("X-Tracker-Pagination-Page", _page.ToString());
            msg.Headers.Add("X-Tracker-Pagination-PageSize", _pageSize.ToString());
            msg.Headers.Add("X-Tracker-Pagination-PageCount", _pageCount.ToString());
            msg.Headers.Add("X-Tracker-Pagination-TotalCount", _totalCount.ToString());

            return Task.FromResult(msg);

        }
    }
}