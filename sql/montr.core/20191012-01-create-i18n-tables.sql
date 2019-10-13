-- Table: public.locale_string

-- DROP TABLE public.locale_string;

CREATE TABLE public.locale_string
(
    locale character varying(8) COLLATE pg_catalog."default" NOT NULL,
    module character varying(32) COLLATE pg_catalog."default" NOT NULL,
    key character varying(128) COLLATE pg_catalog."default" NOT NULL,
    value text COLLATE pg_catalog."default",
    CONSTRAINT locale_string_pkey PRIMARY KEY (locale, module, key)
);

GRANT INSERT, SELECT, UPDATE, DELETE ON TABLE public.locale_string TO web;

/*

insert into locale_string(locale, module, key, value)
values ('en', 'common', 'save.button', 'Save');

insert into locale_string(locale, module, key, value)
values ('ru', 'common', 'save.button', 'Сохранить');

insert into locale_string(locale, module, key, value)
values ('en', 'common', 'confirm.title', 'Confirm operation');

insert into locale_string(locale, module, key, value)
values ('ru', 'common', 'confirm.title', 'Подтверждение операции');

insert into locale_string(locale, module, key, value)
values ('en', 'common', 'operation.success', 'The operation completed successfully.');

insert into locale_string(locale, module, key, value)
values ('ru', 'common', 'operation.success', 'Операция выполнена успешно.');

insert into locale_string(locale, module, key, value)
values ('en', 'common', 'operation.error', 'An error occurred while performing the operation.');

insert into locale_string(locale, module, key, value)
values ('ru', 'common', 'operation.error', 'Произошла ошибка при выполнении операции.');

insert into locale_string(locale, module, key, value)
values ('en', 'tendr', 'publish.button', 'Publish');

insert into locale_string(locale, module, key, value)
values ('en', 'tendr', 'publish.confirm.content', 'Are you sure you want to publish the event?');

insert into locale_string(locale, module, key, value)
values ('en', 'tendr', 'cancel.button', 'Cancel');

insert into locale_string(locale, module, key, value)
values ('en', 'tendr', 'cancel.confirm.content', 'Are you sure you want to cancel the event?');

insert into locale_string(locale, module, key, value)
values ('ru', 'tendr', 'publish.button', 'Опубликовать');

insert into locale_string(locale, module, key, value)
values ('ru', 'tendr', 'publish.confirm.content', 'Вы действительно хотите опубликовать событие?');

insert into locale_string(locale, module, key, value)
values ('ru', 'tendr', 'cancel.button', 'Отменить');

insert into locale_string(locale, module, key, value)
values ('ru', 'tendr', 'cancel.confirm.content', 'Вы действительно хотите отменить событие?');

*/