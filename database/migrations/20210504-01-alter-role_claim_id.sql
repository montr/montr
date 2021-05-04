-- Sequence: montr.role_claim_id_seq

-- DROP SEQUENCE montr.role_claim_id_seq;

CREATE SEQUENCE montr.role_claim_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 100
  CACHE 1;
  
ALTER TABLE montr.role_claim ALTER COLUMN id TYPE integer USING 0;
ALTER TABLE montr.role_claim ALTER COLUMN id SET DEFAULT nextval('montr.role_claim_id_seq'::regclass);
