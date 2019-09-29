-- SEQUENCE: public.event_id_seq

-- DROP SEQUENCE public.event_id_seq;

CREATE SEQUENCE public.event_id_seq;

GRANT USAGE, SELECT, UPDATE ON SEQUENCE public.event_id_seq TO web;

-- Table: public.event

-- DROP TABLE public.event;

CREATE TABLE public.event
(
    uid uuid NOT NULL DEFAULT uuid_generate_v1(),
    id bigint NOT NULL DEFAULT nextval('event_id_seq'::regclass),
    company_uid uuid NOT NULL,
    is_template boolean NOT NULL,
    template_uid uuid,
	config_code character varying(64) NOT NULL COLLATE pg_catalog."default",
    status_code character varying(16) NOT NULL COLLATE pg_catalog."default",
    name character varying(2048) COLLATE pg_catalog."default",
    description text COLLATE pg_catalog."default",
    CONSTRAINT event_pk PRIMARY KEY (uid)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

-- Index: event_id_ux

-- DROP INDEX public.event_id_ux;

CREATE UNIQUE INDEX event_id_ux
    ON public.event USING btree (id)
    TABLESPACE pg_default;

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.event TO web;

ALTER TABLE public.event
    ADD CONSTRAINT fk_event_template_uid FOREIGN KEY (template_uid)
    REFERENCES public.event (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE public.event
    ADD CONSTRAINT fk_event_company_uid FOREIGN KEY (company_uid)
    REFERENCES public.company (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

/*

insert into event (uid, id, company_uid, is_template, status_code, config_code, name, description) values (
	uuid_generate_v1(), nextval('event_id_seq'::regclass), '6465dd4c-8664-4433-ba6a-14effd40ebed', 1::boolean, 'draft',
	'rfi', 'Запрос информации', ''
);

insert into event (uid, id, company_uid, is_template, status_code, config_code, name, description) values (
	uuid_generate_v1(), nextval('event_id_seq'::regclass), '6465dd4c-8664-4433-ba6a-14effd40ebed', 1::boolean, 'draft',
	'rfp', 'Запрос предложений', ''
);

insert into event (uid, id, company_uid, is_template, status_code, config_code, name, description) values (
	uuid_generate_v1(), nextval('event_id_seq'::regclass), '6465dd4c-8664-4433-ba6a-14effd40ebed', 1::boolean, 'draft',
	'proposal', 'Предложение', ''
);

*/
