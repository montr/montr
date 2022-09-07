SET search_path = 'montr';

DO
$$
BEGIN
	
	if exists (select * from information_schema.columns 
		where table_name = 'automation' and column_name = 'type_code') then
		
		alter table montr.automation
		rename column type_code to automation_type_code;
	
	end if;

END
$$;

RESET search_path;

