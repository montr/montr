import { FieldData } from "./";

export interface DataFormChanges {
	changedValues?: unknown;
	values?: unknown;
	changedFields?: FieldData[];
	allFields?: FieldData[];
}
