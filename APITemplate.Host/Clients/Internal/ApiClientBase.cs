using APITemplate.Host.Logging;

namespace APITemplate.Host.Clients.Internal
{
    public class ApiClientBase
    {
        #region Atributes
        private const string _className = "ApiClientBase";
        protected readonly HttpClient _httpClient;
        #endregion

        public ApiClientBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected async Task<T?> GetByIdAsync<T>(string endpoint)
        {
            try
            {
                Logger.Debug(_className, "GetByIdAsync", $"Calling endpoint: {_httpClient.BaseAddress + endpoint}");
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                Logger.Debug(_className, "GetAllAsync", $"Request Success!");
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetByIdAsync", $"Error: {ex.Message}");
                throw;
            }
        }

        protected async Task<List<T>> GetAllAsync<T>(string endpoint)
        {
            try
            {
                Logger.Debug(_className, "GetAllAsync", $"Calling endpoint: {_httpClient.BaseAddress + endpoint}");
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<T>>();
                Logger.Debug(_className, "GetAllAsync", $"Request Success!");
                return result ?? new List<T>();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetAllAsync", $"Error: {ex.Message}");
                throw;
            }
        }
    }
}
