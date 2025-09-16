using System.Collections.Generic;
using System.Net.Http.Headers;

const string baseUrl = "https://dummyjson.com";

using var client = new HttpClient();
var loginService = new LoginService(client, baseUrl);

var loginResult = await loginService.AuthenticateAsync();

if (!loginResult.Success)
{
    Console.WriteLine("Authentication failed. Exiting application.");
    return;
}

client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

//var a = await client.GetAsync($"{baseUrl}/auth/products/categories");
//var b = await a.Content.ReadAsStringAsync();
var productService = new ProductService(client, baseUrl);

var getProductsResult = await productService.GetMostExpensiveProducts(3);

if (!getProductsResult.Success)
{
    Console.WriteLine("Failed to retrieve products or no products available.");
    return;
}

await productService.UpdateProductPrices(getProductsResult.Products);

Console.ReadKey();