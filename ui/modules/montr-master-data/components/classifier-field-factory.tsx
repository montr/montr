import * as React from "react";
import { FormFieldFactory } from "@montr-core/components";
import { IIndexer, IFormField, IClassifierField } from "@montr-core/models";
import { ClassifierSelect } from ".";

export class ClassifierFieldFactory implements FormFieldFactory {
	createNode(field: IFormField, data: IIndexer): React.ReactNode {
		return <ClassifierSelect field={field as IClassifierField} />;
	}
}

FormFieldFactory.register("classifier", new ClassifierFieldFactory());
