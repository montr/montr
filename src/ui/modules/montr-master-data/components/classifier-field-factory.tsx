import * as React from "react";
import { DataFieldFactory } from "@montr-core/components";
import { IIndexer } from "@montr-core/models";
import { ClassifierGroupSelect, ClassifierSelect, SelectClassifierType } from ".";
import { ISelectClassifierTypeField, IClassifierField, IClassifierGroupField } from "../models";

export class ClassifierFieldFactory extends DataFieldFactory<IClassifierField> {
	createEditNode(field: IClassifierField, data: IIndexer): React.ReactElement {
		return <ClassifierSelect field={field} />;
	}
}

export class ClassifierGroupFieldFactory extends DataFieldFactory<IClassifierGroupField> {
	createEditNode(field: IClassifierGroupField, data: IIndexer): React.ReactElement {
		return <ClassifierGroupSelect field={field} />;
	}
}

export class SelectClassifierTypeFieldFactory extends DataFieldFactory<ISelectClassifierTypeField> {
	createEditNode(field: ISelectClassifierTypeField, data: IIndexer): React.ReactElement {
		return <SelectClassifierType />;
	}
}
