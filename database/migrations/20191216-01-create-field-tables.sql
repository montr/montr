-- Table: public.field_metadata

-- DROP TABLE public.field_metadata;

CREATE TABLE public.field_metadata
(
    uid uuid NOT NULL,
    entity_type_code character varying(32) COLLATE pg_catalog."default" NOT NULL,
    entity_uid uuid NOT NULL,
    key character varying(32) COLLATE pg_catalog."default" NOT NULL,
    type_code character varying(32) COLLATE pg_catalog."default" NOT NULL,
    is_active boolean NOT NULL,
    is_system boolean NOT NULL,
    is_required boolean NOT NULL,
    is_readonly boolean NOT NULL,
    display_order integer NOT NULL DEFAULT 0,
    created_by uuid,
    created_at_utc timestamp with time zone,
    modified_by uuid,
    modified_at_utc timestamp with time zone,
    name character varying(128) COLLATE pg_catalog."default",
    description text COLLATE pg_catalog."default",
    placeholder character varying(128) COLLATE pg_catalog."default",
    icon character varying(32) COLLATE pg_catalog."default",
    props text COLLATE pg_catalog."default",
    CONSTRAINT field_meta_pkey PRIMARY KEY (uid)
);

ALTER TABLE public.field_metadata
    OWNER to postgres;

GRANT ALL ON TABLE public.field_metadata TO postgres;

GRANT ALL ON TABLE public.field_metadata TO web;

-- Index: ix_field_metadata_entity_type_code_entity_uid

-- DROP INDEX public.ix_field_metadata_entity_type_code_entity_uid;

CREATE INDEX ix_field_metadata_entity_type_code_entity_uid
    ON public.field_metadata USING btree
    (entity_type_code COLLATE pg_catalog."default" ASC NULLS LAST, entity_uid ASC NULLS LAST);


-- Table: public.field_data

-- DROP TABLE public.field_data;

CREATE TABLE public.field_data
(
    uid uuid NOT NULL,
    entity_type_code character varying(32) COLLATE pg_catalog."default" NOT NULL,
    entity_uid uuid NOT NULL,
    key character varying(32) COLLATE pg_catalog."default" NOT NULL,
    value text COLLATE pg_catalog."default",
    CONSTRAINT field_data_pkey PRIMARY KEY (uid),
    CONSTRAINT ux_field_data_entity_type_code_entity_uid_key UNIQUE (entity_type_code, entity_uid, key)

);

ALTER TABLE public.field_data
    OWNER to postgres;

GRANT ALL ON TABLE public.field_data TO postgres;

GRANT ALL ON TABLE public.field_data TO web;

-- Index: ix_field_data_entity_type_code_entity_uid

-- DROP INDEX public.ix_field_data_entity_type_code_entity_uid;

CREATE INDEX ix_field_data_entity_type_code_entity_uid ON public.field_data
    (entity_type_code COLLATE pg_catalog."default", entity_uid);
