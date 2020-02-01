-- Table: public.user_role

-- DROP TABLE public.user_role;

CREATE TABLE public.user_role
(
  user_id uuid NOT NULL,
  role_id uuid NOT NULL,
  CONSTRAINT pk_user_role PRIMARY KEY (user_id, role_id)
)
WITH (
  OIDS=FALSE
);
