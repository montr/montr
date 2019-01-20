-- Table: public.audit_log

-- DROP TABLE public.audit_log;
-- SELECT * FROM information_schema.tables WHERE table_name = 'audit_log'

CREATE TABLE IF NOT EXISTS public.audit_log
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(),
	entity_type_code character varying(32) NOT NULL COLLATE pg_catalog."default",
	entity_uid uuid NOT NULL,
	company_uid uuid NOT NULL,
	user_uid uuid NOT NULL,
	created_at_utc timestamp with time zone NOT NULL,
	message_code character varying(512) NOT NULL COLLATE pg_catalog."default",
	message_params text COLLATE pg_catalog."default",
	ip character varying(45) COLLATE pg_catalog."default",
	browser text COLLATE pg_catalog."default",
	CONSTRAINT audit_log_pk PRIMARY KEY (uid)
)
WITH (
	OIDS = FALSE
)
