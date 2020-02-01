-- Table: public.users

-- DROP TABLE public.users;

CREATE TABLE public.users
(
  id uuid NOT NULL, -- DEFAULT uuid_generate_v1(),
  user_name character varying(128),
  first_name character varying(128),
  last_name character varying(128),
  email character varying(128),
  email_confirmed boolean NOT NULL DEFAULT false,
  phone_number character varying(12),
  phone_number_confirmed boolean NOT NULL DEFAULT false,
  password_hash text,
  security_stamp character varying(36),
  two_factor_enabled boolean NOT NULL DEFAULT false,
  lockout_enabled boolean NOT NULL DEFAULT false,
  lockout_end timestamp without time zone,
  access_failed_count integer NOT NULL DEFAULT 0,
  normalized_user_name character varying(128),
  normalized_email character varying(128),
  concurrency_stamp character varying(36) NOT NULL,
  --created_at timestamp without time zone NOT NULL,
  --created_by character varying(128),
  --modified_at timestamp without time zone,
  --modified_by character varying(128),
  CONSTRAINT pk_user_id PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);

-- Index: public.ix_users_email

-- DROP INDEX public.ix_users_email;

CREATE INDEX ix_users_email
  ON public.users
  USING btree
  (normalized_email COLLATE pg_catalog."default");

-- Index: public.ix_users_user_name

-- DROP INDEX public.ix_users_user_name;

CREATE INDEX ix_users_user_name
  ON public.users
  USING btree
  (normalized_user_name COLLATE pg_catalog."default");
