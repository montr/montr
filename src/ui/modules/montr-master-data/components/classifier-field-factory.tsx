import * as React from "react";
import { DataFieldFactory } from "@montr-core/components";
import { IIndexer, IClassifierField, IClassifierGroupField, ISelectClassifierTypeField } from "@montr-core/models";
import { ClassifierGroupSelect, ClassifierSelect } from ".";
import { SelectClassifierType } from "./select-classifier-type";

export class ClassifierFieldFactory extends DataFieldFactory<IClassifierField> {
	createNode(field: IClassifierField, data: IIndexer): React.ReactElement {
		return <ClassifierSelect field={field} />;
	}
}

export class ClassifierGroupFieldFactory extends DataFieldFactory<IClassifierGroupField> {
	createNode(field: IClassifierGroupField, data: IIndexer): React.ReactElement {
		return <ClassifierGroupSelect field={field} />;
	}
}

export class SelectClassifierTypeFieldFactory extends DataFieldFactory<ISelectClassifierTypeField> {
	createNode(field: ISelectClassifierTypeField, data: IIndexer): React.ReactElement {
		return <SelectClassifierType />;
	}
}

DataFieldFactory.register("classifier-group", new ClassifierGroupFieldFactory());
DataFieldFactory.register("classifier", new ClassifierFieldFactory());
DataFieldFactory.register("select-classifier-type", new SelectClassifierTypeFieldFactory());
