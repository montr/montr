-- Table: montr.settings

-- DROP TABLE montr.settings;

CREATE TABLE montr.settings
(
    id character varying(128) COLLATE pg_catalog."default" NOT NULL,
    value text COLLATE pg_catalog."default",
    CONSTRAINT settings_pkey PRIMARY KEY (id)
);
