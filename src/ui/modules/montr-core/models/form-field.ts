import { Guid } from ".";

export interface IFormField {
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

export interface IStringField extends IFormField {
	autosize: boolean;
}

export interface ITextAreaField extends IStringField {
	rows?: number;
}

export interface IPasswordField extends IStringField {
}

export interface INumberField extends IFormField {
	min?: number;
	max?: number;
}

export interface IDecimalField extends IFormField {
	min?: number;
	max?: number;
	precision?: number;
}

export interface IDateField extends IFormField {
}

export interface ITimeField extends IFormField {
}

export interface IDateTimeField extends IFormField {
}

export interface ISelectField extends IFormField {
	options: IOption[];
}

export interface IOption {
	value: string;
	name: string;
}

export interface IClassifierGroupField extends IFormField {
	typeCode: string;
	treeCode: string;
	treeUid: Guid;
}

export interface IClassifierField extends IFormField {
	typeCode: string;
}

export interface IFileField extends IFormField {
}
