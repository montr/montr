ALTER TABLE montr.numerator_entity
    ALTER COLUMN numerator_uid DROP NOT NULL;

ALTER TABLE montr.numerator_entity
    ADD COLUMN is_auto_numbering boolean;

update montr.numerator_entity 
	set is_auto_numbering = true 
	where is_auto_numbering is null;

ALTER TABLE montr.numerator_entity
    ALTER COLUMN is_auto_numbering SET NOT NULL;
