-- Table: public.invitation

-- DROP TABLE public.invitation;

CREATE TABLE public.invitation
(
    uid uuid NOT NULL DEFAULT uuid_generate_v1(),
    company_uid uuid NOT NULL,
    CONSTRAINT invitation_pk PRIMARY KEY (uid)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.invitation TO web;

ALTER TABLE public.invitation
    ADD CONSTRAINT fk_invitation_coompany_uid FOREIGN KEY (company_uid)
    REFERENCES public.company (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;