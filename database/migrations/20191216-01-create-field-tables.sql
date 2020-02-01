-- Table: montr.field_metadata

-- DROP TABLE montr.field_metadata;

CREATE TABLE montr.field_metadata
(
    uid uuid NOT NULL,
    entity_type_code character varying(32) NOT NULL,
    entity_uid uuid NOT NULL,
    key character varying(32) NOT NULL,
    type_code character varying(32) NOT NULL,
    is_active boolean NOT NULL,
    is_system boolean NOT NULL,
    is_required boolean NOT NULL,
    is_readonly boolean NOT NULL,
    display_order integer NOT NULL DEFAULT 0,
    created_by uuid,
    created_at_utc timestamp with time zone,
    modified_by uuid,
    modified_at_utc timestamp with time zone,
    name character varying(128),
    description text,
    placeholder character varying(128),
    icon character varying(32),
    props text,
    CONSTRAINT field_meta_pkey PRIMARY KEY (uid)
);

-- Index: ix_field_metadata_entity_type_code_entity_uid

-- DROP INDEX montr.ix_field_metadata_entity_type_code_entity_uid;

CREATE INDEX ix_field_metadata_entity_type_code_entity_uid
    ON montr.field_metadata USING btree (entity_type_code, entity_uid);

-- Table: montr.field_data

-- DROP TABLE montr.field_data;

CREATE TABLE montr.field_data
(
    uid uuid NOT NULL,
    entity_type_code character varying(32) NOT NULL,
    entity_uid uuid NOT NULL,
    key character varying(32) NOT NULL,
    value text,
    CONSTRAINT field_data_pkey PRIMARY KEY (uid),
    CONSTRAINT ux_field_data_entity_type_code_entity_uid_key UNIQUE (entity_type_code, entity_uid, key)

);

-- Index: ix_field_data_entity_type_code_entity_uid

-- DROP INDEX montr.ix_field_data_entity_type_code_entity_uid;

CREATE INDEX ix_field_data_entity_type_code_entity_uid ON montr.field_data (entity_type_code, entity_uid);
