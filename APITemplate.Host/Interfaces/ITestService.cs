using Microsoft.AspNetCore.Mvc;

namespace APITemplate.Host.Interfaces
{
    public interface ITestService
    {
        public Task<object> Get();
    }
}
