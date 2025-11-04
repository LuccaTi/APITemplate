using Microsoft.AspNetCore.Mvc;
using Service.API.Interfaces;
using Service.API.Logging;

namespace Service.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController : ControllerBase
    {
        #region Atributes
        private const string _className = "TestController";
        private readonly IApiService _apiService;
        #endregion

        public TestController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var message = await _apiService.Get();
                return Ok(message);
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Get", $"Erro ao processar requisição: {ex.Message}");
                return BadRequest(ex);
            }
        }
    }
}
