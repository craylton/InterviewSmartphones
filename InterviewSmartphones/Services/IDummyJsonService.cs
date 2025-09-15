using InterviewSmartphones.Models;

namespace InterviewSmartphones.Services
{
    public interface IDummyJsonService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest);
        Task<ApiResponse<List<SmartphoneDto>>> GetTopThreeSmartphonesAsync(string token);
        Task<ApiResponse<SmartphoneDto>> UpdateProductPriceAsync(int productId, decimal newPrice, string token);
        Task<ApiResponse<List<User>>> GetUsersAsync();
    }
}