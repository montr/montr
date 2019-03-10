-- DROP TABLE public.classifier_tree;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_tree'

CREATE TABLE IF NOT EXISTS public.classifier_tree
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(), 
	type_uid uuid NOT NULL,
    code character varying(32) NOT NULL COLLATE pg_catalog."default",
    name character varying(2048) COLLATE pg_catalog."default", 
	CONSTRAINT classifier_tree_pk PRIMARY KEY (uid),
	UNIQUE (type_uid, code)
)
WITH (
	OIDS = FALSE
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.classifier_tree TO web;

ALTER TABLE public.classifier_tree
    ADD CONSTRAINT fk_classifier_tree_type_code FOREIGN KEY (type_uid)
    REFERENCES public.classifier_type (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

-- DROP TABLE public.classifier_group;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_group'

CREATE TABLE IF NOT EXISTS public.classifier_group
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(), 
	tree_uid uuid NOT NULL,
	parent_uid uuid NULL,
    code character varying(32) NOT NULL COLLATE pg_catalog."default", 
    name character varying(2048) COLLATE pg_catalog."default", 
	CONSTRAINT classifier_group_pk PRIMARY KEY (uid),
	UNIQUE (tree_uid, code)
)
WITH (
	OIDS = FALSE
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.classifier_group TO web;

ALTER TABLE public.classifier_group
    ADD CONSTRAINT fk_classifier_group_tree_uid FOREIGN KEY (tree_uid)
    REFERENCES public.classifier_tree (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE public.classifier_group
    ADD CONSTRAINT fk_classifier_group_parent_uid FOREIGN KEY (parent_uid)
    REFERENCES public.classifier_group (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

-- DROP TABLE public.classifier;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier'

CREATE TABLE IF NOT EXISTS public.classifier
(
	uid uuid NOT NULL DEFAULT uuid_generate_v1(), 
	type_uid uuid NOT NULL,
    status_code character varying(16) NOT NULL COLLATE pg_catalog."default", 
	parent_uid uuid NULL,
    code character varying(32) NOT NULL COLLATE pg_catalog."default", 
    name character varying(2048) COLLATE pg_catalog."default", 
	CONSTRAINT classifier_pk PRIMARY KEY (uid),
	UNIQUE (type_uid, code)
)
WITH (
	OIDS = FALSE
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.classifier TO web;

ALTER TABLE public.classifier
    ADD CONSTRAINT fk_classifier_type_uid FOREIGN KEY (type_uid)
    REFERENCES public.classifier_type (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

ALTER TABLE public.classifier
    ADD CONSTRAINT fk_classifier_parent_uid FOREIGN KEY (parent_uid)
    REFERENCES public.classifier (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;

-- DROP TABLE public.classifier_link;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_link'

CREATE TABLE IF NOT EXISTS public.classifier_link
(
	group_uid uuid NOT NULL, 
	item_uid uuid NOT NULL,
	CONSTRAINT classifier_link_pk PRIMARY KEY (group_uid, item_uid)
)
WITH (
	OIDS = FALSE
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.classifier_link TO web;

ALTER TABLE public.classifier_link
    ADD CONSTRAINT fk_classifier_link_group_uid FOREIGN KEY (group_uid)
    REFERENCES public.classifier_group (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
	
ALTER TABLE public.classifier_link
    ADD CONSTRAINT fk_classifier_link_item_uid FOREIGN KEY (item_uid)
    REFERENCES public.classifier (uid) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
	
-- DROP TABLE public.classifier_closure;
-- SELECT * FROM information_schema.tables WHERE table_name = 'classifier_closure'

CREATE TABLE IF NOT EXISTS public.classifier_closure
(
	parent_uid uuid NOT NULL, 
	child_uid uuid NOT NULL,
	level smallint NOT NULL,
	CONSTRAINT classifier_closure_pk PRIMARY KEY (parent_uid, child_uid, level)
)
WITH (
	OIDS = FALSE
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.classifier_closure TO web;
	