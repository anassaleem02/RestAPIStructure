using CommonDataLayer.DTOs;
using CommonDataLayer.Entities;
using CommonDataLayer.IServices;
using CommonDataLayer.Model.RequestModels;
using Microsoft.AspNetCore.Mvc;
using WorshipcareAPI.Controllers;

namespace APIStructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IGenericService<UserDto, Users> _genericService;
        private readonly IAuthService _authService;

        public AuthController(IGenericService<UserDto, Users> genericService, IAuthService authService, ILogger<UserController> logger) : base(logger)
        {
            _genericService = genericService;
            _authService = authService;
        }
        [HttpPost("login")]
        public Task<IActionResult> Login(LoginRequestModel user)
        {
            return ExecuteAndLogAsync(nameof(Login), async () =>
            {
                var response = await _authService.Login(user);
                return StatusCode((int)response.HttpStatusCode, response);
            });
        }
    }
}