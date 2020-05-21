import * as React from "react";
import { Form, Spin } from "antd";
import { FormInstance } from "antd/lib/form";
import { getFieldId } from "antd/lib/form/util";
import { FieldData, Store, Rule } from "rc-field-form/lib/interface";
import { IDataField, IApiResult, IIndexer } from "../models";
import { NotificationService } from "../services/notification-service";
import { OperationService, DataHelper } from "../services";
import { FormDefaults, DataFieldFactory, ButtonSave, Toolbar } from ".";
import { withTranslation, WithTranslation } from "react-i18next";

declare const FormLayouts: ["horizontal", "inline", "vertical"];

interface IProps extends WithTranslation {
	layout?: (typeof FormLayouts)[number];
	fields: IDataField[]; // todo: provide url to load fields or create wrapped component
	data: any; // IIndexer;
	showControls?: boolean;
	submitButton?: string;
	resetButton?: string;
	successMessage?: string;
	errorMessage?: string;
	mode?: "Edit" | "View";
	hideLabels?: boolean;
	onChange?: (values: IIndexer, changedValues: IIndexer) => void;
	onSubmit?: (values: any /* IIndexer */) => Promise<IApiResult>;
	formRef?: React.RefObject<FormInstance>;
}

interface IState {
	loading: boolean;
}

class WrappedDataForm extends React.Component<IProps, IState> {

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

		if (onChange) {
			onChange(values, changedValues);
		}
	};

	handleSubmit = async (values: any /* IIndexer */) => {

		const { t, onSubmit, successMessage, errorMessage } = this.props;

		if (this._isMounted) this.setState({ loading: true });

		await this._operation.execute(async () => {
			return await onSubmit(values);
		}, {
			successMessage: successMessage || t("dataForm.submit.success"),
			errorMessage: errorMessage || t("dataForm.submit.error"),
			showFieldErrors: async (result: IApiResult) => {
				await this.setFieldErrors(result, values);
			}
		});

		if (this._isMounted) this.setState({ loading: false });
	};

	setFieldErrors = async (result: IApiResult, values: any /* IIndexer */) => {
		const { fields } = this.props,
			fieldErrors: FieldData[] = [],
			otherErrors: string[] = [];

		if (result && result.errors) {
			result.errors.forEach(error => {
				// todo: check key exists in state.fields (ignore case + add tests)
				const field = fields.find(x => x.key && error.key && x.key.toLowerCase() == error.key.toLowerCase());
				if (field) {
					fieldErrors.push({
						name: field.key.split("."),
						value: DataHelper.indexer(values, field.key, undefined),
						errors: error.messages
					});
				}
				else {
					// todo: display one message with list of errors
					error.messages.forEach(message => {
						// todo: remove key (?)
						otherErrors.push(`${message} (${error.key})`);
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
		const { t, layout, data, mode, hideLabels } = this.props;

		const fieldFactory = DataFieldFactory.get(field.type);

		// todo: display default placeholder for not found field type
		if (!fieldFactory) {
			console.error(`Field type ${field.type} is not found.`);
			return null;
		}

		const required: Rule = {
			required: field.required,
			message: t("dataForm.rule.required", { name: field.name })
		};

		if (field.type == "text" || field.type == "textarea" || field.type == "password") {
			required.whitespace = field.required;
		}

		if (fieldFactory.shouldFormatValue) {
			const value = DataHelper.indexer(data, field.key, undefined);
			const formattedValue = fieldFactory.formatValue(field, data, value);
			DataHelper.indexer(data, field.key, formattedValue);
		}

		const fieldNode = (mode == "View")
			? fieldFactory.createViewNode(field, data)
			: fieldFactory.createEditNode(field, data);

		const itemLayout = (layout == null || layout == "horizontal")
			? (field.type == "boolean" ? FormDefaults.tailFormItemLayout : FormDefaults.formItemLayout)
			: {};


		if (mode == "View") {
			return (
				<Form.Item
					key={field.key}
					label={hideLabels ? null : field.name}
					extra={field.description}
					valuePropName={fieldFactory.valuePropName}
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
				valuePropName={fieldFactory.valuePropName}
				rules={[required]}
				{...itemLayout}>
				{fieldNode}
			</Form.Item>
		);
	};

	render = () => {
		const { t, layout, data, fields, mode, showControls, submitButton, resetButton } = this.props,
			{ loading } = this.state;

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.tailFormItemLayout : null;

		return (
			<Spin spinning={loading}>
				{fields && <Form
					ref={this.getFormRef()}
					initialValues={data}
					layout={layout || "horizontal"}
					onValuesChange={this.handleValuesChange}
					onFinish={this.handleSubmit}>

					{fields.map(x => this.createItem(x))}

					<Form.Item {...itemLayout} style={{ display: mode == "View" || showControls === false ? "none" : "block" }}>
						<Toolbar>
							<ButtonSave htmlType="submit">{submitButton}</ButtonSave>
						</Toolbar>
					</Form.Item>
				</Form>}
			</Spin>
		);
	};
}

export const DataForm = withTranslation()(WrappedDataForm);
