using Microsoft.Data.Sqlite;

namespace InsurancePartnerApp.Data;

public interface IDbConnectionFactory
{
    SqliteConnection CreateConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
}