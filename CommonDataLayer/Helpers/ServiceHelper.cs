using CommonDataLayer.Model;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CommonDataLayer.Helpers
{
    public static class ServiceHelper
    {
        public static async Task<ApiResponse> ExecuteWithErrorHandlingAsync(Func<Task<ApiResponse>> operation, ILogger logger, IStringLocalizer localizer, string errorMessageKey = "ErrorOccurred")
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, localizer[errorMessageKey]);
                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = localizer[errorMessageKey],
                    Error = ex.Message
                };
            }
        }
    }
}
