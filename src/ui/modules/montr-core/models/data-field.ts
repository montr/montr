import { Guid } from ".";

export interface IDataField {
	uid?: Guid;
	type: string;
	key: string;
	name: string;
	description?: string;
	placeholder?: string;
	icon?: string;
	// multiple?: boolean;
	readonly?: boolean;
	required?: boolean;
}

export interface IStringField extends IDataField {
	autosize: boolean;
}

export interface ITextAreaField extends IStringField {
	rows?: number;
}

export interface IPasswordField extends IStringField {
}

export interface INumberField extends IDataField {
	min?: number;
	max?: number;
}

export interface IDecimalField extends IDataField {
	min?: number;
	max?: number;
	precision?: number;
}

export interface IDateField extends IDataField {
}

export interface ITimeField extends IDataField {
}

export interface IDateTimeField extends IDataField {
}

export interface ISelectField extends IDataField {
	options: IOption[];
}

export interface IOption {
	value: string;
	name: string;
}

export interface IClassifierGroupField extends IDataField {
	typeCode: string;
	treeCode: string;
	treeUid: Guid;
}

export interface IClassifierField extends IDataField {
	typeCode: string;
}

export interface IFileField extends IDataField {
}
