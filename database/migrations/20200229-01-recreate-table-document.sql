DO
$$
BEGIN
    IF EXISTS (select * from information_schema.columns
		where table_schema = 'montr' and table_name = 'document' and column_name = 'uid'
	) AND NOT EXISTS (select * from information_schema.columns
		where table_schema = 'montr' and table_name = 'document' and column_name = 'document_number'
	) THEN
       ALTER TABLE montr.document DROP CONSTRAINT document_pk;
       ALTER TABLE montr.document RENAME TO document_20200229;
    END IF;
END
$$;

-- Table: montr.document

-- DROP TABLE montr.document;
-- SELECT * FROM information_schema.tables WHERE table_name = 'document'

CREATE TABLE IF NOT EXISTS montr.document
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(),
	company_uid uuid NOT NULL,
	config_code character varying(64) NOT NULL,
	status_code character varying(16) NOT NULL,
	direction character varying(8) NOT NULL, -- incoming, outgoing, internal
	document_number character varying(64) NOT NULL,
	document_date_utc timestamp with time zone NOT NULL,
	name character varying(2048),
	created_at_utc timestamp with time zone,
	created_by character varying(128),
	modified_at_utc timestamp with time zone,
	modified_by character varying(128),
	CONSTRAINT document_pk PRIMARY KEY (uid)
);
