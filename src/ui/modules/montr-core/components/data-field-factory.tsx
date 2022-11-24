import { Checkbox, DatePicker, Form, Input, InputNumber, Select } from "antd";
import dayjs from "dayjs";
import { Rule } from "rc-field-form/lib/interface";
import * as React from "react";
import { DataFormOptions, FormDefaults, Icon } from ".";
import { IBooleanField, IDataField, IDateField, IDesignSelectOptionsField, IIndexer, INumberField, IPasswordField, ISelectField, ITextAreaField, ITextField } from "../models";
import { DataHelper } from "../services/data-helper";
import { DesignSelectOptions } from "./design-select-options";
import { EmptyFieldView } from "./empty-field-view";

export abstract class DataFieldFactory<TField extends IDataField> {
	private static Map: { [key: string]: DataFieldFactory<IDataField>; } = {};

	static register(key: string, factory: DataFieldFactory<IDataField>) {
		DataFieldFactory.Map[key] = factory;
	}

	static get(key: string): DataFieldFactory<IDataField> {
		const factory = DataFieldFactory.Map[key];

		if (!factory) {
			// todo: display default placeholder for not found field type (?)
			console.warn(`Warning: Field type '${key}' is not found.`);
		}

		return factory;
	}

	valuePropName = "value";

	shouldFormatValue = false;

	createFormItem = (field: Partial<TField>, data: IIndexer, options: DataFormOptions): React.ReactNode => {
		const { t, layout, mode, hideLabels } = options;

		if (this.shouldFormatValue) {
			const value = DataHelper.indexer(data, field.key, undefined);
			const formattedValue = this.formatValue(field, data, options, value);
			DataHelper.indexer(data, field.key, formattedValue);
		}

		const fieldNode = (mode == "view")
			? this.createViewNode(field, data)
			: this.createEditNode(field, data);

		const itemLayout = (layout == null || layout == "horizontal")
			? (mode != "view" && field.type == "boolean" ? FormDefaults.tailFormItemLayout : FormDefaults.formItemLayout)
			: {};

		const namePath: (string | number)[] = [];
		if (options.namePathPrefix)
			namePath.push(...options.namePathPrefix);
		namePath.push(...field.key.split("."));

		const key = namePath.join(".");

		if (mode == "view") {
			return (
				<Form.Item
					key={key}
					label={hideLabels ? null : field.name}
					extra={field.description}
					valuePropName={this.valuePropName}
					{...itemLayout}>
					{fieldNode}
				</Form.Item>
			);
		}

		return (
			<Form.Item
				key={key}
				name={namePath}
				label={hideLabels || (field.type == "boolean") ? null : field.name}
				extra={field.description}
				valuePropName={this.valuePropName}
				tooltip={field.tooltip}
				rules={this.createFormItemRules(field, options)}
				{...itemLayout}>
				{fieldNode}
			</Form.Item>
		);
	};

	createFormItemRules(field: Partial<TField>, options: DataFormOptions): Rule[] {
		const { t } = options;

		const required: Rule = {
			required: field.required,
			message: t("dataForm.rule.required", { name: field.name })
		};

		if (field.type == "text" || field.type == "textarea" || field.type == "password") {
			required.whitespace = field.required;
		}

		return [required];
	}

	formatValue(field: Partial<TField>, data: IIndexer, options: DataFormOptions, value: any): any {
		return value;
	}

	createViewNode(field: Partial<TField>, data: IIndexer): React.ReactElement {
		const value = DataHelper.indexer(data, field.key, undefined);

		return (value != undefined) ? value : <EmptyFieldView />;
	}

	abstract createEditNode(field: Partial<TField>, data: IIndexer): React.ReactElement;
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
			{props?.options?.map(x => {
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

	formatValue(field: IDateField, data: IIndexer, options: DataFormOptions, value: any): any {
		const { mode } = options;

		if (mode == "view") return value;

		return value ? dayjs(value) : null;
	}

	createEditNode(field: IDateField, data: IIndexer): React.ReactElement {
		const props = field?.props;

		return <DatePicker
			// allowClear
			showTime={props?.includeTime}
			disabled={field.readonly}
			placeholder={field.placeholder}
		/>;
	}

	createViewNode(field: IDateField, data: IIndexer): React.ReactElement {
		const value = DataHelper.indexer(data, field.key, undefined);

		return (value != undefined)
			? value // .format(field.props.includeTime ? "LLL" : "L")
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
