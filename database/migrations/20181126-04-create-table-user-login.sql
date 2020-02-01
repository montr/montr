-- Table: montr.user_login

-- DROP TABLE montr.user_login;

CREATE TABLE montr.user_login
(
  user_id uuid NOT NULL,
  login_provider character varying(36) NOT NULL,
  provider_key character varying(128) NOT NULL,
  provider_display_name character varying(128),
  CONSTRAINT pk_user_login PRIMARY KEY (login_provider, provider_key),
  CONSTRAINT fk_user_login_user_id FOREIGN KEY (user_id)
      REFERENCES montr.users (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Index: montr.ix_user_login_user_id

-- DROP INDEX montr.ix_user_login_user_id;

CREATE INDEX ix_user_login_user_id ON montr.user_login USING btree (user_id);

