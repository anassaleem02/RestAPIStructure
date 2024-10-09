using CommonDataLayer.DTOs;
using CommonDataLayer.Entities;
using CommonDataLayer.Model;

namespace CommonDataLayer.IServices
{
    public interface IUserService : IGenericService<UserDto, Users>
    {
        Task<ApiResponse> GetUserByEmailAsync(string email);
        Task<ApiResponse> GetUserRolesAsync();
    }
}
