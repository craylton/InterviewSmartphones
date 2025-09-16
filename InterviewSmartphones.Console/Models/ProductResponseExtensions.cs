namespace InterviewSmartphones.Console.Models;

public static class ProductResponseExtensions
{
    public static void PrintDetails(this ProductResponse product)
    {
        System.Console.WriteLine($"Brand: {product.Brand}");
        System.Console.WriteLine($"Title: {product.Title}");
        System.Console.WriteLine($"Price: ${product.Price}");
    }
}
