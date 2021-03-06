-- Sequence: montr.user_claim_id_seq

-- DROP SEQUENCE montr.user_claim_id_seq;

CREATE SEQUENCE montr.user_claim_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 100
  CACHE 1;

-- Table: montr.user_claim

-- DROP TABLE montr.user_claim;

CREATE TABLE montr.user_claim
(
  id integer NOT NULL DEFAULT nextval('montr.user_claim_id_seq'::regclass),
  user_id uuid NOT NULL,
  claim_type character varying(32) NOT NULL,
  claim_value character varying(128) NOT NULL,
  CONSTRAINT pk_user_claim PRIMARY KEY (id),
  CONSTRAINT fk_user_claim_user_id FOREIGN KEY (user_id)
      REFERENCES montr.users (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Index: montr.ix_user_claim_user_id

-- DROP INDEX montr.ix_user_claim_user_id;

CREATE INDEX ix_user_claim_user_id ON montr.user_claim USING btree (user_id);
