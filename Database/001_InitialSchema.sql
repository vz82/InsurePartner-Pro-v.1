-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

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
    CONSTRAINT partner_number_unique UNIQUE (partner_number, partner_type_id),
    CONSTRAINT external_code_length CHECK (char_length(external_code) <= 20)
);

-- Create policies table
CREATE TABLE IF NOT EXISTS policies (
    id SERIAL PRIMARY KEY,
    partner_id INT NOT NULL REFERENCES partners(id) ON DELETE CASCADE,
    shelf_number VARCHAR(15) NOT NULL CHECK (char_length(shelf_number) >= 10),
    policy_amount NUMERIC(18, 2) NOT NULL,
    created_at_utc TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'UTC',
    CONSTRAINT shelf_number_length CHECK (char_length(shelf_number) <= 15)
);

-- Create indices for better query performance
CREATE INDEX IF NOT EXISTS idx_partners_created_at_utc ON partners(created_at_utc DESC);
CREATE INDEX IF NOT EXISTS idx_partners_external_code ON partners(external_code);
CREATE INDEX IF NOT EXISTS idx_partners_partner_number ON partners(partner_number);
CREATE INDEX IF NOT EXISTS idx_policies_partner_id ON policies(partner_id);
CREATE INDEX IF NOT EXISTS idx_policies_created_at_utc ON policies(created_at_utc DESC);
