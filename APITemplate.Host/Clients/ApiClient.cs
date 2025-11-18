using APITemplate.Host.Clients.Interfaces;
using APITemplate.Host.Clients.Internal;

namespace APITemplate.Host.Clients
{
    public class ApiClient : ApiClientBase, IApiClient
    {
        #region Atributes
        private const string _className = "ApiClient";
        #endregion

        public ApiClient(HttpClient httpClient) : base(httpClient)
        {

        }
    }
}
