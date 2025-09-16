using System.Text.Json.Serialization;

namespace InterviewSmartphones.Models;

public class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("brand")]
    public string Brand { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}
