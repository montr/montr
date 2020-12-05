ALTER TABLE montr.classifier ADD COLUMN is_active boolean NOT NULL DEFAULT true;
ALTER TABLE montr.classifier ADD COLUMN is_system boolean NOT NULL DEFAULT true;

ALTER TABLE montr.classifier ALTER COLUMN is_active DROP DEFAULT;
ALTER TABLE montr.classifier ALTER COLUMN is_system DROP DEFAULT;
