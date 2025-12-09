using Microsoft.AspNetCore.Mvc;

namespace APITemplate.Host.Services.Interfaces
{
    public interface IService
    {
        public Task<List<object>> GetAllAsync();
        public Task<object> GetByIdAsync(long id);
    }
}
