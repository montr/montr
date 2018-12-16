-- Table: public.company

-- DROP TABLE public.company;
-- SELECT * FROM information_schema.tables WHERE table_name = 'company'

CREATE TABLE IF NOT EXISTS public.company
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(),
	config_code character varying(16) COLLATE pg_catalog."default",
	status_code character varying(16) COLLATE pg_catalog."default",
	name character varying(2048) COLLATE pg_catalog."default",
	CONSTRAINT company_pk PRIMARY KEY (uid)
)
WITH (
	OIDS = FALSE
)
