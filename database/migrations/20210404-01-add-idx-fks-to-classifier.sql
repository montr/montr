ALTER TABLE montr.roles ADD CONSTRAINT fk_roles_id_classifier_uid FOREIGN KEY (id) REFERENCES montr.classifier(uid);

ALTER TABLE montr.users ADD CONSTRAINT fk_users_id_classifier_uid FOREIGN KEY (id) REFERENCES montr.classifier(uid);
