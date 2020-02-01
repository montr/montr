-- Table: montr.document

-- DROP TABLE montr.document;
-- SELECT * FROM information_schema.tables WHERE table_name = 'document'

CREATE TABLE IF NOT EXISTS montr.document
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(),
	config_code character varying(64) NOT NULL,
	status_code character varying(16) NOT NULL,
	company_uid uuid NOT NULL,
	CONSTRAINT document_pk PRIMARY KEY (uid)
);
