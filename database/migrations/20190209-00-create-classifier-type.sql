-- DROP TABLE public.classifier_type;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_type'

CREATE TABLE IF NOT EXISTS public.classifier_type
(
	uid uuid NOT NULL, 
	code character varying(64) NOT NULL COLLATE pg_catalog."default",
    name character varying(2048) NOT NULL COLLATE pg_catalog."default",
    description character varying(4096) COLLATE pg_catalog."default",
	hierarchy_type character varying(6) NOT NULL COLLATE pg_catalog."default",
	is_system boolean NOT NULL,
	CONSTRAINT classifier_type_pk PRIMARY KEY (uid),
	UNIQUE (code)
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.classifier_type TO web;

/*
-- select uuid_generate_v1()

insert into classifier_type (uid, company_uid, code, name, description, hierarchy_type) values (
	'7d5d9c56-3ddc-11e9-8c57-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'okei', 'ОКЕИ', 'Общероссийский классификатор единиц измерения',
	'Groups'
);
insert into classifier_type (uid, company_uid, code, name, description, hierarchy_type) values (
	'86c43b74-3ddc-11e9-8c58-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'okved2', 'ОКВЭД 2', 'Общероссийский классификатор видов экономической деятельности',
	'Items'
);
insert into classifier_type (uid, company_uid, code, name, description, hierarchy_type) values (
	'8eb7c1d4-3ddc-11e9-8c59-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'okpd2', 'ОКПД 2', 'Общероссийский классификатор продукции по видам экономической деятельности',
	'Items'
);
insert into classifier_type (uid, company_uid, code, name, description, hierarchy_type) values (
	'ba229b6e-49bc-11e9-8cc9-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'oktmo', 'ОКТМО', 'Общероссийский классификатор территорий муниципальных образований',
	'Items'
);
insert into classifier_type (uid, company_uid, code, name, description, hierarchy_type) values (
	'36524396-4e5f-11e9-8062-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'okopf', 'ОКОПФ', 'Общероссийский классификатор организационно-правовых форм',
	'Items'
);
insert into classifier_type (uid, company_uid, code, name, description, hierarchy_type) values (
	'3f56c480-4e5f-11e9-8063-00ff279ba9e1',
	'6465dd4c-8664-4433-ba6a-14effd40ebed', 'okv', 'ОКВ', 'Общероссийский классификатор валют',
	'None'
);
*/
