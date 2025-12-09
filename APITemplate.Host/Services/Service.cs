using Microsoft.AspNetCore.Mvc;
using APITemplate.Host.Services.Interfaces;
using System.Diagnostics.Metrics;
using APITemplate.Host.Clients.Interfaces;

namespace APITemplate.Host.Services
{
    internal class Service : IService
    {
        private const string className = "TestService";
        private readonly ILogger<Service> _logger;
        private readonly IApiClient _apiClient;

        public Service(ILogger<Service> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<List<object>> GetAllAsync()
        {
            var obj = await _apiClient.GetAllAsync();
            return obj;
        }

        public async Task<object> GetByIdAsync(long id)
        {
            var obj = await _apiClient.GetByIdAsync(id);
            return obj;
        }
    }
}
