using System.Net.Http.Headers;
using System.Text.Json;
using InterviewSmartphones.Console.Models;
using Serilog;

namespace InterviewSmartphones.Console.Services;

public class HttpProductClient(HttpClient client, string baseUrl, ILogger logger) : IProductClient
{
    public async Task<List<ProductResponse>> GetAllProductsAsync()
    {
        var allProducts = new List<ProductResponse>();
        const int limit = 30; // API limit per request
        int skip = 0;

        try
        {
            // First request to get the total count
            var firstResponse = await client.GetAsync($"{baseUrl}/auth/products?select=title,price,brand,category&limit={limit}&skip={skip}");

            if (!firstResponse.IsSuccessStatusCode)
            {
                logger.Warning("First request failed with status code: {StatusCode}", firstResponse.StatusCode);
                throw new HttpRequestException($"HTTP request failed with status code: {firstResponse.StatusCode}");
            }

            var firstJson = await firstResponse.Content.ReadAsStringAsync();
            var firstResult = JsonSerializer.Deserialize<ProductsResponse>(firstJson);

            if (firstResult?.Products is null)
            {
                logger.Warning("First request returned null products");
                return allProducts;
            }

            int total = firstResult.Total;
            allProducts.AddRange(firstResult.Products);
            logger.Information("Initial fetch: {Count} products retrieved, total available: {Total}", firstResult.Products.Count, total);

            // Continue fetching remaining products if there are more
            while (allProducts.Count < total)
            {
                skip += limit;

                var response = await client.GetAsync($"{baseUrl}/auth/products?select=title,price,brand,category&limit={limit}&skip={skip}");

                if (!response.IsSuccessStatusCode)
                {
                    logger.Warning("Subsequent request failed with status code: {StatusCode}", response.StatusCode);
                    throw new HttpRequestException($"HTTP request failed with status code: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ProductsResponse>(json);

                if (result?.Products is null || result.Products.Count == 0)
                {
                    logger.Warning("No more products returned in batch, breaking loop");
                    break;
                }

                allProducts.AddRange(result.Products);
            }

            logger.Information("Completed fetching all products. Total retrieved: {Count}", allProducts.Count);
            return allProducts;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error occurred while fetching products");
            throw;
        }
    }

    public async Task<ProductResponse> UpdateProductPriceAsync(int productId, decimal newPrice)
    {
        var updatePayload = new ProductUpdate { Price = newPrice };

        var updateContent = new StringContent(JsonSerializer.Serialize(updatePayload));
        updateContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        try
        {
            var updateResponse = await client.PutAsync($"{baseUrl}/products/{productId}", updateContent);
            var updateJson = await updateResponse.Content.ReadAsStringAsync();

            var updatedProduct = JsonSerializer.Deserialize<ProductResponse>(updateJson);

            if (updatedProduct is null)
            {
                logger.Warning("Update response returned null for product ID {ProductId}", productId);
                throw new Exception("Failed to deserialize updated product");
            }
            logger.Information("Successfully updated product ID {ProductId} to new price {NewPrice}", productId, updatedProduct.Price);
            return updatedProduct;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Exception occurred while updating product ID {ProductId}", productId);
            throw;
        }
    }
}