-- Table: montr.user_role

-- DROP TABLE montr.user_role;

CREATE TABLE montr.user_role
(
  user_id uuid NOT NULL,
  role_id uuid NOT NULL,
  CONSTRAINT pk_user_role PRIMARY KEY (user_id, role_id)
);
