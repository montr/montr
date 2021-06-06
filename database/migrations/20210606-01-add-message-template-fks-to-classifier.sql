ALTER TABLE montr.message_template ADD CONSTRAINT fk_message_template_id_classifier_uid FOREIGN KEY (uid) REFERENCES montr.classifier(uid);
