-- Table: montr.numerator

CREATE TABLE montr.numerator
(
    uid uuid NOT NULL,
    name character varying(128) NOT NULL,
    pattern character varying(64) NOT NULL,
    periodicity character varying(8) NOT NULL,
    is_active boolean NOT NULL,
    is_system boolean NOT NULL,
    PRIMARY KEY (uid)
);

CREATE TABLE montr.numerator_counter
(
    numerator_uid uuid NOT NULL,
    key character varying(128) NOT NULL,
    value bigint NOT NULL,
    PRIMARY KEY (numerator_uid, key),
    CONSTRAINT fk_numerator_counter_numerator FOREIGN KEY (numerator_uid)
        REFERENCES montr.numerator (uid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
);