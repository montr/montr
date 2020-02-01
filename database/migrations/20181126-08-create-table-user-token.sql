-- Table: montr.user_token

-- DROP TABLE montr.user_token;

CREATE TABLE montr.user_token
(
  user_id uuid NOT NULL,
  login_provider character varying(36) NOT NULL,
  name character varying(128) NOT NULL,
  value text,
  CONSTRAINT pk_user_token PRIMARY KEY (user_id, login_provider, name)
);
