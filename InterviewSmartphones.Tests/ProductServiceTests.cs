using InterviewSmartphones.Console.Models;
using InterviewSmartphones.Console.Services;
using Moq;
using Serilog;

namespace InterviewSmartphones.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductClient> _productClientMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productClientMock = new Mock<IProductClient>();
        _loggerMock = new Mock<ILogger>();
        _productService = new ProductService(_productClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetMostExpensiveProducts_WhenProductClientThrowsException_ReturnsSuccessFalse()
    {
        // Arrange
        const string category = "smartphones";
        const int numberOfProducts = 3;

        _productClientMock.Setup(x => x.GetAllProductsAsync())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _productService.GetMostExpensiveProducts(category, numberOfProducts);

        // Assert
        Assert.False(result.Success);
        Assert.Empty(result.Products);
        _productClientMock.Verify(x => x.GetAllProductsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetMostExpensiveProducts_WhenNoProductsFound_ReturnsSuccessFalse()
    {
        // Arrange
        const string category = "smartphones";
        const int numberOfProducts = 3;
        var emptyProductList = new List<ProductResponse>();

        _productClientMock.Setup(x => x.GetAllProductsAsync())
            .ReturnsAsync(emptyProductList);

        // Act
        var result = await _productService.GetMostExpensiveProducts(category, numberOfProducts);

        // Assert
        Assert.False(result.Success);
        Assert.Empty(result.Products);
        _productClientMock.Verify(x => x.GetAllProductsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetMostExpensiveProducts_WhenProductsFoundSuccessfully_ReturnsSuccessTrue()
    {
        // Arrange
        const string category = "smartphones";
        const int numberOfProducts = 2;
        var mockProducts = new List<ProductResponse>
        {
            new() { Id = 1, Title = "iPhone", Price = 999.99m, Brand = "Apple", Category = "smartphones" },
            new() { Id = 2, Title = "Samsung Galaxy", Price = 899.99m, Brand = "Samsung", Category = "smartphones" },
            new() { Id = 3, Title = "Google Pixel", Price = 799.99m, Brand = "Google", Category = "smartphones" },
            new() { Id = 4, Title = "Laptop", Price = 1299.99m, Brand = "Dell", Category = "laptops" }
        };

        _productClientMock.Setup(x => x.GetAllProductsAsync())
            .ReturnsAsync(mockProducts);

        // Act
        var result = await _productService.GetMostExpensiveProducts(category, numberOfProducts);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Products.Count());

        var productsList = result.Products.ToList();
        Assert.Equal("iPhone", productsList[0].Title); // Most expensive smartphone
        Assert.Equal(999.99m, productsList[0].Price);
        Assert.Equal("Samsung Galaxy", productsList[1].Title); // Second most expensive smartphone
        Assert.Equal(899.99m, productsList[1].Price);

        _productClientMock.Verify(x => x.GetAllProductsAsync(), Times.Once);
    }
}