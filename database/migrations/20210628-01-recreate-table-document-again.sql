DROP TABLE montr."document";

CREATE TABLE montr."document" (
	uid uuid NOT NULL DEFAULT uuid_generate_v1(),
	document_type_uid uuid NOT NULL,
	document_date_utc timestamptz NOT NULL,
	status_code varchar(16) NOT NULL,
	direction varchar(8) NOT NULL,
	document_number varchar(64) NULL,
	company_uid uuid NULL,
	"name" varchar(2048) NULL,
	created_at_utc timestamptz NULL,
	created_by uuid NULL,
	modified_at_utc timestamptz NULL,
	modified_by uuid NULL,
	CONSTRAINT document_pk PRIMARY KEY (uid),
	CONSTRAINT document_document_type_uid FOREIGN KEY (document_type_uid) REFERENCES montr.document_type(uid),
	CONSTRAINT document_created_by FOREIGN KEY (created_by) REFERENCES montr.users(id),
	CONSTRAINT document_modified_by FOREIGN KEY (modified_by) REFERENCES montr.users(id),
	CONSTRAINT document_company_uid FOREIGN KEY (company_uid) REFERENCES montr.company(uid)
);
