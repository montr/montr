## Default column size

    uid uuid NOT NULL DEFAULT uuid_generate_v1()

    config_code character varying(64) COLLATE pg_catalog."default" NOT NULL
    status_code character varying(16) COLLATE pg_catalog."default" NOT NULL

    entity_type_code character varying(32) COLLATE pg_catalog."default" NOT NULL
    entity_uid uuid NOT NULL

    company_uid uuid NOT NULL


-- https://ruhighload.com/%D0%9F%D1%80%D0%BE%D1%84%D0%B8%D0%BB%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5+%D0%B2+postgresql
-- http://pgfoundry.org/softwaremap/trove_list.php?form_cat=320
-- https://www.percona.com/blog/2019/02/13/plprofiler-getting-a-handy-tool-for-profiling-your-pl-pgsql-code/

select query, * from pg_stat_activity
-- select * from pg_stat_statements