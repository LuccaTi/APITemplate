namespace APITemplate.Host.Clients.Interfaces
{
    public interface IApiClient
    {
        public Task<List<object>> GetAllAsync();
        public Task<object> GetByIdAsync(long id);
    }
}
