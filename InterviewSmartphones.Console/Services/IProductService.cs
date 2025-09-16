using InterviewSmartphones.Console.Models;

namespace InterviewSmartphones.Console.Services;

public interface IProductService
{
    Task<ProductsResult> GetMostExpensiveProducts(string category, int numberOfProducts);
    Task UpdateProductPrices(IEnumerable<ProductResponse> products);
}