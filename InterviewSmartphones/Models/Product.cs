namespace InterviewSmartphones.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double DiscountPercentage { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Brand { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Weight { get; set; }
        public Dimensions Dimensions { get; set; } = new();
        public string WarrantyInformation { get; set; } = string.Empty;
        public string ShippingInformation { get; set; } = string.Empty;
        public string AvailabilityStatus { get; set; } = string.Empty;
        public List<Review> Reviews { get; set; } = new();
        public string ReturnPolicy { get; set; } = string.Empty;
        public int MinimumOrderQuantity { get; set; }
        public Meta Meta { get; set; } = new();
        public List<string> Images { get; set; } = new();
        public string Thumbnail { get; set; } = string.Empty;
    }

    public class Dimensions
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
    }

    public class Review
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string ReviewerEmail { get; set; } = string.Empty;
    }

    public class Meta
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
    }

    public class ProductsResponse
    {
        public List<Product> Products { get; set; } = new();
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }

    public class SmartphoneDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
    }

    public class PriceUpdateRequest
    {
        public decimal PercentageIncrease { get; set; }
    }
}