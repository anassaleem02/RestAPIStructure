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
        /// <summary>
        /// Get all users with filters
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="searchColumn"></param>
        /// <param name="searchableColumns"></param>
        /// <param name="orderByColumn"></param>
        /// <param name="isAscending"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("filtered/users")]
        public Task<IActionResult> GetUsers([FromQuery] string searchTerm = null, [FromQuery] string searchColumn = null, [FromQuery] List<string> searchableColumns = null, [FromQuery] string orderByColumn = "Id", [FromQuery] bool isAscending = true, [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            var allowedColumns = new List<string> { "FirstName", "LastName", "Email", "UserName" };
            return ExecuteAndLogAsync(nameof(GetUsers), async () =>
            {
                var response = await _userService.GetUsersAsync(searchTerm, searchColumn, orderByColumn, isAscending, pageSize, pageNumber, searchableColumns is null ? allowedColumns : searchableColumns);
                return StatusCode((int)response.HttpStatusCode, response);
            });
        }
    }
}

