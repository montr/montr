-- Table: montr.audit_log

-- DROP TABLE montr.audit_log;
-- SELECT * FROM information_schema.tables WHERE table_name = 'audit_log'

CREATE TABLE IF NOT EXISTS montr.audit_log
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(),
	entity_type_code character varying(32) NOT NULL,
	entity_uid uuid NOT NULL,
	company_uid uuid NOT NULL,
	user_uid uuid NOT NULL,
	created_at_utc timestamp with time zone NOT NULL,
	message_code character varying(512) NOT NULL,
	message_params text,
	ip character varying(45),
	browser text,
	CONSTRAINT audit_log_pk PRIMARY KEY (uid)
);
