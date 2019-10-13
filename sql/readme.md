## Default column size

    uid uuid NOT NULL DEFAULT uuid_generate_v1()

    config_code character varying(64) COLLATE pg_catalog."default" NOT NULL
    status_code character varying(16) COLLATE pg_catalog."default" NOT NULL

    entity_type_code character varying(32) COLLATE pg_catalog."default" NOT NULL
    entity_uid uuid NOT NULL

    company_uid uuid NOT NULL
