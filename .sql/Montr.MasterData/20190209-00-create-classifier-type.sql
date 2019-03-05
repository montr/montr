-- DROP TABLE public.classifier_type;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_type'

CREATE TABLE IF NOT EXISTS public.classifier_type
(
	uid uuid NOT NULL, 
	company_uid uuid NOT NULL,
	code character varying(64) NOT NULL COLLATE pg_catalog."default",
    name character varying(2048) NOT NULL COLLATE pg_catalog."default",
	hierarchy_type character varying(6) NOT NULL COLLATE pg_catalog."default",
	CONSTRAINT classifier_type_pk PRIMARY KEY (uid),
	UNIQUE (company_uid, code)
)
WITH (
	OIDS = FALSE
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.classifier_type TO web;

ALTER TABLE public.classifier_type
    ADD CONSTRAINT fk_classifier_type_company_uid FOREIGN KEY (company_uid)
    REFERENCES public.company (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

/*
insert into classifier_type (uid, company_uid, code, name, hierarchy_type) values (
	'7d5d9c56-3ddc-11e9-8c57-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'okei', 'ОКЕИ — Общероссийский классификатор единиц измерения',
	'Groups'
);
insert into classifier_type (uid, company_uid, code, name, hierarchy_type) values (
	'86c43b74-3ddc-11e9-8c58-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'okved2', 'ОКВЭД 2 — Общероссийский классификатор видов экономической деятельности',
	'Items'
);
insert into classifier_type (uid, company_uid, code, name, hierarchy_type) values (
	'8eb7c1d4-3ddc-11e9-8c59-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'okpd2', 'ОКПД 2 — Общероссийский классификатор продукции по видам экономической деятельности',
	'Items'
);
insert into classifier_type (uid, company_uid, code, name, hierarchy_type) values (
	'9afcae3c-3ddc-11e9-8c5a-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'okato', 'ОКАТО — Общероссийский классификатор объектов административно-территориального деления',
	'Items'
);
*/
