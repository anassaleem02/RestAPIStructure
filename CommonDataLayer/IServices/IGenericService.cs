using CommonDataLayer.Model.ResponseModels;

namespace CommonDataLayer.IServices
{
    public interface IGenericService<TDto, TEntity>
      where TDto : class
      where TEntity : class
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> AddAsync(TEntity TDto);
        Task<ApiResponse> UpdateAsync(int id, TEntity TDto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
