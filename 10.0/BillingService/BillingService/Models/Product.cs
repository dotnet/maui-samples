namespace BillingService.Models;

public class Product
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public decimal PriceAmount { get; set; }
    public bool IsOwned { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
