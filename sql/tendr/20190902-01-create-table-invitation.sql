-- Table: public.invitation

-- DROP TABLE public.invitation;

CREATE TABLE public.invitation
(
    uid uuid NOT NULL, -- DEFAULT uuid_generate_v1(),
    event_uid uuid NOT NULL,
	status_code character varying(16) COLLATE pg_catalog."default",
    counterparty_uid uuid NOT NULL,
    CONSTRAINT invitation_pk PRIMARY KEY (uid)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.invitation TO web;

ALTER TABLE public.invitation
    ADD CONSTRAINT fk_invitation_event_uid FOREIGN KEY (event_uid)
    REFERENCES public.event (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE public.invitation
    ADD CONSTRAINT fk_invitation_counterparty_uid FOREIGN KEY (counterparty_uid)
    REFERENCES public.classifier (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;