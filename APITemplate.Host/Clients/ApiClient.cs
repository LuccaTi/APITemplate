using APITemplate.Host.Clients.Interfaces;
using APITemplate.Host.Clients.Internal;

namespace APITemplate.Host.Clients
{
    public class ApiClient : ApiClientBase, IApiClient
    {
        private const string _className = "ApiClient";
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger) : base(httpClient, logger)
        {
            _logger = logger;
        }

        public async Task<List<object>> GetAllAsync()
        {
            var obj = await base.GetAllAsync<object>("users");
            return obj;
        }

        public async Task<object> GetByIdAsync(long id)
        {
            var obj = await base.GetByIdAsync<object>($"users/{id}");
            if (obj == null)
                obj = new object();

            return obj;
        }
    }
}
