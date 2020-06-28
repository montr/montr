-- Table: montr.automation

-- DROP TABLE montr.automation;

CREATE TABLE montr.automation
(
    uid uuid NOT NULL,
    entity_type_code character varying(32) NOT NULL,
    entity_type_uid uuid NOT NULL,
    type_code character varying(32) NOT NULL,
    display_order integer NOT NULL DEFAULT 0,
    name character varying(128),
    description text,
    is_active boolean NOT NULL,
    is_system boolean NOT NULL,
    created_by uuid,
    created_at_utc timestamp with time zone,
    modified_by uuid,
    modified_at_utc timestamp with time zone,
    CONSTRAINT pk_automation PRIMARY KEY (uid)
);

-- Index: ix_field_metadata_entity_type_code_entity_type_uid

-- DROP INDEX montr.ix_field_metadata_entity_type_code_entity_type_uid;

CREATE INDEX ix_automation_entity_type_code_entity_type_uid
    ON montr.automation USING btree (entity_type_code, entity_type_uid);

-- Table: montr.automation_action

-- DROP TABLE montr.automation_action;

CREATE TABLE montr.automation_action
(
    uid uuid NOT NULL,
    automation_uid uuid NOT NULL,
    display_order integer NOT NULL,
    type_code character varying(32) COLLATE pg_catalog."default" NOT NULL,
    props text COLLATE pg_catalog."default",
    CONSTRAINT pk_automation_action PRIMARY KEY (uid),
    CONSTRAINT fk_automation_action_automation_uid FOREIGN KEY (automation_uid)
        REFERENCES montr.automation (uid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

-- Table: montr.automation_condition

-- DROP TABLE montr.automation_condition;

CREATE TABLE montr.automation_condition
(
    uid uuid NOT NULL,
    automation_uid uuid NOT NULL,
    display_order integer NOT NULL,
    type_code character varying(32) COLLATE pg_catalog."default" NOT NULL,
    parent_uid uuid,
    props text COLLATE pg_catalog."default",
    CONSTRAINT pk_automation_condition PRIMARY KEY (uid),
    CONSTRAINT fk_automation_condition_automation_uid FOREIGN KEY (automation_uid)
        REFERENCES montr.automation (uid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT fk_automation_condition_parent_uid FOREIGN KEY (parent_uid)
        REFERENCES montr.automation_condition (uid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);
