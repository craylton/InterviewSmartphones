using Microsoft.AspNetCore.Mvc;
using InterviewSmartphones.Models;
using InterviewSmartphones.Services;

namespace InterviewSmartphones.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IDummyJsonService _dummyJsonService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IDummyJsonService dummyJsonService, ILogger<AuthController> logger)
        {
            _dummyJsonService = dummyJsonService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                _logger.LogWarning("Login attempt with missing credentials");
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Username and password are required",
                    Errors = new List<string> { "Invalid credentials provided" }
                });
            }

            _logger.LogInformation("Login attempt for user: {Username}", loginRequest.Username);
            
            var result = await _dummyJsonService.LoginAsync(loginRequest);
            
            if (result.Success)
            {
                _logger.LogInformation("Successful login for user: {Username}", loginRequest.Username);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Failed login attempt for user: {Username}", loginRequest.Username);
                return Unauthorized(result);
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetUsers()
        {
            _logger.LogInformation("Fetching users list");
            
            var result = await _dummyJsonService.GetUsersAsync();
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}