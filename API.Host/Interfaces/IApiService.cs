using Microsoft.AspNetCore.Mvc;

namespace Service.API.Interfaces
{
    public interface IApiService
    {
        public Task<object> Get();
    }
}
