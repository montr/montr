import * as React from "react";
import { Input, InputNumber, Select, Checkbox, Icon } from "antd";
import { IDataField, IIndexer, ISelectField, ITextAreaField, INumberField } from "../models";

export abstract class DataFieldFactory {
	private static Map: { [key: string]: DataFieldFactory; } = {};

	static register(key: string, factory: DataFieldFactory) {
		DataFieldFactory.Map[key] = factory;
	}

	static get(key: string): DataFieldFactory {
		return DataFieldFactory.Map[key];
	}

	public valuePropName: string = "value";

	abstract createNode(field: IDataField, data: IIndexer): React.ReactNode;
}

class BooleanFieldFactory extends DataFieldFactory {
	constructor() {
		super();
		this.valuePropName = "checked";
	}

	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		return <Checkbox>{field.name}</Checkbox>;
	}
}

class StringFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		return <Input
			allowClear
			disabled={field.readonly}
			placeholder={field.placeholder}
			prefix={field.icon && <Icon type={field.icon} style={{ color: "rgba(0,0,0,.25)" }} />}
		/>;
	}
}

class NumberFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		const numberField = field as INumberField;

		return <InputNumber
			min={numberField.min}
			max={numberField.max}
			disabled={field.readonly}
			placeholder={field.placeholder}
		/>;
	}
}

class TextAreaFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		const textAreaField = field as ITextAreaField;

		return <Input.TextArea
			allowClear
			placeholder={field.placeholder}
			autoSize={{ minRows: textAreaField?.rows || 4, maxRows: 24 }}
		/>;
	}
}

class SelectFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		const selectField = field as ISelectField;

		return <Select allowClear showSearch placeholder={field.placeholder}>
			{selectField && selectField.options && selectField.options.map(x => {
				return <Select.Option key={x.value} value={x.value}>{x.name || x.value}</Select.Option>;
			})}
		</Select>;
	}
}

class PasswordFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		return <Input.Password
			allowClear
			placeholder={field.placeholder}
			prefix={field.icon && <Icon type={field.icon} style={{ color: "rgba(0,0,0,.25)" }} />}
		/>;
	}
}

DataFieldFactory.register("boolean", new BooleanFieldFactory());
DataFieldFactory.register("number", new NumberFieldFactory());
DataFieldFactory.register("string", new StringFieldFactory());
DataFieldFactory.register("textarea", new TextAreaFieldFactory());
DataFieldFactory.register("select", new SelectFieldFactory());
DataFieldFactory.register("password", new PasswordFieldFactory());
