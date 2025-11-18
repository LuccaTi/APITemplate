using APITemplate.Host.Logging;
using APITemplate.Host.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APITemplate.Host.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController : ControllerBase
    {
        #region Attributes
        private const string _className = "TestController";
        #endregion

        #region Dependencies
        private readonly IService _service;
        #endregion

        public TestController(IService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var message = await _service.GetAllAsync();
                return Ok(message);
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetAllAsync", $"Error processing request: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            try
            {
                var message = await _service.GetByIdAsync(id);
                return Ok(message);
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "GetByIdAsync", $"Error processing request: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
