import { FieldData } from "rc-field-form/lib/interface";

export interface DataFormChanges {
	changedValues?: unknown;
	values?: unknown;
	changedFields?: FieldData[];
	allFields?: FieldData[];
}
