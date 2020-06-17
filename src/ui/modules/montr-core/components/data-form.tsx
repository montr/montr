import * as React from "react";
import { Form, Spin } from "antd";
import { FormInstance } from "antd/lib/form";
import { FieldData, Store } from "rc-field-form/lib/interface";
import { IDataField, IApiResult, IIndexer } from "../models";
import { NotificationService } from "../services/notification-service";
import { OperationService, DataHelper } from "../services";
import { FormDefaults, DataFieldFactory, ButtonSave, Toolbar } from ".";
import { withTranslation, WithTranslation } from "react-i18next";

declare const FormLayouts: ["horizontal", "inline", "vertical"];

export interface IDataFormOptions extends WithTranslation {
	layout?: (typeof FormLayouts)[number];
	mode?: "Edit" | "View";
	hideLabels?: boolean;
}

interface IProps extends IDataFormOptions {
	fields: IDataField[]; // todo: provide url to load fields or create wrapped component
	data: any; // IIndexer;
	showControls?: boolean;
	submitButton?: string;
	resetButton?: string;
	successMessage?: string;
	errorMessage?: string;
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
			loading: false,
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

	render = () => {
		const { t, layout, data, fields, mode, showControls, submitButton, resetButton } = this.props,
			{ loading } = this.state;

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.tailFormItemLayout : null;

		return (
			<Spin spinning={loading}>
				{fields &&
					<Form
						ref={this.getFormRef()}
						autoComplete="off"
						colon={false}
						className={mode == "View" ? "data-form-mode-view" : null}
						initialValues={data}
						scrollToFirstError={true}
						layout={layout || "horizontal"}
						onValuesChange={this.handleValuesChange}
						onFinish={this.handleSubmit}>

						{fields.map(field => {
							const factory = DataFieldFactory.get(field.type);

							if (!factory) {
								// todo: display default placeholder for not found field type
								console.error(`Field type ${field.type} is not found.`);
								return null;
							}

							return factory.createFormItem(field, data, this.props);
						})}

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
