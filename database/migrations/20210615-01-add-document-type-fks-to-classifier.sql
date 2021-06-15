ALTER TABLE montr.document_type ADD CONSTRAINT fk_document_type_id_classifier_uid FOREIGN KEY (uid) REFERENCES montr.classifier(uid);
