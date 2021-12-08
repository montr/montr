
-- montr.entity_relation definition

-- Drop table

-- DROP TABLE montr.entity_relation;

CREATE TABLE montr.entity_relation (
    entity_type_code varchar(32) NOT NULL,
    entity_uid uuid NOT NULL,
    related_entity_type_code varchar(32) NOT NULL,
    related_entity_uid uuid NOT NULL,
    relation_type varchar(32) NOT NULL,
	CONSTRAINT pk_relation_type PRIMARY KEY (entity_type_code, entity_uid, related_entity_type_code, related_entity_uid, relation_type)
);
