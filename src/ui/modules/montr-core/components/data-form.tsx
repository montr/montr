import * as React from "react";
import { Form, Button, Spin } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { IDataField, IIndexer, IApiResult } from "../models";
import { NotificationService } from "../services/notification-service";
import { OperationService } from "../services";
import { FormDefaults, DataFieldFactory } from ".";
import { withTranslation, WithTranslation } from "react-i18next";

declare const FormLayouts: ["horizontal", "inline", "vertical"];

interface IProps extends WithTranslation, FormComponentProps {
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

		const { form } = this.props;

		return form.getFieldValue(fieldName);
	};

	handleChange = async (e: React.SyntheticEvent) => {
		const { form, onChange } = this.props;

		if (onChange) {
			var values = form.getFieldsValue();

			onChange(values);
		}
	};

	handleSubmit = async (e: React.SyntheticEvent) => {
		e.preventDefault();

		const { t, form, onSubmit: onSave, successMessage, errorMessage } = this.props;

		form.validateFieldsAndScroll(async (errors, values: any) => {
			if (!errors) {
				if (this._isMounted) this.setState({ loading: true });

				await this._operation.execute(() => onSave(values), {
					successMessage: successMessage || t("dataForm.submit.success"),
					errorMessage: errorMessage || t("dataForm.submit.error"),
					showFieldErrors: async (result) => {
						await this.setFieldErrors(result, values);
					}
				});

				if (this._isMounted) this.setState({ loading: false });
			}
		});
	};

	setFieldErrors = async (result: IApiResult, values: IIndexer) => {
		const { form, fields } = this.props,
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

			form.setFields(fieldErrors);

			if (otherErrors.length > 0) {
				// todo: show as alert before form
				this._notificationService.error(otherErrors);
			}
		}
	};

	createItem = (field: IDataField): React.ReactNode => {
		const { t, layout, data, hideLabels } = this.props;
		const { getFieldDecorator } = this.props.form;

		const initialValue = data?.[field.key];

		const fieldOptions = field.type == "boolean"
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
			};

		const fieldFactory = DataFieldFactory.get(field.type);

		const fieldNode = fieldFactory.createNode(field, data);

		const itemLayout = (layout == null || layout == "horizontal")
			? (field.type == "boolean" ? FormDefaults.tailFormItemLayout : FormDefaults.formItemLayout)
			: {};

		return (
			<Form.Item
				key={field.key}
				label={hideLabels || field.type == "boolean" ? null : field.name}
				extra={field.description}
				{...itemLayout}>
				{getFieldDecorator(field.key, fieldOptions)(fieldNode)}
			</Form.Item>
		);
	};

	render = () => {
		const { t, layout, fields, showControls, submitButton } = this.props,
			{ loading } = this.state;

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.tailFormItemLayout : null;

		return (
			<Spin spinning={loading}>
				<Form layout={layout || "horizontal"}
					onChange={this.handleChange}
					onSubmit={this.handleSubmit}
				>
					{fields && fields.map(x => this.createItem(x))}
					{fields && showControls !== false &&
						<Form.Item {...itemLayout}>
							<Button type="primary" htmlType="submit" icon="check">{submitButton || t("button.save")}</Button>&#xA0;
							{/* <Button htmlType="reset">{t("button.cancel")}</Button> */}
						</Form.Item>
					}
				</Form>
			</Spin>
		);
	};
}

export const DataForm = withTranslation()(Form.create<IProps>()(WrappedDataForm));
