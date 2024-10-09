using CommonDataLayer.DTOs;
using CommonDataLayer.Entities;
using CommonDataLayer.IRepositories;
using System.Data;


namespace DataAccessLayer.Repositories
{
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        public UserRepository(IDbConnection dbConnection) : base(dbConnection) { }

        public async Task<IEnumerable<UserRolesDto>> GetUserRoleAsync(IDbTransaction transaction = null)
        {
            var sql = @"SELECT u.id, u.Username,r.RoleName FROM UserRoles ur join Users u on ur.UserId=u.Id join Roles r on r.Id=ur.RoleId;";

            return await ExecuteJoinAsync<UserRolesDto>(sql, transaction: transaction);
        }
    }
}
