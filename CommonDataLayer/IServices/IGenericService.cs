using CommonDataLayer.Model;

namespace CommonDataLayer.IServices
{
    public interface IGenericService<TDto, TEntity>
      where TDto : class
      where TEntity : class
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> AddAsync(TDto dto);
        Task<ApiResponse> UpdateAsync(int id, TDto dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
