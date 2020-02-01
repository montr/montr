-- SEQUENCE: montr.event_id_seq

-- DROP SEQUENCE montr.event_id_seq;

CREATE SEQUENCE montr.event_id_seq;

-- Table: montr.event

-- DROP TABLE montr.event;

CREATE TABLE montr.event
(
    uid uuid NOT NULL DEFAULT uuid_generate_v1(),
    id bigint NOT NULL DEFAULT nextval('montr.event_id_seq'::regclass),
    company_uid uuid NOT NULL,
    is_template boolean NOT NULL,
    template_uid uuid,
	config_code character varying(64) NOT NULL,
    status_code character varying(16) NOT NULL,
    name character varying(2048),
    description text,
    CONSTRAINT event_pk PRIMARY KEY (uid)
);

-- Index: event_id_ux

-- DROP INDEX montr.event_id_ux;

CREATE UNIQUE INDEX event_id_ux
    ON montr.event USING btree (id);

ALTER TABLE montr.event
    ADD CONSTRAINT fk_event_template_uid FOREIGN KEY (template_uid)
    REFERENCES montr.event (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE montr.event
    ADD CONSTRAINT fk_event_company_uid FOREIGN KEY (company_uid)
    REFERENCES montr.company (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
