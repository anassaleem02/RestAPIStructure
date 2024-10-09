using CommonDataLayer.DTOs;
using CommonDataLayer.Entities;
using CommonDataLayer.IServices;
using Microsoft.AspNetCore.Mvc;

namespace WorshipcareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : BaseController
    {
        private readonly IGenericService<UserDto, Users> _genericService;
        private readonly IUserService _userService;

        public UserController(IGenericService<UserDto, Users> genericService,IUserService userService,ILogger<UserController> logger) : base(logger)
        {
            _genericService = genericService;
            _userService = userService;
        }

        [HttpGet]
        public Task<IActionResult> GetAll()
        {
            return ExecuteAndLogAsync(nameof(GetAll), async () =>
            {
                var response = await _genericService.GetAllAsync();
                return StatusCode((int)response.HttpStatusCode, response);
            });
        }

        [HttpGet("{id}")]
        public Task<IActionResult> GetById(int id)
        {
            return ExecuteAndLogAsync(nameof(GetById), async () =>
            {
                var response = await _genericService.GetByIdAsync(id);
                return StatusCode((int)response.HttpStatusCode, response);
            });
        }

        [HttpPost]
        public Task<IActionResult> Add([FromBody] UserDto userDto)
        {
            return ExecuteAndLogAsync(nameof(Add), async () =>
            {
                var response = await _genericService.AddAsync(userDto);
                return StatusCode((int)response.HttpStatusCode, response);
            });
        }

        [HttpPut("{id}")]
        public Task<IActionResult> Update(int id, [FromBody] UserDto userDto)
        {
            return ExecuteAndLogAsync(nameof(Update), async () =>
            {
                var response = await _genericService.UpdateAsync(id, userDto);
                return StatusCode((int)response.HttpStatusCode, response);
            });
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(int id)
        {
            return ExecuteAndLogAsync(nameof(Delete), async () =>
            {
                var response = await _genericService.DeleteAsync(id);
                return StatusCode((int)response.HttpStatusCode, response);
            });
        }

        [HttpGet("user/email/{email}")]
        public Task<IActionResult> GetByEmail(string email)
        {
            return ExecuteAndLogAsync(nameof(GetByEmail), async () =>
            {
                var response = await _userService.GetUserByEmailAsync(email);
                return StatusCode((int)response.HttpStatusCode, response);
            });
        }
        [HttpGet("users/roles")]
        public Task<IActionResult> GetUsersAndRoles()
        {
            return ExecuteAndLogAsync(nameof(GetByEmail), async () =>
            {
                var response = await _userService.GetUserRolesAsync();
                return StatusCode((int)response.HttpStatusCode, response);
            });
        }
    }
}

