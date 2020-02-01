-- DROP TABLE montr.classifier_tree CASCADE;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_tree'

CREATE TABLE IF NOT EXISTS montr.classifier_tree
(
	uid uuid NOT NULL, -- DEFAULT uuid_generate_v1(),
	type_uid uuid NOT NULL,
    code character varying(32) NOT NULL,
    name character varying(2048),
	CONSTRAINT classifier_tree_pk PRIMARY KEY (uid),
	UNIQUE (type_uid, code)
);

ALTER TABLE montr.classifier_tree
    ADD CONSTRAINT fk_classifier_tree_type_code FOREIGN KEY (type_uid)
    REFERENCES montr.classifier_type (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

-- DROP TABLE montr.classifier_group CASCADE;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_group'

CREATE TABLE IF NOT EXISTS montr.classifier_group
(
	uid uuid NOT NULL, -- DEFAULT uuid_generate_v1(),
	tree_uid uuid NOT NULL,
	parent_uid uuid NULL,
    code character varying(32) NOT NULL,
    name character varying(2048),
	CONSTRAINT classifier_group_pk PRIMARY KEY (uid),
	UNIQUE (tree_uid, code)
);

ALTER TABLE montr.classifier_group
    ADD CONSTRAINT fk_classifier_group_tree_uid FOREIGN KEY (tree_uid)
    REFERENCES montr.classifier_tree (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE montr.classifier_group
    ADD CONSTRAINT fk_classifier_group_parent_uid FOREIGN KEY (parent_uid)
    REFERENCES montr.classifier_group (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

-- DROP TABLE montr.classifier CASCADE;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier'

CREATE TABLE IF NOT EXISTS montr.classifier
(
	uid uuid NOT NULL, -- DEFAULT uuid_generate_v1(),
	type_uid uuid NOT NULL,
    status_code character varying(16) NOT NULL,
	parent_uid uuid NULL,
    code character varying(32) NOT NULL,
    name character varying(2048),
	CONSTRAINT classifier_pk PRIMARY KEY (uid),
	UNIQUE (type_uid, code)
);

ALTER TABLE montr.classifier
    ADD CONSTRAINT fk_classifier_type_uid FOREIGN KEY (type_uid)
    REFERENCES montr.classifier_type (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE montr.classifier
    ADD CONSTRAINT fk_classifier_parent_uid FOREIGN KEY (parent_uid)
    REFERENCES montr.classifier (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

-- DROP TABLE montr.classifier_link;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_link'

CREATE TABLE IF NOT EXISTS montr.classifier_link
(
	group_uid uuid NOT NULL,
	item_uid uuid NOT NULL,
	CONSTRAINT classifier_link_pk PRIMARY KEY (group_uid, item_uid)
);

ALTER TABLE montr.classifier_link
    ADD CONSTRAINT fk_classifier_link_group_uid FOREIGN KEY (group_uid)
    REFERENCES montr.classifier_group (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE montr.classifier_link
    ADD CONSTRAINT fk_classifier_link_item_uid FOREIGN KEY (item_uid)
    REFERENCES montr.classifier (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

-- DROP TABLE montr.classifier_closure;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_closure'

CREATE TABLE IF NOT EXISTS montr.classifier_closure
(
	parent_uid uuid NOT NULL,
	child_uid uuid NOT NULL,
	level smallint NOT NULL,
	CONSTRAINT classifier_closure_pk PRIMARY KEY (parent_uid, child_uid, level)
);
