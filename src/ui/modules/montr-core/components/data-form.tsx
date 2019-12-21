import * as React from "react";
import { Form, Button, Spin } from "antd";
import { FormInstance } from "antd/lib/form";
import { IDataField, IIndexer, IApiResult } from "../models";
import { NotificationService } from "../services/notification-service";
import { OperationService } from "../services";
import { FormDefaults, DataFieldFactory, Icon } from ".";
import { withTranslation, WithTranslation } from "react-i18next";

declare const FormLayouts: ["horizontal", "inline", "vertical"];

interface IProps extends WithTranslation {
	layout?: (typeof FormLayouts)[number];
	fields: IDataField[];
	data: IIndexer;
	showControls?: boolean;
	submitButton?: string;
	resetButton?: string;
	successMessage?: string;
	errorMessage?: string;
	hideLabels?: boolean;
	onChange?: (values: IIndexer) => void;
	onSubmit: (values: IIndexer) => Promise<IApiResult>;
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

	getFieldValue = async (fieldName: string) => {
		return this._formRef.current.getFieldValue(fieldName);
	};

	handleChange = async (e: React.SyntheticEvent) => {
		const { onChange } = this.props;

		if (onChange) {
			var values = this._formRef.current.getFieldsValue();

			onChange(values);
		}
	};

	handleSubmit = async (values: IIndexer) => {

		const { t, onSubmit: onSave, successMessage, errorMessage } = this.props;

		if (this._isMounted) this.setState({ loading: true });

		await this._operation.execute(() => onSave(values), {
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

			this._formRef.current.setFields(fieldErrors);

			if (otherErrors.length > 0) {
				// todo: show as alert before form
				this._notificationService.error(otherErrors);
			}
		}
	};

	createItem = (field: IDataField): React.ReactNode => {
		const { t, layout, data, hideLabels } = this.props;

		const initialValue = data?.[field.key];

		/* const fieldOptions = field.type == "boolean"
			? {
				initialValue: initialValue,
				valuePropName: "checked",
			}
			: {
				initialValue: initialValue,
				rules: [{
					required: field.required,
					whitespace: field.required,
					message: t("dataForm.rule.required", { name: field.name })
				}]
			}; */

		const rules = field.type == "boolean" ? null : [{
			required: field.required,
			whitespace: field.required,
			message: t("dataForm.rule.required", { name: field.name })
		}];

		const fieldFactory = DataFieldFactory.get(field.type);

		const fieldNode = fieldFactory.createNode(field, data);

		const itemLayout = (layout == null || layout == "horizontal")
			? (field.type == "boolean" ? FormDefaults.tailFormItemLayout : FormDefaults.formItemLayout)
			: {};

		return (
			<Form.Item
				key={field.key}
				name={field.key}
				label={hideLabels || field.type == "boolean" ? null : field.name}
				extra={field.description}
				valuePropName={field.type == "boolean" ? "checked" : "value"}
				rules={rules}
				{...itemLayout}>
				{fieldNode}
			</Form.Item>
		);
	};

	render = () => {
		const { t, layout, data, fields, showControls, submitButton } = this.props,
			{ loading } = this.state;

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.tailFormItemLayout : null;

		return (
			<Spin spinning={loading}>
				<Form ref={this._formRef}
					initialValues={data}
					layout={layout || "horizontal"}
					onChange={this.handleChange}
					onFinish={this.handleSubmit}>

					{fields && fields.map(x => this.createItem(x))}

					{fields && showControls !== false &&
						<Form.Item {...itemLayout}>
							<Button type="primary" htmlType="submit" icon={Icon.Check}>{submitButton || t("button.save")}</Button>
						</Form.Item>
					}
				</Form>
			</Spin>
		);
	};
}

export const DataForm = withTranslation()(WrappedDataForm);
