-- Table: public.company

-- DROP TABLE public.company_user;
-- SELECT * FROM information_schema.tables WHERE table_name = 'company_user'

CREATE TABLE IF NOT EXISTS public.company_user
(
	company_uid uuid NOT NULL,
	user_uid uuid NOT NULL,
	CONSTRAINT company_user_pk PRIMARY KEY (company_uid, user_uid)
)
WITH (
	OIDS = FALSE
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.company_user TO web;

ALTER TABLE public.company_user
    ADD CONSTRAINT fk_company_user_coompany_uid FOREIGN KEY (company_uid)
    REFERENCES public.company (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;