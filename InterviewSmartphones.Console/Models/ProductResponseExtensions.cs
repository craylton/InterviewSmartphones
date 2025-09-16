internal static class ProductResponseExtensions
{
    public static void PrintDetails(this ProductResponse product)
    {
        Console.WriteLine($"Brand: {product.Brand}");
        Console.WriteLine($"Title: {product.Title}");
        Console.WriteLine($"Price: ${product.Price}");
    }
}
