using CommonDataLayer.DTOs;
using CommonDataLayer.Entities;
using CommonDataLayer.Model.ResponseModels;

namespace CommonDataLayer.IServices
{
    public interface IUserService : IGenericService<UserDto, Users>
    {
        Task<ApiResponse> GetUserByEmailAsync(string email);
        Task<ApiResponse> GetUserRolesAsync();
        Task<ApiResponse> GetUsersAsync(string searchTerm = null, string searchColumn = null, string orderByColumn = "Id", bool isAscending = true, int pageSize = 10, int pageNumber = 1, List<string>? searchableColumns = null);

    }
}
