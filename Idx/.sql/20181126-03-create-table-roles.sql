-- Table: public.roles

-- DROP TABLE public.roles;

CREATE TABLE public.roles
(
  id character varying(128) NOT NULL, 
  name character varying(128),
  normalized_name character varying(128),
  concurrency_stamp character varying(36) NOT NULL,
  CONSTRAINT pk_role_id PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.roles
  OWNER TO postgres;
GRANT ALL ON TABLE public.roles TO postgres;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE public.roles TO web;

-- Index: public.ix_roles_name

-- DROP INDEX public.ix_roles_name;

CREATE INDEX ix_roles_name
  ON public.roles
  USING btree
  (normalized_name COLLATE pg_catalog."default");
