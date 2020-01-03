import * as React from "react";
import { Input, InputNumber, Select, Checkbox, Icon, DatePicker, TimePicker } from "antd";
import { IDataField, IIndexer, ISelectField, ITextAreaField, INumberField, IDateField } from "../models";

// todo: rename after migrate to antd 4.0
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

class TextFieldFactory extends DataFieldFactory {
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
		const props = numberField?.props;

		return <InputNumber
			min={props?.min}
			max={props?.max}
			disabled={field.readonly}
			placeholder={field.placeholder}
		/>;
	}
}

class TextAreaFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		const textAreaField = field as ITextAreaField;
		const props = textAreaField?.props;

		return <Input.TextArea
			allowClear
			placeholder={field.placeholder}
			autoSize={{ minRows: props?.rows || 4, maxRows: 24 }}
		/>;
	}
}

class SelectFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		const selectField = field as ISelectField;
		const props = selectField?.props;

		return <Select allowClear showSearch placeholder={field.placeholder}>
			{props?.options.map(x => {
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

class DateFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		const dateField = field as IDateField;
		const props = dateField?.props;

		return <DatePicker
			allowClear
			showTime={props?.includeTime}
			disabled={field.readonly}
			placeholder={field.placeholder}
		/>;
	}
}

class TimeFieldFactory extends DataFieldFactory {
	createNode(field: IDataField, data: IIndexer): React.ReactNode {
		return <TimePicker
			allowClear
			disabled={field.readonly}
			placeholder={field.placeholder}
		/>;
	}
}

DataFieldFactory.register("boolean", new BooleanFieldFactory());
DataFieldFactory.register("number", new NumberFieldFactory());
DataFieldFactory.register("text", new TextFieldFactory());
DataFieldFactory.register("textarea", new TextAreaFieldFactory());
DataFieldFactory.register("select", new SelectFieldFactory());
DataFieldFactory.register("password", new PasswordFieldFactory());
DataFieldFactory.register("date", new DateFieldFactory());
DataFieldFactory.register("time", new TimeFieldFactory());
