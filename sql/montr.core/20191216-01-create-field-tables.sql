-- Table: public.field_meta

-- DROP TABLE public.field_meta;

CREATE TABLE public.field_meta
(
    uid uuid NOT NULL,
    entity_type_code character varying(32) COLLATE pg_catalog."default" NOT NULL,
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
	extra text COLLATE pg_catalog."default",
    CONSTRAINT field_meta_pkey PRIMARY KEY (uid)
);

ALTER TABLE public.field_meta OWNER to postgres;

GRANT ALL ON TABLE public.field_meta TO web;

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.field_meta TO web;

-- Index: ix_field_entity_type_code

-- DROP INDEX public.ix_field_entity_type_code;

CREATE INDEX ix_field_entity_type_code
    ON public.field_meta (entity_type_code);
	
-- Index: ux_field_meta_entity_type_code_key

-- DROP INDEX public.ux_field_meta_entity_type_code_key;

CREATE UNIQUE INDEX ux_field_meta_entity_type_code_key
    ON public.field_meta (entity_type_code, key);
