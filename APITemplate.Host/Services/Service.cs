using Microsoft.AspNetCore.Mvc;
using APITemplate.Host.Services.Interfaces;

namespace APITemplate.Host.Services
{
    internal class Service : IService
    {
        #region Attributes
        private const string className = "TestService";
        #endregion
        public Task<object> GetAllAsync()
        {
            return Task.FromResult<object>(new { message = "Application is working!" });
        }

        public Task<object> GetByIdAsync(long id)
        {
            return Task.FromResult<object>(new { message = "Application is working!" });
        }
    }
}
