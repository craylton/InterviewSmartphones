using Serilog;
using System.Net.Http.Headers;
using System.Text.Json;

internal class ProductService(HttpClient client, string baseUrl, ILogger logger)
{
    private readonly HttpClient _client = client;
    private readonly string _baseUrl = baseUrl;
    private readonly ILogger _logger = logger;

    public async Task<ProductsResult> GetMostExpensiveProducts(string category, int numberOfProducts)
    {
        _logger.Information("Starting to retrieve most expensive products for category: {Category}, count: {Count}", category, numberOfProducts);

        var allProducts = await GetAllProducts();

        if (allProducts.Count == 0)
        {
            _logger.Warning("No products found");
            Console.WriteLine("No products found.");
            return new ProductsResult { Success = false };
        }

        var mostExpensiveProducts = allProducts
            .Where(p => p.Category == category)
            .OrderByDescending(p => p.Price)
            .Take(numberOfProducts);

        var productsList = mostExpensiveProducts.ToList();
        _logger.Information("Found {Count} products in category {Category}", productsList.Count, category);

        Console.WriteLine("\nTop 3 Expensive Products:");
        Console.WriteLine(new string('-', 20));
        foreach (var product in productsList)
        {
            product.PrintDetails();
            Console.WriteLine(new string('-', 20));
        }

        return new ProductsResult
        {
            Products = productsList,
            Success = true
        };
    }

    private async Task<List<ProductResponse>> GetAllProducts()
    {
        var allProducts = new List<ProductResponse>();
        const int limit = 30; // API limit per request
        int skip = 0;

        try
        {
            // First request to get the total count
            var firstResponse = await _client.GetAsync($"{_baseUrl}/auth/products?select=title,price,brand,category&limit={limit}&skip={skip}");
            var firstJson = await firstResponse.Content.ReadAsStringAsync();
            var firstResult = JsonSerializer.Deserialize<ProductsResponse>(firstJson);

            if (firstResult?.Products is null)
            {
                _logger.Warning("First request returned null products");
                return allProducts;
            }

            int total = firstResult.Total;
            allProducts.AddRange(firstResult.Products);
            _logger.Information("Initial fetch: {Count} products retrieved, total available: {Total}", firstResult.Products.Count, total);

            // Continue fetching remaining products if there are more
            while (allProducts.Count < total)
            {
                skip += limit;

                var response = await _client.GetAsync($"{_baseUrl}/auth/products?select=title,price,brand,category&limit={limit}&skip={skip}");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ProductsResponse>(json);

                if (result?.Products is null || result.Products.Count == 0)
                {
                    _logger.Warning("No more products returned in batch, breaking loop");
                    break;
                }

                allProducts.AddRange(result.Products);
            }

            _logger.Information("Completed fetching all products. Total retrieved: {Count}", allProducts.Count);
            return allProducts;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while fetching products");
            return allProducts;
        }
    }

    public async Task UpdateProductPrices(IEnumerable<ProductResponse> products)
    {
        _logger.Information("Starting price update process");

        Console.WriteLine("\nEnter percentage to increase prices:");
        var percentStr = Console.ReadLine();
        if (!decimal.TryParse(percentStr, out decimal percent))
        {
            _logger.Warning("Invalid percentage entered: {Percentage}", percentStr);
            Console.WriteLine("Invalid percentage.");
            return;
        }

        _logger.Information("Updating prices with {Percentage}% increase", percent);
        Console.WriteLine(new string('-', 20));

        var productsList = products.ToList();

        foreach (var product in productsList)
        {
            decimal newPrice = Math.Round(product.Price * (1 + percent / 100), 2);

            var updatePayload = new ProductUpdate { Price = newPrice };

            var updateContent = new StringContent(JsonSerializer.Serialize(updatePayload));
            updateContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                var updateResponse = await _client.PutAsync($"{_baseUrl}/products/{product.Id}", updateContent);
                var updateJson = await updateResponse.Content.ReadAsStringAsync();

                var updatedProduct = JsonSerializer.Deserialize<ProductResponse>(updateJson);

                if (updatedProduct is null)
                {
                    _logger.Warning("Failed to update product ID {ProductId} - null response", product.Id);
                    Console.WriteLine($"Failed to update product ID {product.Id}");
                    continue;
                }

                _logger.Information("Successfully updated product ID {ProductId} price to {NewPrice}", product.Id, updatedProduct.Price);
                updatedProduct.PrintDetails();
                Console.WriteLine(new string('-', 20));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred while updating product ID {ProductId}", product.Id);
                Console.WriteLine($"Error updating product ID {product.Id}");
            }
        }

        _logger.Information("Price update completed.");
    }
}
