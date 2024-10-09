using AutoMapper;
using CommonDataLayer.Enums;
using CommonDataLayer.Helpers;
using CommonDataLayer.IRepositories;
using CommonDataLayer.IServices;
using CommonDataLayer.Model;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BusinessLayer.Services
{
    public class GenericService<TDto, TEntity> : IGenericService<TDto, TEntity> where TDto : class where TEntity : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GenericService<TDto, TEntity>> _logger;
        private readonly IStringLocalizer<GenericService<TDto, TEntity>> _localizer;

        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper, ILogger<GenericService<TDto, TEntity>> logger, IStringLocalizer<GenericService<TDto, TEntity>> localizer)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            return await _repository.ExecuteInTransactionAsync(async transaction =>
            {
                var entities = await _repository.GetAllAsync(transaction);
                var dtos = entities.Select(e => _mapper.Map<TDto>(e)).ToList();

                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Success = true,
                    Data = dtos,
                    Count = dtos.Count
                };
            }, _logger, _localizer, ErrorCode.ErrorOccurred);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            return await _repository.ExecuteInTransactionAsync(async transaction =>
            {
                var entity = await _repository.GetByIdAsync(id, transaction);
                if (entity == null)
                {
                    return new ApiResponse
                    {
                        HttpStatusCode = HttpStatusCode.NotFound,
                        Success = false,
                        Message = LocalizationHelper.GetLocalizedMessage(_localizer, ErrorCode.NotFound)
                    };
                }

                var dto = _mapper.Map<TDto>(entity);
                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Success = true,
                    Data = dto,
                };
            }, _logger, _localizer, ErrorCode.ErrorOccurred, SuccessCode.Success);
        }

        public async Task<ApiResponse> AddAsync(TDto dto)
        {
            return await _repository.ExecuteInTransactionAsync(async transaction =>
            {
                var entity = _mapper.Map<TEntity>(dto);
                var recordId = await _repository.AddAsync(entity, transaction);

                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.Created,
                    Success = true,
                    Data = new { recordId = recordId }
                };
            }, _logger, _localizer, ErrorCode.ErrorOccurred, SuccessCode.Success);
        }

        public async Task<ApiResponse> UpdateAsync(int id, TDto dto)
        {
            return await _repository.ExecuteInTransactionAsync(async transaction =>
            {
                var existingEntity = await _repository.GetByIdAsync(id, transaction);
                if (existingEntity == null)
                {
                    return new ApiResponse
                    {
                        HttpStatusCode = HttpStatusCode.NotFound,
                        Success = false,
                        Message = LocalizationHelper.GetLocalizedMessage(_localizer, ErrorCode.NotFound)
                    };
                }

                var updatedEntity = _mapper.Map(dto, existingEntity);
                var isUpdated = await _repository.UpdateAsync(updatedEntity, transaction);

                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Success = true,
                    Data = new { updated = true }
                };
            }, _logger, _localizer, ErrorCode.ErrorOccurred, SuccessCode.Success);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            return await _repository.ExecuteInTransactionAsync(async transaction =>
            {
                var existingEntity = await _repository.GetByIdAsync(id, transaction);
                if (existingEntity == null)
                {
                    return new ApiResponse
                    {
                        HttpStatusCode = HttpStatusCode.NotFound,
                        Success = false,
                        Message = LocalizationHelper.GetLocalizedMessage(_localizer, ErrorCode.NotFound)
                    };
                }

                await _repository.SoftDeleteAsync(id, transaction);

                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.NoContent,
                    Success = true,
                    Data = new { deleted = true },
                    Message = LocalizationHelper.GetLocalizedMessage(_localizer, SuccessCode.Success)
                };
            }, _logger, _localizer, ErrorCode.ErrorOccurred, SuccessCode.Success);
        }
    }
}