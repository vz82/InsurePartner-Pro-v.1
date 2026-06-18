namespace InsurancePartnerApp.Models;

public class Policy
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public required string ShelfNumber { get; set; } // min 10, max 15
    public required decimal PolicyAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}