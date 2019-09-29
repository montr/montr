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
    config_code character varying(64) COLLATE pg_catalog."default",
    status_code character varying(16) COLLATE pg_catalog."default",
    company_uid uuid NOT NULL,
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
    ON public.event USING btree
    (id)
    TABLESPACE pg_default;

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.event TO web;

ALTER TABLE public.event
    ADD CONSTRAINT fk_event_coompany_uid FOREIGN KEY (company_uid)
    REFERENCES public.company (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;