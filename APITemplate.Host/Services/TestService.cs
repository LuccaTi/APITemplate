using Microsoft.AspNetCore.Mvc;
using APITemplate.Host.Interfaces;

namespace APITemplate.Host.Services
{
    internal class TestService : ITestService
    {
        #region Atributes
        private const string className = "TestService";
        #endregion

        public Task<object> Get()
        {
            return Task.FromResult<object>(new { message = "Aplicação está funcionando!" });
        }
    }
}
