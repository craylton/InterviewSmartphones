using Serilog;
using InterviewSmartphones.Console.Models;

namespace InterviewSmartphones.Console.Services;

public class ProductService(IProductClient productClient, ILogger logger) : IProductService
{
    private readonly IProductClient _productClient = productClient;
    private readonly ILogger _logger = logger;

    public async Task<ProductsResult> GetMostExpensiveProducts(string category, int numberOfProducts)
    {
        _logger.Information("Starting to retrieve most expensive products for category: {Category}, count: {Count}", category, numberOfProducts);

        try
        {
            var allProducts = await _productClient.GetAllProductsAsync();

            if (allProducts.Count == 0)
            {
                _logger.Warning("No products found");
                return new ProductsResult { Success = false };
            }

            var mostExpensiveProducts = allProducts
                .Where(p => p.Category == category)
                .OrderByDescending(p => p.Price)
                .Take(numberOfProducts);

            var productsList = mostExpensiveProducts.ToList();
            _logger.Information("Found {Count} products in category {Category}", productsList.Count, category);

            return new ProductsResult
            {
                Products = productsList,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while retrieving most expensive products");
            return new ProductsResult { Success = false };
        }
    }

    public async Task UpdateProductPrices(IEnumerable<ProductResponse> products)
    {
        _logger.Information("Starting price update process");

        System.Console.WriteLine("\nEnter percentage to increase prices:");
        var percentStr = System.Console.ReadLine();
        if (!decimal.TryParse(percentStr, out decimal percent))
        {
            _logger.Warning("Invalid percentage entered: {Percentage}", percentStr);
            System.Console.WriteLine("Invalid percentage.");
            return;
        }

        _logger.Information("Updating prices with {Percentage}% increase", percent);
        System.Console.WriteLine(new string('-', 20));

        var productsList = products.ToList();

        foreach (var product in productsList)
        {
            decimal newPrice = Math.Round(product.Price * (1 + percent / 100), 2);

            try
            {
                var updatedProduct = await _productClient.UpdateProductPriceAsync(product.Id, newPrice);

                if (updatedProduct is null)
                {
                    _logger.Warning("Failed to update product ID {ProductId} - null response", product.Id);
                    System.Console.WriteLine($"Failed to update product ID {product.Id}");
                    continue;
                }

                _logger.Information("Successfully updated product ID {ProductId} price to {NewPrice}", product.Id, updatedProduct.Price);
                updatedProduct.PrintDetails();
                System.Console.WriteLine(new string('-', 20));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred while updating product ID {ProductId}", product.Id);
                System.Console.WriteLine($"Error updating product ID {product.Id}");
            }
        }

        _logger.Information("Price update completed.");
    }
}
