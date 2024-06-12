import { Guid, IDataField, IDataFieldWithProps } from "@montr-core/models";

export interface IClassifierField extends IDataFieldWithProps<IClassifierFieldProps> {
}

interface IClassifierFieldProps {
	typeCode?: string;
	multiple?: boolean;
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
