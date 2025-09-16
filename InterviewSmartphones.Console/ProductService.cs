using System.Net.Http.Headers;
using System.Numerics;
using System.Text.Json;

internal class ProductService(HttpClient client, string baseUrl)
{
    private readonly HttpClient _client = client;
    private readonly string _baseUrl = baseUrl;

    public async Task<ProductsResult> GetMostExpensiveProducts(int numberOfProducts)
    {
        Console.WriteLine("\nTop 3 Expensive Smartphones:");
        var productResponse = await _client.GetAsync($"{_baseUrl}/auth/products");
        var productJson = await productResponse.Content.ReadAsStringAsync();

        var productList = JsonSerializer.Deserialize<ProductsResponse>(productJson);

        if (productList?.Products is null || productList.Products.Count == 0)
        {
            Console.WriteLine("No products found.");
            return new ProductsResult { Success = false };
        }

        var mostExpensiveProducts = productList
            .Products
            .OrderByDescending(p => p.Price)
            .Take(numberOfProducts);

        Console.WriteLine(new string('-', 20));

        foreach (var product in mostExpensiveProducts)
        {
            Console.WriteLine($"Brand: {product.Brand}");
            Console.WriteLine($"Title: {product.Title}");
            Console.WriteLine($"Price: ${product.Price}");
            Console.WriteLine(new string('-', 20));
        }

        return new ProductsResult
        {
            Products = mostExpensiveProducts,
            Success = true
        };
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

            Console.WriteLine($"Brand: {updatedProduct.Brand}");
            Console.WriteLine($"Title: {updatedProduct.Title}");
            Console.WriteLine($"Price: ${updatedProduct.Price}");
            Console.WriteLine(new string('-', 20));
        }
    }
}
