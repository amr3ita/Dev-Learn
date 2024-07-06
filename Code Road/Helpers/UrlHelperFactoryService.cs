namespace Code_Road.Helpers
{
    public class UrlHelperFactoryService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public UrlHelperFactoryService(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public string Action(string action, string controller, object values, string scheme)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var path = _linkGenerator.GetPathByAction(httpContext, action, controller, values);
            return $"{scheme}://{httpContext.Request.Host}{path}";
        }
    }
}