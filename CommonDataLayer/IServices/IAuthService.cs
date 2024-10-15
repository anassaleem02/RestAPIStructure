using CommonDataLayer.Model.RequestModels;
using CommonDataLayer.Model.ResponseModels;

namespace CommonDataLayer.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse> Login(LoginRequestModel userreqModel);
    }
}
