DROP TABLE montr.document_20200229;

TRUNCATE TABLE montr.document;

ALTER TABLE montr.document DROP COLUMN config_code;

ALTER TABLE montr.document
    ADD COLUMN document_type_uid uuid NOT NULL;

ALTER TABLE montr.document
    ADD CONSTRAINT document_document_type_uid FOREIGN KEY (document_type_uid)
    REFERENCES montr.document_type (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;