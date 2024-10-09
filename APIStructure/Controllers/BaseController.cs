using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace WorshipcareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly ILogger<BaseController> _logger;

        protected BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }

        protected async Task<IActionResult> ExecuteAndLogAsync(string actionName, Func<Task<IActionResult>> action)
        {
            await LogRequestDetailsAsync(actionName);

            try
            {
                var result = await action();
                LogResponseDetails(actionName, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing action: {ActionName}", actionName);
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private async Task LogRequestDetailsAsync(string actionName)
        {
            _logger.LogInformation("Accessing action: {ActionName}, Endpoint: {Endpoint}, Method: {Method}",
                actionName, HttpContext.Request.Path, HttpContext.Request.Method);

            if (HttpContext.Request.ContentLength > 0)
            {
                // Save the current request body stream
                var originalBodyStream = HttpContext.Request.Body;

                // Create a new memory stream
                using var memoryStream = new MemoryStream();
                await HttpContext.Request.Body.CopyToAsync(memoryStream);

                // Log the request body
                memoryStream.Seek(0, SeekOrigin.Begin); // Rewind the memory stream
                using var reader = new StreamReader(memoryStream, Encoding.UTF8);
                var body = await reader.ReadToEndAsync();
                _logger.LogInformation("Request body: {Body}", body);

                // Reset the request body stream position so it can be read again by the request pipeline
                memoryStream.Seek(0, SeekOrigin.Begin); // Rewind the memory stream
                HttpContext.Request.Body = memoryStream;
            }
        }

        private void LogResponseDetails(string actionName, IActionResult result)
        {
            var statusCode = (result as StatusCodeResult)?.StatusCode ?? (result as ObjectResult)?.StatusCode;
            _logger.LogInformation("Action: {ActionName} completed with status code: {StatusCode}", actionName, statusCode);
        }
    }
}
