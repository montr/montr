-- Table: montr.company

-- DROP TABLE montr.company;
-- SELECT * FROM information_schema.tables WHERE table_name = 'company'

CREATE TABLE IF NOT EXISTS montr.company
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(),
	config_code character varying(16) NOT NULL,
	status_code character varying(16) NOT NULL,
	name character varying(2048) NOT NULL,
	CONSTRAINT company_pk PRIMARY KEY (uid)
);
