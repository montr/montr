import * as React from "react";
import { Form, Button, Spin } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { IFormField, IIndexer, IApiResult } from "../models";
import { NotificationService } from "../services/notification-service";
import { OperationService } from "../services";
import { FormDefaults, FormFieldFactory } from ".";
import { withTranslation, WithTranslation } from "react-i18next";

declare const FormLayouts: ["horizontal", "inline", "vertical"];

interface IProps extends WithTranslation, FormComponentProps {
	layout?: (typeof FormLayouts)[number];
	fields: IFormField[];
	data: IIndexer;
	showControls?: boolean;
	submitButton?: string;
	resetButton?: string;
	successMessage?: string;
	errorMessage?: string;
	hideLabels?: boolean;
	onSubmit: (values: IIndexer) => Promise<IApiResult>;
}

interface IState {
	loading: boolean;
}

export class WrappedDataForm extends React.Component<IProps, IState> {

	private _operation = new OperationService();
	private _notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: false
		};
	}

	getFieldValue = async (fieldName: string) => {

		const { form } = this.props;

		return form.getFieldValue(fieldName);
	};

	handleSubmit = async (e: React.SyntheticEvent) => {
		e.preventDefault();

		const { t, form, onSubmit: onSave, successMessage, errorMessage } = this.props;

		form.validateFieldsAndScroll(async (errors, values: any) => {
			if (!errors) {
				this.setState({ loading: true });

				await this._operation.execute(() => onSave(values), {
					successMessage: successMessage || t("dataForm.submit.success"),
					errorMessage: errorMessage || t("dataForm.submit.error"),
					showFieldErrors: async (result) => {
						await this.setFieldErrors(result, values);
					}
				});

				this.setState({ loading: false });
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

	createItem = (field: IFormField): React.ReactNode => {
		const { layout, data, hideLabels } = this.props;
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
					// todo: translate
					message: `Поле «${field.name}» обязательно для заполнения`
				}]
			};

		const fieldFactory = FormFieldFactory.get(field.type);

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
		const { layout, fields, showControls, t, submitButton } = this.props,
			{ loading } = this.state;

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.tailFormItemLayout : null;

		return (
			<Spin spinning={loading}>
				<Form layout={layout || "horizontal"} onSubmit={this.handleSubmit}>
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
