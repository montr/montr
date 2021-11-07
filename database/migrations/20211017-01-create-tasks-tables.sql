-- montr.task_type definition

-- Drop table

-- DROP TABLE montr.task_type;

CREATE TABLE montr.task_type (
	uid uuid NOT NULL,
	CONSTRAINT task_type_pk PRIMARY KEY (uid)
);


-- montr.task_type foreign keys

ALTER TABLE montr.task_type ADD CONSTRAINT fk_task_type_id_classifier_uid FOREIGN KEY (uid) REFERENCES montr.classifier(uid);

-- montr.task definition

-- Drop table

-- DROP TABLE montr.task;

CREATE TABLE montr.task (
	uid uuid NOT NULL,
	company_uid uuid NOT NULL,
	task_type_uid uuid NOT NULL,
	status_code varchar(16) NOT NULL,
	number varchar(32) NULL,
	assignee_uid uuid NULL,
	parent_uid uuid NULL,
	"name" varchar(2048) NULL,
	description text NULL,
	start_date_utc timestamptz NULL,
	due_date_utc timestamptz NULL,
	created_by uuid NULL,
	created_at_utc timestamptz NULL,
	modified_by uuid NULL,
	modified_at_utc timestamptz NULL,
	CONSTRAINT pk_task PRIMARY KEY (uid)
);


-- montr.task foreign keys

ALTER TABLE montr.task ADD CONSTRAINT fk_task_company_uid FOREIGN KEY (company_uid) REFERENCES montr.company(uid);
ALTER TABLE montr.task ADD CONSTRAINT fk_task_parent_uid FOREIGN KEY (parent_uid) REFERENCES montr.task(uid);
ALTER TABLE montr.task ADD CONSTRAINT fk_task_task_type_uid FOREIGN KEY (task_type_uid) REFERENCES montr.task_type(uid);
