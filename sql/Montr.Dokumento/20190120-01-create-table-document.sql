-- Table: public.document

-- DROP TABLE public.document;
-- SELECT * FROM information_schema.tables WHERE table_name = 'document'

CREATE TABLE IF NOT EXISTS public.document
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(),
	config_code character varying(64) NOT NULL COLLATE pg_catalog."default",
	status_code character varying(16) NOT NULL COLLATE pg_catalog."default",
	company_uid uuid NOT NULL,
	CONSTRAINT document_pk PRIMARY KEY (uid)
)
WITH (
	OIDS = FALSE
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.document TO web;
