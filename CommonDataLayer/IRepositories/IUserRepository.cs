using CommonDataLayer.DTOs;
using CommonDataLayer.Entities;
using System.Data;

namespace CommonDataLayer.IRepositories
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        Task<IEnumerable<UserRolesDto>> GetUserRoleAsync(IDbTransaction transaction = null);
    }
}
