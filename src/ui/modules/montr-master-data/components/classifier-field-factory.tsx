import * as React from "react";
import { DataFieldFactory } from "@montr-core/components";
import { IIndexer } from "@montr-core/models";
import { ClassifierGroupSelect, ClassifierSelect, ClassifierTypeSelect } from ".";
import { IClassifierTypeField, IClassifierField, IClassifierGroupField } from "../models";
import { ClassifierView } from "./classifier-view";

export class ClassifierFieldFactory extends DataFieldFactory<IClassifierField> {
	createEditNode(field: IClassifierField, data: IIndexer): React.ReactElement {
		return <ClassifierSelect field={field} />;
	}
	createViewNode(field: IClassifierField, data: IIndexer): React.ReactElement {
		return <ClassifierView field={field} data={data} />;
	}
}

export class ClassifierGroupFieldFactory extends DataFieldFactory<IClassifierGroupField> {
	createEditNode(field: IClassifierGroupField, data: IIndexer): React.ReactElement {
		return <ClassifierGroupSelect field={field} />;
	}
}

export class SelectClassifierTypeFieldFactory extends DataFieldFactory<IClassifierTypeField> {
	createEditNode(field: IClassifierTypeField, data: IIndexer): React.ReactElement {
		return <ClassifierTypeSelect field={field} />;
	}
}
