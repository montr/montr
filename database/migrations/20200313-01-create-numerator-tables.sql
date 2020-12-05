-- Table: montr.numerator

CREATE TABLE montr.numerator
(
    uid uuid NOT NULL,
    entity_type_code character varying(32) NOT NULL,
    pattern character varying(64) NOT NULL,
    periodicity character varying(8) NOT NULL,
    key_tags character varying(128),
    PRIMARY KEY (uid),
    CONSTRAINT fk_numerator_classifier_uid FOREIGN KEY (uid)
        REFERENCES montr.classifier (uid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE TABLE montr.numerator_counter
(
    numerator_uid uuid NOT NULL,
    key character varying(128) NOT NULL,
    value bigint NOT NULL,
	generated_at_utc timestamp with time zone NOT NULL,
    PRIMARY KEY (numerator_uid, key),
    CONSTRAINT fk_numerator_counter_numerator_uid FOREIGN KEY (numerator_uid)
        REFERENCES montr.numerator (uid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE TABLE montr.numerator_entity
(
    numerator_uid uuid NOT NULL,
    entity_uid uuid NOT NULL,
    PRIMARY KEY (entity_uid),
    CONSTRAINT fk_numerator_entity_numerator_uid FOREIGN KEY (numerator_uid)
        REFERENCES montr.numerator (uid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE INDEX ix_numerator_entity_numerator_uid
    ON montr.numerator_entity USING btree
    (numerator_uid ASC NULLS LAST);
