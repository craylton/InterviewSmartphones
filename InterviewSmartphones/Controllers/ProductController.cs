using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using InterviewSmartphones.Models;

namespace InterviewSmartphones.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<ProductController> logger) : ControllerBase
{
    private readonly string _baseUrl = configuration["BaseUrl"] ?? "https://dummyjson.com";
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    private readonly ILogger<ProductController> _logger = logger;

    [HttpGet("most-expensive")]
    public async Task<ActionResult<List<Product>>> GetMostExpensiveProducts()
    {
        _logger.LogInformation("Fetching most expensive products...");

        try
        {
            var productResponse = await _httpClient.GetAsync($"{_baseUrl}/auth/products");
            var productJson = await productResponse.Content.ReadAsStringAsync();

            _logger.LogInformation($"Product response status: {productResponse.StatusCode}");

            if (!productResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch products from external API");
                return StatusCode(500, "Failed to fetch products from external API");
            }

            var productList = JsonSerializer.Deserialize<ProductList>(productJson);

            if (productList?.Products == null)
            {
                _logger.LogError("Invalid response from products API");
                return BadRequest("Invalid response from products API");
            }

            var mostExpensiveProducts = productList.Products
                .OrderByDescending(p => p.Price)
                .Take(3)
                .ToList();

            _logger.LogInformation($"Successfully retrieved {mostExpensiveProducts.Count} products");

            return Ok(mostExpensiveProducts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching top three most expensive products");
            return StatusCode(500, "Internal server error while fetching products");
        }
    }
}