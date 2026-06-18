using Dapper;
using InsurancePartnerApp.Data;
using InsurancePartnerApp.Models;

namespace InsurancePartnerApp.Services;

public class PartnerService : IPartnerService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PartnerService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Partner>> GetAllPartnersAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT 
                p.id,
                p.first_name as FirstName,
                p.last_name as LastName,
                p.address as Address,
                p.partner_number as PartnerNumber,
                p.croatian_pin as CroatianPIN,
                p.partner_type_id as PartnerTypeId,
                p.created_at_utc as CreatedAtUtc,
                p.created_by_user as CreatedByUser,
                p.is_foreign as IsForeign,
                p.external_code as ExternalCode,
                p.gender as Gender,
                COALESCE(COUNT(pol.id), 0) as PolicyCount,
                COALESCE(SUM(pol.policy_amount), 0) as TotalPolicyAmount
            FROM partners p
            LEFT JOIN policies pol ON p.id = pol.partner_id
            GROUP BY p.id
            ORDER BY p.created_at_utc DESC
        ";
        
        return await connection.QueryAsync<Partner>(sql);
    }

    public async Task<PartnerDetail?> GetPartnerByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT 
                p.id,
                p.first_name as FirstName,
                p.last_name as LastName,
                p.address as Address,
                p.partner_number as PartnerNumber,
                p.croatian_pin as CroatianPIN,
                p.partner_type_id as PartnerTypeId,
                CASE WHEN p.partner_type_id = 1 THEN 'Personal' ELSE 'Legal' END as PartnerTypeName,
                p.created_at_utc as CreatedAtUtc,
                p.created_by_user as CreatedByUser,
                p.is_foreign as IsForeign,
                p.external_code as ExternalCode,
                p.gender as Gender,
                COALESCE(COUNT(pol.id), 0) as PolicyCount,
                COALESCE(SUM(pol.policy_amount), 0) as TotalPolicyAmount
            FROM partners p
            LEFT JOIN policies pol ON p.id = pol.partner_id
            WHERE p.id = @Id
            GROUP BY p.id
        ";
        
        return await connection.QueryFirstOrDefaultAsync<PartnerDetail>(sql, new { Id = id });
    }

    public async Task<int> CreatePartnerAsync(Partner partner)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            INSERT INTO partners 
            (first_name, last_name, address, partner_number, croatian_pin, partner_type_id, created_at_utc, created_by_user, is_foreign, external_code, gender)
            VALUES 
            (@FirstName, @LastName, @Address, @PartnerNumber, @CroatianPIN, @PartnerTypeId, @CreatedAtUtc, @CreatedByUser, @IsForeign, @ExternalCode, @Gender)
            RETURNING id
        ";
        
        partner.CreatedAtUtc = DateTime.UtcNow;
        return await connection.QuerySingleAsync<int>(sql, partner);
    }

    public async Task<bool> UpdatePartnerAsync(Partner partner)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            UPDATE partners SET
                first_name = @FirstName,
                last_name = @LastName,
                address = @Address,
                partner_number = @PartnerNumber,
                croatian_pin = @CroatianPIN,
                partner_type_id = @PartnerTypeId,
                created_by_user = @CreatedByUser,
                is_foreign = @IsForeign,
                external_code = @ExternalCode,
                gender = @Gender
            WHERE id = @Id
        ";
        
        return await connection.ExecuteAsync(sql, partner) > 0;
    }

    public async Task<bool> ExternalCodeExistsAsync(string externalCode, int? excludeId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(1) FROM partners WHERE external_code = @Code";
        
        if (excludeId.HasValue)
            sql += " AND id != @ExcludeId";
        
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Code = externalCode, ExcludeId = excludeId });
        return count > 0;
    }

    public async Task<bool> PartnerNumberExistsAsync(string partnerNumber, int? excludeId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(1) FROM partners WHERE partner_number = @PartnerNumber";
        
        if (excludeId.HasValue)
            sql += " AND id != @ExcludeId";
        
        var count = await connection.ExecuteScalarAsync<int>(sql, new { PartnerNumber = partnerNumber, ExcludeId = excludeId });
        return count > 0;
    }
}