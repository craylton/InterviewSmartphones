using System.Net.Http.Headers;
using System.Text.Json;
using InterviewSmartphones.Models;

internal class ProductService
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public ProductService(HttpClient client, string baseUrl)
    {
        _client = client;
        _baseUrl = baseUrl;
    }

    public async Task ProcessProductsAsync()
    {
        var topThreeMostExpensiveProducts = await GetTopThreeMostExpensiveProducts();

        foreach (var p in topThreeMostExpensiveProducts)
        {
            Console.WriteLine($"{p.Brand} - {p.Title} - ${p.Price}");
        }

        await UpdateTopThreeMostExpensiveProducts(topThreeMostExpensiveProducts);
    }

    public async Task<List<Product>> GetTopThreeMostExpensiveProducts()
    {
        Console.WriteLine("\nTop 3 Expensive Smartphones:");
        Log("Fetching products...");
        var productResponse = await _client.GetAsync($"{_baseUrl}/auth/products");
        var productJson = await productResponse.Content.ReadAsStringAsync();
        Log("Product response: " + productResponse.StatusCode);

        var productList = JsonSerializer.Deserialize<ProductList>(productJson);
        var topThreeMostExpensiveProducts = productList?.Products.OrderByDescending(p => p.Price).Take(3).ToList();

        return topThreeMostExpensiveProducts;
    }


    public async Task UpdateTopThreeMostExpensiveProducts(List<Product> topThreeMostExpensiveProducts)
    {
        Console.WriteLine("\nEnter percentage to increase prices:");
        var percentStr = Console.ReadLine();
        if (!decimal.TryParse(percentStr, out decimal percent))
        {
            Console.WriteLine("Invalid percentage.");
            return;
        }

        foreach (var phone in topThreeMostExpensiveProducts)
        {
            var newPrice = phone.Price * (1 + percent / 100);
            var updatePayload = new ProductUpdate { Price = newPrice };

            var updateContent = new StringContent(JsonSerializer.Serialize(updatePayload));
            updateContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var updateResponse = await _client.PutAsync($"{_baseUrl}/products/{phone.Id}", updateContent);
            var updateJson = await updateResponse.Content.ReadAsStringAsync();

            Log($"Updated {phone.Title} - New Price: ${newPrice} - Status: {updateResponse.StatusCode}");
        }

        Console.WriteLine("\nUpdated Smartphone Prices:");
        foreach (var phone in topThreeMostExpensiveProducts)
        {
            Console.WriteLine($"{phone.Brand} - {phone.Title} - ${phone.Price * (1 + percent / 100)}");
        }
    }

    private void Log(string message)
    {
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        Console.WriteLine(logMessage);
        //The log gets saved in bin\Debug\net6.0
        File.AppendAllText("log.txt", logMessage + Environment.NewLine);
    }
}