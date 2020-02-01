-- Table: montr.roles

-- DROP TABLE montr.roles;

CREATE TABLE montr.roles
(
  id uuid NOT NULL, -- DEFAULT uuid_generate_v1(), 
  name character varying(128),
  normalized_name character varying(128),
  concurrency_stamp character varying(36) NOT NULL,
  CONSTRAINT pk_role_id PRIMARY KEY (id)
);

-- Index: montr.ix_roles_name

-- DROP INDEX montr.ix_roles_name;

CREATE INDEX ix_roles_name ON montr.roles USING btree (normalized_name);
