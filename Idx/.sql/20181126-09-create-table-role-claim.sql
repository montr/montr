-- Table: public.role_claim

-- DROP TABLE public.role_claim;

CREATE TABLE public.role_claim
(
  id uuid NOT NULL, -- DEFAULT uuid_generate_v1(),
  role_id uuid NOT NULL,
  claim_type character varying(32) NOT NULL,
  claim_value character varying(128) NOT NULL,
  CONSTRAINT pk_role_claim PRIMARY KEY (id),
  CONSTRAINT fk_role_claim_role_id FOREIGN KEY (role_id)
      REFERENCES public.roles (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);

-- Index: public.ix_role_claim_role_id

-- DROP INDEX public.ix_role_claim_role_id;

CREATE INDEX ix_role_claim_role_id
  ON public.role_claim
  USING btree
  (role_id);
