DO $$
BEGIN
    IF NOT EXISTS (
        SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'montr'
      )
    THEN
      EXECUTE 'CREATE SCHEMA "montr";';
    END IF;
END
$$;

DO $$
BEGIN
	IF NOT EXISTS (
		SELECT * FROM pg_tables WHERE schemaname = 'montr' and tablename = 'migration'
	)
	then
		CREATE TABLE montr.migration
		(
			id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
			file_name character varying(500) NOT NULL,
			hash character(40) NOT NULL,
			executed_at_utc timestamp with time zone,
			duration_ms bigint,
			PRIMARY KEY (id),
			CONSTRAINT ux_file_name UNIQUE (file_name),
			CONSTRAINT ux_hash UNIQUE (hash)
		);
	end if;
END
$$

