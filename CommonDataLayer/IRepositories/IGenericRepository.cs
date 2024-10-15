using CommonDataLayer.Enums;
using CommonDataLayer.Model;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Data;

namespace CommonDataLayer.IRepositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetDataAsync(string searchTerm, string searchColumn, string orderByColumn, bool isAscending, int pageSize, int pageNumber, List<string> searchableColumns, IDbTransaction transaction = null);
        Task<IEnumerable<TEntity>> GetAllAsync(IDbTransaction transaction = null);
        Task<TEntity> GetByIdAsync(int id, IDbTransaction transaction = null);
        Task<int> AddAsync(TEntity entity, IDbTransaction transaction = null);
        Task<int> UpdateAsync(TEntity entity, IDbTransaction transaction = null);
        Task<int> DeleteAsync(int id, IDbTransaction transaction = null);
        Task<int> SoftDeleteAsync(int id, IDbTransaction transaction = null);
        public Task<ApiResponse> ExecuteInTransactionAsync(Func<IDbTransaction, Task<ApiResponse>> operation, ILogger logger, IStringLocalizer localizer, ErrorCode errorCode, SuccessCode? successCode = null);
        Task<IEnumerable<TResult>> ExecuteJoinAsync<TResult>(string sql, object parameters = null, IDbTransaction transaction = null);
    }
}
