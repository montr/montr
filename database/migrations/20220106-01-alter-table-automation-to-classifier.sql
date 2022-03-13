DROP INDEX montr.ix_automation_entity_type_code_entity_type_uid;

ALTER TABLE montr.automation DROP COLUMN "name";
ALTER TABLE montr.automation DROP COLUMN description;
ALTER TABLE montr.automation DROP COLUMN display_order;
ALTER TABLE montr.automation DROP COLUMN entity_type_uid;
ALTER TABLE montr.automation DROP COLUMN is_active;
ALTER TABLE montr.automation DROP COLUMN is_system;
ALTER TABLE montr.automation DROP COLUMN created_by;
ALTER TABLE montr.automation DROP COLUMN created_at_utc;
ALTER TABLE montr.automation DROP COLUMN modified_by;
ALTER TABLE montr.automation DROP COLUMN modified_at_utc;

CREATE INDEX automation_entity_type_code_idx ON montr.automation (entity_type_code);

ALTER TABLE montr.automation ADD CONSTRAINT automation_fk FOREIGN KEY (uid) REFERENCES montr.classifier(uid);
