-- Table: montr.invitation

-- DROP TABLE montr.invitation;

CREATE TABLE montr.invitation
(
    uid uuid NOT NULL, -- DEFAULT uuid_generate_v1(),
    event_uid uuid NOT NULL,
	status_code character varying(16),
    counterparty_uid uuid NOT NULL,
	email character varying(128),
    CONSTRAINT invitation_pk PRIMARY KEY (uid)
);

ALTER TABLE montr.invitation
    ADD CONSTRAINT fk_invitation_event_uid FOREIGN KEY (event_uid)
    REFERENCES montr.event (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE montr.invitation
    ADD CONSTRAINT fk_invitation_counterparty_uid FOREIGN KEY (counterparty_uid)
    REFERENCES montr.classifier (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
