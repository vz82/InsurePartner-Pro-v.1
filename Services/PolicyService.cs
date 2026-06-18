using Dapper;
using InsurancePartnerApp.Data;
using InsurancePartnerApp.Models;

namespace InsurancePartnerApp.Services;

public class PolicyService : IPolicyService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PolicyService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Policy>> GetPoliciesByPartnerIdAsync(int partnerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT 
                id,
                partner_id as PartnerId,
                shelf_number as ShelfNumber,
                policy_amount as PolicyAmount,
                created_at_utc as CreatedAtUtc
            FROM policies
            WHERE partner_id = @PartnerId
            ORDER BY created_at_utc DESC
        ";
        
        return await connection.QueryAsync<Policy>(sql, new { PartnerId = partnerId });
    }

    public async Task<int> CreatePolicyAsync(Policy policy)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            INSERT INTO policies 
            (partner_id, shelf_number, policy_amount, created_at_utc)
            VALUES 
            (@PartnerId, @ShelfNumber, @PolicyAmount, @CreatedAtUtc)
            RETURNING id
        ";
        
        policy.CreatedAtUtc = DateTime.UtcNow;
        return await connection.QuerySingleAsync<int>(sql, policy);
    }
}