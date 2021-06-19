ALTER TABLE montr.company ADD CONSTRAINT fk_company_id_classifier_uid FOREIGN KEY (uid) REFERENCES montr.classifier(uid);

ALTER TABLE montr.company DROP COLUMN name;
ALTER TABLE montr.company DROP COLUMN status_code;
