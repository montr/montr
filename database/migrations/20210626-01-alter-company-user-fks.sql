ALTER TABLE montr.company_user RENAME CONSTRAINT fk_company_user_coompany_uid TO fk_company_user_company_uid;
ALTER TABLE montr.company_user ADD CONSTRAINT fk_company_user_users_id FOREIGN KEY (user_uid) REFERENCES montr.users(id);
