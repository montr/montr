import { IDataField } from "@montr-core/models/data-field";

export interface SettingsBlock {
	typeCode: string;
	displayName: string;
	fields: IDataField[];
}
