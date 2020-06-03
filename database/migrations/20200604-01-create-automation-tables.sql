-- Table: montr.automation

-- DROP TABLE montr.automation;

CREATE TABLE montr.automation
(
    uid uuid NOT NULL,
    entity_type_code character varying(32) NOT NULL,
    entity_type_uid uuid NOT NULL,
    type_code character varying(32) NOT NULL,
    display_order integer NOT NULL DEFAULT 0,
    name character varying(128),
    description text,
    is_active boolean NOT NULL,
    is_system boolean NOT NULL,
    created_by uuid,
    created_at_utc timestamp with time zone,
    modified_by uuid,
    modified_at_utc timestamp with time zone,
    CONSTRAINT automation_pkey PRIMARY KEY (uid)
);

-- Index: ix_field_metadata_entity_type_code_entity_type_uid

-- DROP INDEX montr.ix_field_metadata_entity_type_code_entity_type_uid;

CREATE INDEX ix_field_metadata_entity_type_code_entity_type_uid
    ON montr.automation USING btree (entity_type_code, entity_type_uid);
