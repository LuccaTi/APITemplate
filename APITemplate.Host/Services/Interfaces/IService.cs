using Microsoft.AspNetCore.Mvc;

namespace APITemplate.Host.Services.Interfaces
{
    public interface IService
    {
        public Task<object> GetAllAsync();
        public Task<object> GetByIdAsync(long id);
    }
}
