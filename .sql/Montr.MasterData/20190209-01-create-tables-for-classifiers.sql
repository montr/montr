-- Table: public.company

-- DROP TABLE public.classifier;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier'

CREATE TABLE IF NOT EXISTS public.classifier
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(), 
	company_uid uuid NOT NULL,
	config_code character varying(64) NOT NULL COLLATE pg_catalog."default",
    status_code character varying(16) NOT NULL COLLATE pg_catalog."default", 
    code character varying(32) NOT NULL COLLATE pg_catalog."default", 
    name character varying(2048) COLLATE pg_catalog."default", 
	CONSTRAINT classifier_pk PRIMARY KEY (uid)
)
WITH (
	OIDS = FALSE
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.classifier TO web;

ALTER TABLE public.classifier
    ADD CONSTRAINT fk_classifier_coompany_uid FOREIGN KEY (company_uid)
    REFERENCES public.company (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
    