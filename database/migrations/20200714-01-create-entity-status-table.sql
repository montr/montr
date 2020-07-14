-- Table: montr.entity_status

-- DROP TABLE montr.entity_status;

CREATE TABLE montr.entity_status
(
    entity_type_code character varying(32) NOT NULL,
    entity_uid uuid NOT NULL,
    code character varying(16) NOT NULL,
    display_order integer NOT NULL DEFAULT 0,
    name character varying(128) NOT NULL,
    CONSTRAINT pk_entity_status PRIMARY KEY (entity_type_code, entity_uid, code)
);
