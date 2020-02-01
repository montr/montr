-- DROP TABLE montr.classifier_type;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_type'

CREATE TABLE IF NOT EXISTS montr.classifier_type
(
	uid uuid NOT NULL, 
	code character varying(64) NOT NULL,
    name character varying(2048) NOT NULL,
    description character varying(4096),
	hierarchy_type character varying(6),
	is_system boolean NOT NULL,
	CONSTRAINT classifier_type_pk PRIMARY KEY (uid),
	UNIQUE (code)
);
