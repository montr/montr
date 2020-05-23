import { IDataField, IDataFieldWithProps, Guid } from "@montr-core/models";

export interface IClassifierField extends IDataFieldWithProps<IClassifierFieldProps> {
}

interface IClassifierFieldProps {
	typeCode?: string;
}

export interface IClassifierGroupField extends IDataFieldWithProps<IClassifierGroupFieldProps> {
}

interface IClassifierGroupFieldProps {
	typeCode?: string;
	treeCode?: string;
	treeUid?: Guid;
}

export interface IClassifierTypeField extends IDataField {
}
