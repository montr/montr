-- Table: montr.company

-- DROP TABLE montr.company_user;
-- SELECT * FROM information_schema.tables WHERE table_name = 'company_user'

CREATE TABLE IF NOT EXISTS montr.company_user
(
	company_uid uuid NOT NULL,
	user_uid uuid NOT NULL,
	CONSTRAINT company_user_pk PRIMARY KEY (company_uid, user_uid)
);


ALTER TABLE montr.company_user
    ADD CONSTRAINT fk_company_user_coompany_uid FOREIGN KEY (company_uid)
    REFERENCES montr.company (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
    