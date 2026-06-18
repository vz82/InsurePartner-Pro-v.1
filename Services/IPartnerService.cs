using InsurancePartnerApp.Models;

namespace InsurancePartnerApp.Services;

public interface IPartnerService
{
    Task<IEnumerable<Partner>> GetAllPartnersAsync();
    Task<PartnerDetail?> GetPartnerByIdAsync(int id);
    Task<int> CreatePartnerAsync(Partner partner);
    Task<bool> UpdatePartnerAsync(Partner partner);
    Task<bool> ExternalCodeExistsAsync(string externalCode, int? excludeId = null);
    Task<bool> PartnerNumberExistsAsync(string partnerNumber, int? excludeId = null);
}