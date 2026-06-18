using Microsoft.Data.Sqlite;

namespace InsurancePartnerApp.Data;

public class DbInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DbInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Initialize()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            -- Create partners table
            CREATE TABLE IF NOT EXISTS partners (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                first_name TEXT NOT NULL,
                last_name TEXT NOT NULL,
                address TEXT,
                partner_number TEXT NOT NULL,
                croatian_pin TEXT,
                partner_type_id INTEGER NOT NULL CHECK (partner_type_id IN (1, 2)),
                created_at_utc TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                created_by_user TEXT NOT NULL,
                is_foreign BOOLEAN NOT NULL,
                external_code TEXT NOT NULL UNIQUE CHECK (LENGTH(external_code) >= 10 AND LENGTH(external_code) <= 20),
                gender TEXT NOT NULL CHECK (gender IN ('M', 'F', 'N')),
                UNIQUE(partner_number, partner_type_id)
            );

            -- Create policies table
            CREATE TABLE IF NOT EXISTS policies (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                partner_id INTEGER NOT NULL REFERENCES partners(id) ON DELETE CASCADE,
                shelf_number TEXT NOT NULL CHECK (LENGTH(shelf_number) >= 10 AND LENGTH(shelf_number) <= 15),
                policy_amount NUMERIC(18, 2) NOT NULL,
                created_at_utc TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            );

            -- Create indices for better query performance
            CREATE INDEX IF NOT EXISTS idx_partners_created_at_utc ON partners(created_at_utc DESC);
            CREATE INDEX IF NOT EXISTS idx_partners_external_code ON partners(external_code);
            CREATE INDEX IF NOT EXISTS idx_partners_partner_number ON partners(partner_number);
            CREATE INDEX IF NOT EXISTS idx_policies_partner_id ON policies(partner_id);
            CREATE INDEX IF NOT EXISTS idx_policies_created_at_utc ON policies(created_at_utc DESC);
        ";
        cmd.ExecuteNonQuery();
    }
}
                created_at_utc TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'UTC'
            );

            -- Create indices
            CREATE INDEX IF NOT EXISTS idx_partners_created_at_utc ON partners(created_at_utc DESC);
            CREATE INDEX IF NOT EXISTS idx_policies_partner_id ON policies(partner_id);
        ";
        cmd.ExecuteNonQuery();
        connection.Close();
    }
}