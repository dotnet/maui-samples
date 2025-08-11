namespace BillingService.Models;

public class PurchaseResult
{
    public bool IsSuccess { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
