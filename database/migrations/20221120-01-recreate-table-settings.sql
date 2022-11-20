-- Table: montr.settings

ALTER TABLE montr.settings DROP CONSTRAINT settings_pkey;
ALTER TABLE montr.settings RENAME TO settings_bak;

CREATE TABLE montr.settings
(
    entity_type_code character varying(32) NOT NULL,
    entity_uid uuid NOT NULL,
    key character varying(128) NOT NULL,
    value text,
    created_at_utc timestamp with time zone,
	created_by character varying(128),
	modified_at_utc timestamp with time zone,
	modified_by character varying(128),
    CONSTRAINT settings_pkey PRIMARY KEY (entity_type_code, entity_uid, key)
);

insert into montr.settings (entity_type_code, entity_uid, key, value, created_at_utc, created_by, modified_at_utc, modified_by)
select 'application', '00000000-0000-0000-0000-000000000000', id, value, created_at_utc, created_by, modified_at_utc, modified_by from montr.settings_bak;

DROP TABLE montr.settings_bak;
