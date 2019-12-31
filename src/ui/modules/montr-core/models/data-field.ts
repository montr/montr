import { Guid, IIndexer } from ".";

export interface IFieldType {
	code?: string;
	name?: string;
	icon?: string;
}

export interface IDataField /* extends IIndexer */ {
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
	properties?: TProps;
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

export interface IDateField extends IDataField {
}

export interface ITimeField extends IDataField {
}

export interface IDateTimeField extends IDataField {
}

export interface ISelectField extends IDataFieldWithProps<ISelectFieldProps> {
}

interface ISelectFieldProps {
	options: IOption[];
}

interface IOption {
	value: string;
	name: string;
}

export interface IClassifierGroupField extends IDataFieldWithProps<IClassifierGroupFieldProps> {
}

interface IClassifierGroupFieldProps {
	typeCode?: string;
	treeCode?: string;
	treeUid?: Guid;
}

export interface IClassifierField extends IDataFieldWithProps<IClassifierFieldProps> {
}

interface IClassifierFieldProps {
	typeCode?: string;
}

export interface IFileField extends IDataField {
}
