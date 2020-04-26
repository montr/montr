-- Table: montr.document_type

-- DROP TABLE montr.document_type;

CREATE TABLE montr.document_type
(
    uid uuid NOT NULL,
    code character varying(64) NOT NULL,
    name character varying(2048) NOT NULL,
    description character varying(4096),
    CONSTRAINT document_type_pk PRIMARY KEY (uid),
    CONSTRAINT document_type_code_key UNIQUE (code)
);
