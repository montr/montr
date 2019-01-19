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
)
