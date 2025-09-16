using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Serilog;
using InterviewSmartphones.Console.Services;
using InterviewSmartphones.Console.Models;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Configure Serilog from configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

const string baseUrl = "https://dummyjson.com";

using var client = new HttpClient();

Console.WriteLine("Enter username:");
var username = Console.ReadLine() ?? string.Empty;
Console.WriteLine("Enter password:");
var password = Console.ReadLine() ?? string.Empty;

var authenticationClient = new HttpAuthenticationClient(client, baseUrl, Log.Logger);

Log.Information("Application starting...");

var loginResult = await authenticationClient.AuthenticateAsync(username, password);

if (!loginResult.Success)
{
    Log.Error("Authentication failed. Exiting application.");
    Console.WriteLine("Authentication failed. Exiting application.");
    return;
}

Log.Information("Authentication successful");
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

var productClient = new HttpProductClient(client, baseUrl, Log.Logger);
var productService = new ProductService(productClient, Log.Logger);

var getProductsResult = await productService.GetMostExpensiveProducts("smartphones", 3);

if (!getProductsResult.Success)
{
    Log.Error("Failed to retrieve products or no products available.");
    Console.WriteLine("Failed to retrieve products or no products available.");
    return;
}

Console.WriteLine("\nTop 3 Expensive Products:");
Console.WriteLine(new string('-', 20));
foreach (var product in getProductsResult.Products)
{
    product.PrintDetails();
    Console.WriteLine(new string('-', 20));
}

// Get percentage from user
Console.WriteLine("\nEnter percentage to increase prices:");
var percentStr = Console.ReadLine();
if (!decimal.TryParse(percentStr, out decimal percent))
{
    Log.Warning("Invalid percentage entered: {Percentage}", percentStr);
    Console.WriteLine("Invalid percentage.");
    return;
}

var updateResult = await productService.UpdateProductPrices(getProductsResult.Products, percent);

if (!updateResult.Success)
{
    Log.Error("Failed to update product prices.");
    Console.WriteLine("Failed to update product prices.");
    return;
}

// Display updated products
Console.WriteLine(new string('-', 20));
foreach (var product in updateResult.UpdatedProducts)
{
    product.PrintDetails();
    Console.WriteLine(new string('-', 20));
}

Log.Information("Application completed successfully");
Console.ReadKey();
