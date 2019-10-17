-- Table: public.locale_string

-- DROP TABLE public.locale_string;

CREATE TABLE public.locale_string
(
    locale character varying(8) COLLATE pg_catalog."default" NOT NULL,
    module character varying(32) COLLATE pg_catalog."default" NOT NULL,
    key character varying(128) COLLATE pg_catalog."default" NOT NULL,
    value text COLLATE pg_catalog."default",
    CONSTRAINT locale_string_pkey PRIMARY KEY (locale, module, key)
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.locale_string TO web;
