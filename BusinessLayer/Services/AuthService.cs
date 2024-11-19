using CommonDataLayer.Entities;
using CommonDataLayer.IServices;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using CommonDataLayer.Enums;
using CommonDataLayer.IRepositories;
using CommonDataLayer.DTOs;
using AutoMapper;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Net;
using CommonDataLayer.Model.ResponseModels;
using CommonDataLayer.Model.RequestModels;
using System.ComponentModel;
using CommonDataLayer.Helpers;

namespace BusinessLayer.Services
{
    public class AuthService : GenericService<UserDto, Users>, IAuthService
    {
        public readonly IConfiguration _configuration;
        private readonly IGenericRepository<Users> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IStringLocalizer<UserService> _localizer;
        private readonly IGenericRepository<UserRoles> _userRoleRepository;
        public AuthService(IConfiguration configuration, IGenericRepository<Users> genericRepository, IMapper mapper, ILogger<UserService> logger, IStringLocalizer<UserService> localizer, IUserRepository userRepository, IGenericRepository<UserRoles> userRoleRepository) : base(genericRepository, mapper, logger, localizer)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;   
            _localizer = localizer;
            _userRoleRepository= userRoleRepository;
        }

        public TokenResponse GenerateToken(Users user, List<UserRoles> userRoles)
        {
            // Define claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("UserId", user.Id.ToString())
            };
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, ((RoleEnum)role.RoleId).ToString())); 
            }

            // Generate security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"]));

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiry
            };
        }
        public async Task<ApiResponse> Login(LoginRequestModel userreqModel)
        {
            return await _userRepository.ExecuteInTransactionAsync(async transaction =>
            {
                var users = await _userRepository.GetAllAsync(transaction);
                var user = users.FirstOrDefault(u => u.Username == userreqModel.Username);
                var IsUserVerified = Utilities.VerifyPassword(userreqModel.Password, user.Password, user.Salt);
                if (!IsUserVerified)
                {
                    return new ApiResponse
                    {
                        HttpStatusCode = HttpStatusCode.NotFound,
                        Success = false
                    };
                }
                var userRoles = await _userRoleRepository.GetAllAsync();
                var userRolesList = userRoles.Where(x => x.UserId == user.Id).ToList();
                
                TokenResponse token = GenerateToken(user, userRolesList);
                return new ApiResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Success = true,
                    Data = token
                };
            }, _logger, _localizer, ErrorCode.NotFound);
        }
    }
}
