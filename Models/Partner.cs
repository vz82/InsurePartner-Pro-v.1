namespace InsurancePartnerApp.Models;

public class Partner
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Address { get; set; }
    public required string PartnerNumber { get; set; } // Exactly 20 digits
    public string? CroatianPIN { get; set; } // OIB
    public required int PartnerTypeId { get; set; } // 1=Personal, 2=Legal
    public DateTime CreatedAtUtc { get; set; }
    public required string CreatedByUser { get; set; }
    public required bool IsForeign { get; set; }
    public required string ExternalCode { get; set; } // min 10, max 20, unique
    public required char Gender { get; set; } // M, F, N
    public string FullName => $"{FirstName} {LastName}";
    public int PolicyCount { get; set; }
    public decimal TotalPolicyAmount { get; set; }
    public bool HasHighRisk => PolicyCount > 5 || TotalPolicyAmount > 5000m;
}

public class PartnerDetail
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Address { get; set; }
    public required string PartnerNumber { get; set; }
    public string? CroatianPIN { get; set; }
    public required int PartnerTypeId { get; set; }
    public required string PartnerTypeName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public required string CreatedByUser { get; set; }
    public required bool IsForeign { get; set; }
    public required string ExternalCode { get; set; }
    public required char Gender { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public int PolicyCount { get; set; }
    public decimal TotalPolicyAmount { get; set; }
    public bool HasHighRisk => PolicyCount > 5 || TotalPolicyAmount > 5000m;
}