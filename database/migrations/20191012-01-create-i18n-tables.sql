-- Table: montr.locale_string

-- DROP TABLE montr.locale_string;

CREATE TABLE montr.locale_string
(
    locale character varying(8) NOT NULL,
    module character varying(32) NOT NULL,
    key character varying(128) NOT NULL,
    value text,
    CONSTRAINT locale_string_pkey PRIMARY KEY (locale, module, key)
);
