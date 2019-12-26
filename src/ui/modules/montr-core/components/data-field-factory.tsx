import * as React from "react";
import { Input, Select, Checkbox, Icon } from "antd";
import { IDataField, IIndexer, ISelectField, ITextAreaField } from "../models";

export abstract class DataFieldFactory {
	private static Map: { [key: string]: DataFieldFactory; } = {};

	static register(key: string, factory: DataFieldFactory) {
		DataFieldFactory.Map[key] = factory;
	}

	static get(key: string): DataFieldFactory {
		return DataFieldFactory.Map[key];
	}

	abstract createNode(field: IDataField, data: IIndexer): React.ReactNode;
}

class BooleanFieldFactory implements DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		return <Checkbox>{field.name}</Checkbox>;
	}
}

class StringFieldFactory implements DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		return <Input
			allowClear
			disabled={field.readonly}
			placeholder={field.placeholder}
			prefix={field.icon && <Icon type={field.icon} style={{ color: "rgba(0,0,0,.25)" }} />}
		/>;
	}
}

class TextAreaFieldFactory implements DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {

		const textAreaField = field as ITextAreaField;

		const minRows = textAreaField.rows || 4;

		return <Input.TextArea allowClear placeholder={field.placeholder} autoSize={{ minRows: minRows, maxRows: 24 }} />;
	}
}

class SelectFieldFactory implements DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {

		const selectField = field as ISelectField;

		return (
			<Select allowClear showSearch placeholder={field.placeholder}>
				{selectField && selectField.options && selectField.options.map(x => {
					return <Select.Option key={x.value} value={x.value}>{x.name || x.value}</Select.Option>;
				})}
			</Select>
		);
	}
}

class PasswordFieldFactory implements DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		return <Input.Password
			allowClear
			placeholder={field.placeholder}
			prefix={field.icon && <Icon type={field.icon} style={{ color: "rgba(0,0,0,.25)" }} />}
		/>;
	}
}

DataFieldFactory.register("boolean", new BooleanFieldFactory());
DataFieldFactory.register("string", new StringFieldFactory());
DataFieldFactory.register("textarea", new TextAreaFieldFactory());
DataFieldFactory.register("select", new SelectFieldFactory());
DataFieldFactory.register("password", new PasswordFieldFactory());
