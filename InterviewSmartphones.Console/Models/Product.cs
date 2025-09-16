using System.Text.Json.Serialization;

internal class ProductsResponse
{
    [JsonPropertyName("products")]
    public List<ProductResponse> Products { get; set; } = [];

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

internal class ProductResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("brand")]
    public string Brand { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
}

internal class ProductsResult
{
    public IEnumerable<ProductResponse> Products { get; set; } = [];
    public bool Success { get; set; }
}
