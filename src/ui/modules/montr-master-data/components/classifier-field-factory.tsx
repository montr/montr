import * as React from "react";
import { DataFieldFactory } from "@montr-core/components";
import { IIndexer, IDataField, IClassifierField, IClassifierGroupField } from "@montr-core/models";
import { ClassifierGroupSelect, ClassifierSelect } from ".";

export class ClassifierGroupFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactElement {
		return <ClassifierGroupSelect field={field as IClassifierGroupField} />;
	}
}

export class ClassifierFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactElement {
		return <ClassifierSelect field={field as IClassifierField} />;
	}
}

DataFieldFactory.register("classifier-group", new ClassifierGroupFieldFactory());
DataFieldFactory.register("classifier", new ClassifierFieldFactory());
