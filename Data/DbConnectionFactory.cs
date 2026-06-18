using Npgsql;

namespace InsurancePartnerApp.Data;

public interface IDbConnectionFactory
{
    NpgsqlConnection CreateConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
}