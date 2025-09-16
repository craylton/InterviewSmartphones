using InterviewSmartphones.Console.Models;

namespace InterviewSmartphones.Console.Services;

public interface IProductClient
{
    Task<List<ProductResponse>> GetAllProductsAsync();
    Task<ProductResponse> UpdateProductPriceAsync(int productId, decimal newPrice);
}