﻿using AutoMapper;
using CommonDataLayer.DTOs;
using CommonDataLayer.Entities;
using CommonDataLayer.Enums;
using CommonDataLayer.IRepositories;
using CommonDataLayer.IServices;
using CommonDataLayer.Model;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BusinessLayer.Services
{
    public class UserService : GenericService<UserDto, Users>, IUserService
    {
        private readonly IGenericRepository<Users> _genericRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IStringLocalizer<UserService> _localizer;
        private readonly IUserRepository _userRepository;
        public UserService(IGenericRepository<Users> genericRepository, IMapper mapper,ILogger<UserService> logger,IStringLocalizer<UserService> localizer,IUserRepository userRepository)
            : base(genericRepository, mapper, logger, localizer)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
            _userRepository= userRepository;
        }

        public async Task<ApiResponse> GetUserByEmailAsync(string email)
        {
            return await _userRepository.ExecuteInTransactionAsync(async transaction =>
            {
                var users = await _userRepository.GetAllAsync(transaction);
                var user = users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    return new ApiResponse
                    {
                        HttpStatusCode = HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                var userDto = _mapper.Map<UserDto>(user);
                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Success = true,
                    Data = userDto
                };
            }, _logger, _localizer, ErrorCode.NotFound);
        }

        public async Task<ApiResponse> GetUserRolesAsync()
        {
            return await _userRepository.ExecuteInTransactionAsync(async transaction =>
            {
                var userRoles = await _userRepository.GetUserRoleAsync(transaction);
                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Success = true,
                    Data = userRoles // Return user roles
                };
            }, _logger, _localizer, ErrorCode.NotFound);
        }
    }
}