import * as React from "react";
import { Form, Spin } from "antd";
import { FormInstance } from "antd/lib/form";
import { Store, StoreValue, Rule } from "rc-field-form/lib/interface";
import { IDataField, IIndexer, IApiResult } from "../models";
import { NotificationService } from "../services/notification-service";
import { OperationService, DataHelper } from "../services";
import { FormDefaults, DataFieldFactory, ButtonSave, Toolbar, ButtonCancel } from ".";
import { withTranslation, WithTranslation } from "react-i18next";

declare const FormLayouts: ["horizontal", "inline", "vertical"];

interface IProps extends WithTranslation {
	layout?: (typeof FormLayouts)[number];
	fields: IDataField[];
	data: any; // IIndexer;
	showControls?: boolean;
	submitButton?: string;
	resetButton?: string;
	successMessage?: string;
	errorMessage?: string;
	hideLabels?: boolean;
	onChange?: (values: IIndexer, changedValues: IIndexer) => void;
	onSubmit?: (values: IIndexer) => Promise<IApiResult>;
	formRef?: React.RefObject<FormInstance>;
}

interface IState {
	loading: boolean;
}

export class WrappedDataForm extends React.Component<IProps, IState> {

	private _operation = new OperationService();
	private _notificationService = new NotificationService();

	private _isMounted: boolean = true;
	private _formRef = React.createRef<FormInstance>();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: false
		};
	}

	componentWillUnmount = () => {
		this._isMounted = false;
	};

	getFormRef = (): React.RefObject<FormInstance> => {
		return (this.props.formRef ?? this._formRef);
	};

	handleValuesChange = async (changedValues: Store, values: Store) => {
		const { onChange } = this.props;

		// console.log("Form.onChange", changedValues, values);

		if (onChange) {
			// var values = this.getFormRef().current.getFieldsValue();

			onChange(values, changedValues);
		}
	};

	handleSubmit = async (values: IIndexer) => {

		const { t, onSubmit, successMessage, errorMessage } = this.props;

		// console.log("Form.onFinish", this.getFormRef().current.getFieldsValue());

		if (this._isMounted) this.setState({ loading: true });

		await this._operation.execute(() => onSubmit(values), {
			successMessage: successMessage || t("dataForm.submit.success"),
			errorMessage: errorMessage || t("dataForm.submit.error"),
			showFieldErrors: async (result) => {
				await this.setFieldErrors(result, values);
			}
		});

		if (this._isMounted) this.setState({ loading: false });
	};

	setFieldErrors = async (result: IApiResult, values: IIndexer) => {
		const { fields } = this.props,
			fieldErrors: any = {},
			otherErrors: string[] = [];

		if (result && result.errors) {
			result.errors.forEach(error => {
				// todo: check key exists in state.fields (ignore case + add tests)
				const field = fields.find(x => x.key && error.key && x.key.toLowerCase() == error.key.toLowerCase());
				if (field) {
					fieldErrors[field.key] = {
						value: values[field.key],
						errors: error.messages.map((x: string) => new Error(x))
					};
				}
				else {
					error.messages.forEach(message => {
						otherErrors.push(message);
					});
				}
			});

			this.getFormRef().current.setFields(fieldErrors);

			if (otherErrors.length > 0) {
				// todo: show as alert before form
				this._notificationService.error(otherErrors);
			}
		}
	};

	createItem = (field: IDataField): React.ReactNode => {
		const { t, layout, data, hideLabels } = this.props;

		// const initialValue = data?.[field.key];
		// const initialValue = DataHelper.indexer(data, field.key, undefined);

		const fieldFactory = DataFieldFactory.get(field.type);

		if (!fieldFactory) return null;

		const required: Rule = {
			required: field.required,
			message: t("dataForm.rule.required", { name: field.name })
		};

		if (field.type == "text" || field.type == "textarea") {
			required.whitespace = field.required;
		}

		/* const fieldOptions: GetFieldDecoratorOptions = {
			initialValue: initialValue,
			valuePropName: fieldFactory.valuePropName
		}; */

		// todo: why boolean can't be required?
		let rules: Rule[];
		if (field.type != "boolean") {
			rules = [required];
		}

		if (field.type == "boolean") {
			// todo: fix server value convert
			// fieldOptions.initialValue = fieldOptions.initialValue === "true" || fieldOptions.initialValue === true;
			const value = DataHelper.indexer(data, field.key, undefined);
			DataHelper.indexer(data, field.key, value === "true" || value === true);
		}

		const fieldNode = fieldFactory.createNode(field, data);

		const itemLayout = (layout == null || layout == "horizontal")
			? (field.type == "boolean" ? FormDefaults.tailFormItemLayout : FormDefaults.formItemLayout)
			: {};

		return (
			<Form.Item
				key={field.key}
				htmlFor={field.key}
				name={field.key.split(".")}
				label={hideLabels || field.type == "boolean" ? null : field.name}
				extra={field.description}
				valuePropName={fieldFactory.valuePropName}
				rules={rules}
				{...itemLayout}>
				{fieldNode}
			</Form.Item>
		);
	};

	render = () => {
		const { t, layout, data, fields, showControls, submitButton, resetButton } = this.props,
			{ loading } = this.state;

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.tailFormItemLayout : null;

		// console.log("Form.render", data, fields);

		return (
			<Spin spinning={loading}>
				{fields && <Form
					ref={this.getFormRef()}
					initialValues={data}
					layout={layout || "horizontal"}
					onValuesChange={this.handleValuesChange}
					onFinish={this.handleSubmit}>

					{fields.map(x => this.createItem(x))}

					<Form.Item {...itemLayout} style={{ display: showControls === false ? "none" : "block" }}>
						<Toolbar>
							<ButtonSave htmlType="submit">{submitButton}</ButtonSave>
							<ButtonCancel htmlType="reset">{resetButton}</ButtonCancel>
						</Toolbar>
					</Form.Item>
				</Form>}
			</Spin>
		);
	};
}

/* export const DataForm = withTranslation()(Form.create<IProps>({
	// Form.onChange is not working - not triggered when Select field changes
	// https://github.com/ant-design/ant-design/issues/18867
	onValuesChange: (props, values, allFieldsValues) => {
		if (props.onChange) {
			props.onChange(allFieldsValues);
		}
	}
})(WrappedDataForm)); */

export const DataForm = withTranslation()(WrappedDataForm);
