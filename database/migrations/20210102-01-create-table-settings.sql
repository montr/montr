-- Table: montr.settings

-- DROP TABLE montr.settings;

CREATE TABLE montr.settings
(
    id character varying(128) NOT NULL,
    value text,
    created_at_utc timestamp with time zone,
	created_by character varying(128),
	modified_at_utc timestamp with time zone,
	modified_by character varying(128),
    CONSTRAINT settings_pkey PRIMARY KEY (id)
);
