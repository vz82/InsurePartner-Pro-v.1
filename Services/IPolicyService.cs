using InsurancePartnerApp.Models;

namespace InsurancePartnerApp.Services;

public interface IPolicyService
{
    Task<IEnumerable<Policy>> GetPoliciesByPartnerIdAsync(int partnerId);
    Task<int> CreatePolicyAsync(Policy policy);
}