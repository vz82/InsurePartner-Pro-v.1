using System.Data.SQLite;

namespace InsurancePartnerApp.Data;

public interface IDbConnectionFactory
{
    SQLiteConnection CreateConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SQLiteConnection CreateConnection()
    {
        return new SQLiteConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
}