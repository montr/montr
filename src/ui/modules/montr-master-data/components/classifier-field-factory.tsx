import * as React from "react";
import { FormFieldFactory } from "@montr-core/components";
import { IIndexer, IFormField, IClassifierField } from "@montr-core/models";
import { ClassifierGroupSelect } from ".";

export class ClassifierGroupFieldFactory implements FormFieldFactory {
	createNode(field: IFormField, data: IIndexer): React.ReactNode {
		return <ClassifierGroupSelect field={field as IClassifierField} />;
	}
}

export class ClassifierFieldFactory implements FormFieldFactory {
	createNode(field: IFormField, data: IIndexer): React.ReactNode {
		return <ClassifierGroupSelect field={field as IClassifierField} />;
	}
}

FormFieldFactory.register("classifier-group", new ClassifierGroupFieldFactory());
FormFieldFactory.register("classifier", new ClassifierFieldFactory());
