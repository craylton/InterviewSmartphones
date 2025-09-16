using System.Text.Json.Serialization;

namespace InterviewSmartphones.Models;

public class ProductUpdate
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}
