DO
$$
BEGIN
    IF EXISTS (select * from information_schema.columns
		where table_schema = 'montr' and table_name = 'document' and column_name = 'uid'
	) AND NOT EXISTS (select * from information_schema.columns
		where table_schema = 'montr' and table_name = 'document' and column_name = 'document_number'
	) THEN
       ALTER TABLE montr.document DROP CONSTRAINT document_pk;
       ALTER TABLE montr.document RENAME TO document_20200229;
    END IF;
END
$$;
