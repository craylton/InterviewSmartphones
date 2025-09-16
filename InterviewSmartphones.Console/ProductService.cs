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
}
