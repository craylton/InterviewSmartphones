using Microsoft.AspNetCore.Mvc;
using InterviewSmartphones.Models;
using InterviewSmartphones.Services;

namespace InterviewSmartphones.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmartphonesController : ControllerBase
    {
        private readonly IDummyJsonService _dummyJsonService;
        private readonly ILogger<SmartphonesController> _logger;

        public SmartphonesController(IDummyJsonService dummyJsonService, ILogger<SmartphonesController> logger)
        {
            _dummyJsonService = dummyJsonService;
            _logger = logger;
        }

        [HttpGet("top-three")]
        public async Task<ActionResult<ApiResponse<List<SmartphoneDto>>>> GetTopThreeSmartphones()
        {
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Request to get smartphones without valid authorization header");
                return Unauthorized(new ApiResponse<List<SmartphoneDto>>
                {
                    Success = false,
                    Message = "Authorization token is required",
                    Errors = new List<string> { "Please provide a valid Bearer token" }
                });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            _logger.LogInformation("Fetching top three smartphones");
            
            var result = await _dummyJsonService.GetTopThreeSmartphonesAsync(token);
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("update-prices")]
        public async Task<ActionResult<ApiResponse<List<SmartphoneDto>>>> UpdateSmartphonePrices([FromBody] PriceUpdateRequest request)
        {
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Request to update prices without valid authorization header");
                return Unauthorized(new ApiResponse<List<SmartphoneDto>>
                {
                    Success = false,
                    Message = "Authorization token is required",
                    Errors = new List<string> { "Please provide a valid Bearer token" }
                });
            }

            if (request.PercentageIncrease < 0 || request.PercentageIncrease > 1000)
            {
                _logger.LogWarning("Invalid percentage increase: {Percentage}", request.PercentageIncrease);
                return BadRequest(new ApiResponse<List<SmartphoneDto>>
                {
                    Success = false,
                    Message = "Invalid percentage increase",
                    Errors = new List<string> { "Percentage must be between 0 and 1000" }
                });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            _logger.LogInformation("Updating smartphone prices by {Percentage}%", request.PercentageIncrease);
            
            // First, get the current top three smartphones
            var smartphonesResult = await _dummyJsonService.GetTopThreeSmartphonesAsync(token);
            if (!smartphonesResult.Success || smartphonesResult.Data == null)
            {
                return BadRequest(smartphonesResult);
            }

            var updatedSmartphones = new List<SmartphoneDto>();
            var errors = new List<string>();

            foreach (var smartphone in smartphonesResult.Data)
            {
                var newPrice = smartphone.Price * (1 + request.PercentageIncrease / 100);
                var updateResult = await _dummyJsonService.UpdateProductPriceAsync(smartphone.Id, newPrice, token);
                
                if (updateResult.Success && updateResult.Data != null)
                {
                    updateResult.Data.OriginalPrice = smartphone.Price;
                    updatedSmartphones.Add(updateResult.Data);
                }
                else
                {
                    _logger.LogWarning("Failed to update price for smartphone {Id}: {Message}", 
                        smartphone.Id, updateResult.Message);
                    errors.AddRange(updateResult.Errors);
                }
            }

            if (updatedSmartphones.Any())
            {
                return Ok(new ApiResponse<List<SmartphoneDto>>
                {
                    Success = true,
                    Data = updatedSmartphones.OrderByDescending(s => s.Price).ToList(),
                    Message = $"Updated {updatedSmartphones.Count} smartphone prices by {request.PercentageIncrease}%",
                    Errors = errors
                });
            }
            else
            {
                return BadRequest(new ApiResponse<List<SmartphoneDto>>
                {
                    Success = false,
                    Message = "Failed to update any smartphone prices",
                    Errors = errors
                });
            }
        }
    }
}