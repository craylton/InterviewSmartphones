using InterviewSmartphones.Console.Models;

namespace InterviewSmartphones.Console;

public static class ProductResponseExtensions
{
    public static void PrintDetails(this IEnumerable<ProductResponse> products)
    {
        System.Console.WriteLine(new string('-', 20));
        foreach (var product in products)
        {
            System.Console.WriteLine($"Brand: {product.Brand}");
            System.Console.WriteLine($"Title: {product.Title}");
            System.Console.WriteLine($"Price: ${product.Price}");
            System.Console.WriteLine(new string('-', 20));
        }
    }
}
