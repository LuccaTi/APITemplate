using Microsoft.AspNetCore.Mvc;
using Service.API.Interfaces;

namespace Service.API.Services
{
    internal class ApiService : IApiService
    {
        #region Atributes
        private const string className = "ApiService";
        #endregion

        public Task<object> Get()
        {
            return Task.FromResult<object>(new { message = "API is Working!" });
        }
    }
}
