
using System.Text.Json.Serialization;

internal class ProductUpdate
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}