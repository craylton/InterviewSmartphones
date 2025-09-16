using Serilog;
using InterviewSmartphones.Console.Models;

namespace InterviewSmartphones.Console.Services;

public class ProductService(IProductClient productClient, ILogger logger)
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

    public async Task<UpdateResult> UpdateProductPrices(IEnumerable<ProductResponse> products, decimal percentageIncrease)
    {
        _logger.Information("Starting price update process with {Percentage}% increase", percentageIncrease);

        var updatedProducts = new List<ProductResponse>();
        var productsList = products.ToList();

        try
        {
            foreach (var product in productsList)
            {
                decimal newPrice = Math.Round(product.Price * (1 + percentageIncrease / 100), 2);

                try
                {
                    var updatedProduct = await _productClient.UpdateProductPriceAsync(product.Id, newPrice);

                    if (updatedProduct is null)
                    {
                        _logger.Warning("Failed to update product ID {ProductId} - null response", product.Id);
                        continue;
                    }

                    _logger.Information("Successfully updated product ID {ProductId} price from {OldPrice} to {NewPrice}",
                        product.Id, product.Price, updatedProduct.Price);
                    updatedProducts.Add(updatedProduct);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Exception occurred while updating product ID {ProductId}", product.Id);
                    throw; // Re-throw to be caught by outer try-catch
                }
            }

            _logger.Information("Price update completed successfully. Updated {Count} products", updatedProducts.Count);
            return new UpdateResult
            {
                UpdatedProducts = updatedProducts,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred during price update process");
            return new UpdateResult { Success = false };
        }
    }
}
