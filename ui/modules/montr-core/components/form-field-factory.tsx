import * as React from "react";
import { Input, Select } from "antd";
import { IFormField, IIndexer, ISelectField } from "../models";

export abstract class FormFieldFactory {
	private static Map: { [key: string]: FormFieldFactory; } = {};

	static register(key: string, factory: FormFieldFactory) {
		FormFieldFactory.Map[key] = factory;
	}

	static get(key: string): FormFieldFactory {
		return FormFieldFactory.Map[key];
	}

	abstract createNode(field: IFormField, data: IIndexer): React.ReactNode;
}

class StringFieldFactory implements FormFieldFactory {
	createNode(field: IFormField, data: IIndexer): React.ReactNode {
		return <Input placeholder={field.placeholder} />;
	}
}

class TextAreaFieldFactory implements FormFieldFactory {
	createNode(field: IFormField, data: IIndexer): React.ReactNode {
		return <Input.TextArea placeholder={field.placeholder} autosize={{ minRows: 4, maxRows: 24 }} />;
	}
}

class SelectFieldFactory implements FormFieldFactory {
	createNode(field: IFormField, data: IIndexer): React.ReactNode {

		const selectField = field as ISelectField;

		return (
			<Select>
				{selectField && selectField.options && selectField.options.map(x => {
					return <Select.Option key={x.value} value={x.value}>{x.name || x.value}</Select.Option>
				})}
			</Select>
		);
	}
}

FormFieldFactory.register("string", new StringFieldFactory());
FormFieldFactory.register("textarea", new TextAreaFieldFactory());
FormFieldFactory.register("select", new SelectFieldFactory());
