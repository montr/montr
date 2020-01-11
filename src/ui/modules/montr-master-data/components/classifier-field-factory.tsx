import * as React from "react";
import { DataFieldFactory } from "@montr-core/components";
import { IIndexer, IClassifierField, IClassifierGroupField } from "@montr-core/models";
import { ClassifierGroupSelect, ClassifierSelect } from ".";

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

DataFieldFactory.register("classifier-group", new ClassifierGroupFieldFactory());
DataFieldFactory.register("classifier", new ClassifierFieldFactory());
