using System.Text.Json.Serialization;

namespace InterviewSmartphones.Console.Models;

public class ProductUpdate
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}