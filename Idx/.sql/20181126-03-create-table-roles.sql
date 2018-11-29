-- Table: public.roles

-- DROP TABLE public.roles;

CREATE TABLE public.roles
(
  id uuid NOT NULL, -- DEFAULT uuid_generate_v1(), 
  name character varying(128),
  normalized_name character varying(128),
  concurrency_stamp character varying(36) NOT NULL,
  CONSTRAINT pk_role_id PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);

-- Index: public.ix_roles_name

-- DROP INDEX public.ix_roles_name;

CREATE INDEX ix_roles_name
  ON public.roles
  USING btree
  (normalized_name COLLATE pg_catalog."default");
