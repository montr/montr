import { Guid } from ".";

export interface IFieldType {
	code?: string;
	name?: string;
	icon?: string;
}

export interface IDataField {
	uid?: Guid;
	type: string;
	key?: string;
	name?: string;
	description?: string;
	placeholder?: string;
	icon?: string;
	// multiple?: boolean;
	active?: boolean;
	readonly?: boolean;
	required?: boolean;
	displayOrder?: number;
}

export interface IDataFieldWithProps<TProps> extends IDataField {
	props?: TProps;
}

export interface IBooleanField extends IDataField {
}

export interface ITextField extends IDataField {
}

export interface ITextAreaField extends IDataFieldWithProps<ITextAreaFieldProps> {
}

interface ITextAreaFieldProps {
	rows?: number;
}

export interface IPasswordField extends IDataField {
}

export interface INumberField extends IDataFieldWithProps<INumberFieldProps> {
}

interface INumberFieldProps {
	min?: number;
	max?: number;
}

export interface IDecimalField extends IDataFieldWithProps<IDecimalFieldProps> {
}

interface IDecimalFieldProps {
	min?: number;
	max?: number;
	precision?: number;
}

export interface IDateField extends IDataFieldWithProps<IDateFieldProps> {
}

interface IDateFieldProps {
	includeTime?: boolean;
}

/* export interface ITimeField extends IDataField {
} */

export interface IAutomationConditionField extends IDataField {
}

export interface IAutomationActionListField extends IDataField {
}

export interface ISelectField extends IDataFieldWithProps<ISelectFieldProps> {
}

interface ISelectFieldProps {
	options: IOption[];
}

export interface IOption {
	value: string;
	name: string;
}

export interface IDesignSelectOptionsField extends IDataField {
}

export interface IFileField extends IDataField {
}
