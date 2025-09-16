using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Serilog;
using InterviewSmartphones.Console.Services;

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

// Create dependencies for LoginService
var credentialProvider = new ConsoleCredentialProvider();
var authenticationClient = new HttpAuthenticationClient(client, baseUrl, Log.Logger);
var loginService = new LoginService(credentialProvider, authenticationClient, Log.Logger);

Log.Information("Application starting...");

var loginResult = await loginService.AuthenticateAsync();

if (!loginResult.Success)
{
    Log.Error("Authentication failed. Exiting application.");
    Console.WriteLine("Authentication failed. Exiting application.");
    return;
}

Log.Information("Authentication successful");
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

var productService = new ProductService(client, baseUrl, Log.Logger);

var getProductsResult = await productService.GetMostExpensiveProducts("smartphones", 3);

if (!getProductsResult.Success)
{
    Log.Error("Failed to retrieve products or no products available.");
    Console.WriteLine("Failed to retrieve products or no products available.");
    return;
}

await productService.UpdateProductPrices(getProductsResult.Products);

Log.Information("Application completed successfully");
Console.ReadKey();
