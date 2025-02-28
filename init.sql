CREATE TABLE IF NOT EXISTS users (
    user_id SERIAL PRIMARY KEY,
    telegram_user_name VARCHAR(32) UNIQUE,
    refresh_token VARCHAR(256),
    refresh_token_last_updated DATE
);

DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE tablename = 'users' AND indexname = 'idx_telegram_user_name') THEN
        CREATE UNIQUE INDEX idx_telegram_user_name ON users(telegram_user_name);
    END IF;
END $$;