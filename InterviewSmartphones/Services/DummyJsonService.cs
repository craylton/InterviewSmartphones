using InterviewSmartphones.Models;
using System.Text.Json;
using System.Text;

namespace InterviewSmartphones.Services
{
    public class DummyJsonService : IDummyJsonService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DummyJsonService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public DummyJsonService(HttpClient httpClient, ILogger<DummyJsonService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation("Attempting login for user: {Username}", loginRequest.Username);
                
                var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("https://dummyjson.com/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Login API response status: {StatusCode}", response.StatusCode);
                _logger.LogDebug("Login API response content: {Content}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
                    return new ApiResponse<LoginResponse>
                    {
                        Success = true,
                        Data = loginResponse,
                        Message = "Login successful"
                    };
                }
                else
                {
                    _logger.LogWarning("Login failed with status: {StatusCode}, Content: {Content}", 
                        response.StatusCode, responseContent);
                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Login failed",
                        Errors = new List<string> { responseContent }
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during login for user: {Username}", loginRequest.Username);
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Network error occurred",
                    Errors = new List<string> { ex.Message }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user: {Username}", loginRequest.Username);
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<SmartphoneDto>>> GetTopThreeSmartphonesAsync(string token)
        {
            try
            {
                _logger.LogInformation("Fetching smartphones with token");
                
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("https://dummyjson.com/auth/products");
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Products API response status: {StatusCode}", response.StatusCode);
                _logger.LogDebug("Products API response content length: {Length}", responseContent.Length);

                if (response.IsSuccessStatusCode)
                {
                    var productsResponse = JsonSerializer.Deserialize<ProductsResponse>(responseContent, _jsonOptions);
                    
                    var smartphones = productsResponse?.Products?
                        .Where(p => p.Category.ToLower().Contains("smartphone") || 
                                   p.Title.ToLower().Contains("phone") ||
                                   p.Title.ToLower().Contains("smartphone"))
                        .OrderByDescending(p => p.Price)
                        .Take(3)
                        .Select(p => new SmartphoneDto
                        {
                            Id = p.Id,
                            Brand = p.Brand,
                            Title = p.Title,
                            Price = p.Price,
                            OriginalPrice = p.Price
                        })
                        .ToList() ?? new List<SmartphoneDto>();

                    _logger.LogInformation("Found {Count} smartphones", smartphones.Count);

                    return new ApiResponse<List<SmartphoneDto>>
                    {
                        Success = true,
                        Data = smartphones,
                        Message = $"Found {smartphones.Count} smartphones"
                    };
                }
                else
                {
                    _logger.LogWarning("Failed to fetch products with status: {StatusCode}, Content: {Content}", 
                        response.StatusCode, responseContent);
                    return new ApiResponse<List<SmartphoneDto>>
                    {
                        Success = false,
                        Message = "Failed to fetch products",
                        Errors = new List<string> { responseContent }
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while fetching smartphones");
                return new ApiResponse<List<SmartphoneDto>>
                {
                    Success = false,
                    Message = "Network error occurred",
                    Errors = new List<string> { ex.Message }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching smartphones");
                return new ApiResponse<List<SmartphoneDto>>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<SmartphoneDto>> UpdateProductPriceAsync(int productId, decimal newPrice, string token)
        {
            try
            {
                _logger.LogInformation("Updating price for product {ProductId} to {NewPrice}", productId, newPrice);
                
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var updateData = new { price = newPrice };
                var json = JsonSerializer.Serialize(updateData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"https://dummyjson.com/auth/products/{productId}", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Update product API response status: {StatusCode}", response.StatusCode);
                _logger.LogDebug("Update product API response content: {Content}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var updatedProduct = JsonSerializer.Deserialize<Product>(responseContent, _jsonOptions);
                    var smartphoneDto = new SmartphoneDto
                    {
                        Id = updatedProduct!.Id,
                        Brand = updatedProduct.Brand,
                        Title = updatedProduct.Title,
                        Price = updatedProduct.Price
                    };

                    return new ApiResponse<SmartphoneDto>
                    {
                        Success = true,
                        Data = smartphoneDto,
                        Message = "Price updated successfully"
                    };
                }
                else
                {
                    _logger.LogWarning("Failed to update product price with status: {StatusCode}, Content: {Content}", 
                        response.StatusCode, responseContent);
                    return new ApiResponse<SmartphoneDto>
                    {
                        Success = false,
                        Message = "Failed to update product price",
                        Errors = new List<string> { responseContent }
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while updating product {ProductId}", productId);
                return new ApiResponse<SmartphoneDto>
                {
                    Success = false,
                    Message = "Network error occurred",
                    Errors = new List<string> { ex.Message }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating product {ProductId}", productId);
                return new ApiResponse<SmartphoneDto>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<User>>> GetUsersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching users list");
                
                var response = await _httpClient.GetAsync("https://dummyjson.com/users");
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Users API response status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var usersResponse = JsonSerializer.Deserialize<dynamic>(responseContent, _jsonOptions);
                    // Note: DummyJSON users endpoint returns users without passwords
                    // For demo purposes, we'll return a simplified response
                    
                    return new ApiResponse<List<User>>
                    {
                        Success = true,
                        Data = new List<User>(), // Simplified for security
                        Message = "Users endpoint available - use login endpoint with any DummyJSON user credentials"
                    };
                }
                else
                {
                    _logger.LogWarning("Failed to fetch users with status: {StatusCode}", response.StatusCode);
                    return new ApiResponse<List<User>>
                    {
                        Success = false,
                        Message = "Failed to fetch users",
                        Errors = new List<string> { responseContent }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching users");
                return new ApiResponse<List<User>>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}