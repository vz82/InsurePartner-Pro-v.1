using Npgsql;

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
            -- Create partner_type enum if not exists
            DO $$ BEGIN
                CREATE TYPE partner_type AS ENUM ('Personal', 'Legal');
            EXCEPTION
                WHEN duplicate_object THEN null;
            END $$;

            -- Create partners table
            CREATE TABLE IF NOT EXISTS partners (
                id SERIAL PRIMARY KEY,
                first_name VARCHAR(255) NOT NULL,
                last_name VARCHAR(255) NOT NULL,
                address VARCHAR(255),
                partner_number CHAR(20) NOT NULL,
                croatian_pin VARCHAR(50),
                partner_type_id INT NOT NULL CHECK (partner_type_id IN (1, 2)),
                created_at_utc TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'UTC',
                created_by_user VARCHAR(255) NOT NULL,
                is_foreign BOOLEAN NOT NULL,
                external_code VARCHAR(20) NOT NULL UNIQUE CHECK (char_length(external_code) >= 10),
                gender CHAR(1) NOT NULL CHECK (gender IN ('M', 'F', 'N')),
                CONSTRAINT partner_number_unique UNIQUE (partner_number, partner_type_id)
            );

            -- Create policies table
            CREATE TABLE IF NOT EXISTS policies (
                id SERIAL PRIMARY KEY,
                partner_id INT NOT NULL REFERENCES partners(id) ON DELETE CASCADE,
                shelf_number VARCHAR(15) NOT NULL CHECK (char_length(shelf_number) >= 10),
                policy_amount NUMERIC(18, 2) NOT NULL,
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