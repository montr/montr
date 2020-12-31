import * as React from "react";
import { Input, InputNumber, Select, Checkbox, DatePicker, Form } from "antd";
import { getFieldId } from "antd/lib/form/util";
import { Rule } from "rc-field-form/lib/interface";
import {
	IDataField, IIndexer, ISelectField, ITextAreaField, INumberField, IDateField, IBooleanField, ITextField,
	IDesignSelectOptionsField, IPasswordField
} from "../models";
import { Icon, DesignSelectOptions, DesignSelectOptionsFI, EmptyFieldView, DataFormOptions, FormDefaults } from ".";
import moment from "moment";
import { DataHelper } from "../services";

export abstract class DataFieldFactory<TField extends IDataField> {
	private static Map: { [key: string]: DataFieldFactory<IDataField>; } = {};

	static register(key: string, factory: DataFieldFactory<IDataField>) {
		DataFieldFactory.Map[key] = factory;
	}

	static get(key: string): DataFieldFactory<IDataField> {
		return DataFieldFactory.Map[key];
	}

	valuePropName: string = "value";

	shouldFormatValue: boolean = false;

	createFormItem = (field: TField, data: IIndexer, options: DataFormOptions): React.ReactNode => {
		const { t, layout, mode, hideLabels } = options;

		const required: Rule = {
			required: field.required,
			message: t("dataForm.rule.required", { name: field.name })
		};

		if (field.type == "text" || field.type == "textarea" || field.type == "password") {
			required.whitespace = field.required;
		}

		if (this.shouldFormatValue) {
			const value = DataHelper.indexer(data, field.key, undefined);
			const formattedValue = this.formatValue(field, data, value);
			DataHelper.indexer(data, field.key, formattedValue);
		}

		const fieldNode = (mode == "View")
			? this.createViewNode(field, data)
			: this.createEditNode(field, data);

		const itemLayout = (layout == null || layout == "horizontal")
			? (mode != "View" && field.type == "boolean" ? FormDefaults.tailFormItemLayout : FormDefaults.formItemLayout)
			: {};

		if (mode == "View") {
			return (
				<Form.Item
					key={field.key}
					label={hideLabels ? null : field.name}
					extra={field.description}
					valuePropName={this.valuePropName}
					rules={[required]}
					{...itemLayout}>
					{fieldNode}
				</Form.Item>
			);
		}

		const namePath = field.key.split(".");

		return (
			<Form.Item
				key={field.key}
				name={namePath}
				htmlFor={getFieldId(namePath)} // replace(".", "_")
				label={hideLabels || (field.type == "boolean") ? null : field.name}
				extra={field.description}
				valuePropName={this.valuePropName}
				rules={[required]}
				{...itemLayout}>
				{fieldNode}
			</Form.Item>
		);
	};

	formatValue(field: TField, data: IIndexer, value: any): any {
		return value;
	}

	createViewNode(field: TField, data: IIndexer): React.ReactElement {
		const value = DataHelper.indexer(data, field.key, undefined);

		return (value != undefined) ? value : <EmptyFieldView />;
	}

	abstract createEditNode(field: TField, data: IIndexer): React.ReactElement;
}

export class BooleanFieldFactory extends DataFieldFactory<IBooleanField> {
	constructor() {
		super();
		this.valuePropName = "checked";
	}

	createEditNode(field: IBooleanField, data: IIndexer): React.ReactElement {
		return <Checkbox>{field.name}</Checkbox>;
	}
}

export class TextFieldFactory extends DataFieldFactory<ITextField> {
	createEditNode(field: ITextField, data: IIndexer): React.ReactElement {
		return <Input
			allowClear
			disabled={field.readonly}
			placeholder={field.placeholder}
			prefix={field.icon && Icon.get(field.icon)}
		/>;
	}
}

export class NumberFieldFactory extends DataFieldFactory<INumberField> {
	createEditNode(field: INumberField, data: IIndexer): React.ReactElement {
		const props = field?.props;

		return <InputNumber
			min={props?.min}
			max={props?.max}
			disabled={field.readonly}
			placeholder={field.placeholder}
		/>;
	}
}

export class TextAreaFieldFactory extends DataFieldFactory<ITextAreaField> {
	createEditNode(field: ITextAreaField, data: IIndexer): React.ReactElement {
		const props = field?.props;

		return <Input.TextArea
			allowClear
			placeholder={field.placeholder}
			autoSize={{ minRows: props?.rows || 4, maxRows: 12 }}
		/>;
	}
}

export class SelectFieldFactory extends DataFieldFactory<ISelectField> {
	createEditNode(field: ISelectField, data: IIndexer): React.ReactElement {
		const props = field?.props;

		return <Select
			// allowClear
			showSearch
			placeholder={field?.placeholder}
			style={{ minWidth: 200, width: "auto" }}>
			{props?.options.map(x => {
				return <Select.Option key={x.value} value={x.value}>{x.name || x.value}</Select.Option>;
			})}
		</Select>;
	}

	createViewNode(field: ISelectField, data: IIndexer): React.ReactElement {
		const value = DataHelper.indexer(data, field.key, undefined);

		const option = field.props.options.find(x => x.value == value);

		if (option) {
			return <>{option.name}</>;
		}

		return (value) ? value : <EmptyFieldView />;
	}
}

export class DesignSelectOptionsFieldFactory extends DataFieldFactory<IDesignSelectOptionsField> {

	/* createFormItem = (field: IDesignSelectOptionsField, data: IIndexer, options: IDataFormOptions): React.ReactNode => {
		return <DesignSelectOptions key={field.key} />;
	}; */

	createEditNode(field: IDesignSelectOptionsField, data: IIndexer): React.ReactElement {
		return <DesignSelectOptions field={field} />;
	}
}

export class PasswordFieldFactory extends DataFieldFactory<IPasswordField> {
	createEditNode(field: IDataField, data: IIndexer): React.ReactElement {
		return <Input.Password
			allowClear
			placeholder={field.placeholder}
			prefix={field.icon && Icon.get(field.icon)}
		/>;
	}
}

export class DateFieldFactory extends DataFieldFactory<IDateField> {
	constructor() {
		super();
		this.shouldFormatValue = true;
	}

	formatValue(field: IDateField, data: IIndexer, value: any): any {
		return value ? moment.parseZone(value) : null;
	}

	createEditNode(field: IDateField, data: IIndexer): React.ReactElement {
		const props = field?.props;

		return <DatePicker
			allowClear
			showTime={props?.includeTime}
			disabled={field.readonly}
			placeholder={field.placeholder}
		/>;
	}

	createViewNode(field: IDateField, data: IIndexer): React.ReactElement {
		const value = DataHelper.indexer(data, field.key, undefined);

		return (value != undefined)
			? value.format(field.props.includeTime ? "LLL" : "L")
			: <EmptyFieldView />;
	}
}

/* export class TimeFieldFactory extends DataFieldFactory<ITimeField> {
	constructor() {
		super();
		this.shouldFormatValue = true;
	}

	formatValue(field: ITimeField, data: IIndexer, value: any): any {
		return value ? moment.parseZone(value, "HH:mm:ss") : null;
	}

	createEditNode(field: ITimeField, data: IIndexer): React.ReactElement {
		return <TimePicker
			allowClear
			disabled={field.readonly}
			placeholder={field.placeholder}
		/>;
	}

	createViewNode(field: ITimeField, data: IIndexer): React.ReactElement {
		const value = DataHelper.indexer(data, field.key, undefined);

		return (value != undefined)
			? value.format("LTS")
			: <EmptyFieldView />;
	}
} */
