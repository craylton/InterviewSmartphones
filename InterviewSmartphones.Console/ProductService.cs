using System.Net.Http.Headers;
using System.Text.Json;

internal class ProductService(HttpClient client, string baseUrl)
{
    private readonly HttpClient _client = client;
    private readonly string _baseUrl = baseUrl;

    public async Task<ProductsResult> GetMostExpensiveProducts(int numberOfProducts)
    {
        var allProducts = await GetAllProducts();

        if (allProducts.Count == 0)
        {
            Console.WriteLine("No products found.");
            return new ProductsResult { Success = false };
        }

        var mostExpensiveProducts = allProducts
            .Where(p => p.Category == "Smartphones")
            .OrderByDescending(p => p.Price)
            .Take(numberOfProducts);

        Console.WriteLine("\nTop 3 Expensive Products:");
        Console.WriteLine(new string('-', 20));
        foreach (var product in mostExpensiveProducts)
        {
            product.PrintDetails();
            Console.WriteLine(new string('-', 20));
        }

        return new ProductsResult
        {
            Products = mostExpensiveProducts,
            Success = true
        };
    }

    private async Task<List<ProductResponse>> GetAllProducts()
    {
        var allProducts = new List<ProductResponse>();
        const int limit = 30; // API limit per request
        int skip = 0;

        // First request to get the total count
        var firstResponse = await _client.GetAsync($"{_baseUrl}/auth/products?select=title,price,brand,category&limit={limit}&skip={skip}");
        var firstJson = await firstResponse.Content.ReadAsStringAsync();
        var firstResult = JsonSerializer.Deserialize<ProductsResponse>(firstJson);

        if (firstResult?.Products is null)
        {
            return allProducts;
        }

        int total = firstResult.Total;
        allProducts.AddRange(firstResult.Products);

        // Continue fetching remaining products if there are more
        while (allProducts.Count < total)
        {
            skip += limit;
            var response = await _client.GetAsync($"{_baseUrl}/auth/products?select=title,price,brand,category&limit={limit}&skip={skip}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProductsResponse>(json);

            if (result?.Products is null || result.Products.Count == 0)
                break;

            allProducts.AddRange(result.Products);
        }

        return allProducts;
    }

    public async Task UpdateProductPrices(IEnumerable<ProductResponse> products)
    {
        Console.WriteLine("\nEnter percentage to increase prices:");
        var percentStr = Console.ReadLine();
        if (!decimal.TryParse(percentStr, out decimal percent))
        {
            Console.WriteLine("Invalid percentage.");
            return;
        }

        Console.WriteLine(new string('-', 20));

        foreach (var product in products)
        {
            decimal newPrice = Math.Round(product.Price * (1 + percent / 100), 2);

            var updatePayload = new ProductUpdate { Price = newPrice };

            var updateContent = new StringContent(JsonSerializer.Serialize(updatePayload));
            updateContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var updateResponse = await _client.PutAsync($"{_baseUrl}/products/{product.Id}", updateContent);
            var updateJson = await updateResponse.Content.ReadAsStringAsync();

            var updatedProduct = JsonSerializer.Deserialize<ProductResponse>(updateJson);

            if (updatedProduct is null)
            {
                Console.WriteLine($"Failed to update product ID {product.Id}");
                continue;
            }

            updatedProduct.PrintDetails();
            Console.WriteLine(new string('-', 20));
        }
    }
}
