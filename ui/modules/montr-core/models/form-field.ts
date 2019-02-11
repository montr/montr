export interface IFormField {
	type: string;
	key: string;
	name: string;
	description: string;
	placeholder: string;
	multiple: boolean;
	readonly: boolean;
	required: boolean;
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

export interface IClassifierField extends IFormField {
}

export interface IFileField extends IFormField {
}
