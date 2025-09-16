using System.Text.Json.Serialization;

namespace InterviewSmartphones.Models;

public class ProductList
{
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = [];
}
