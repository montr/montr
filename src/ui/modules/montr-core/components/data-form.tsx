import { Form, Spin } from "antd";
import { FormInstance } from "antd/es/form";
import { FieldData, Store } from "rc-field-form/lib/interface";
import * as React from "react";
import { withTranslation, WithTranslation } from "react-i18next";
import { ButtonSave, FormDefaults, Toolbar } from ".";
import { ApiResult, IDataField, IIndexer } from "../models";
import { DataHelper } from "../services/data-helper";
import { NotificationService } from "../services/notification-service";
import { OperationService } from "../services/operation-service";
import { DataFieldFactory } from "./data-field-factory";

export interface DataFormOptions extends WithTranslation {
	mode?: "edit" | "view";
	layout?: "horizontal" | "inline" | "vertical";
	itemLayout?: "default" | "narrow";
	hideLabels?: boolean;
	namePathPrefix?: (string | number)[];
}

interface Props extends DataFormOptions {
	fields?: IDataField[]; // todo: provide url to load fields or create wrapped component
	data?: unknown; // IIndexer;
	hideButtons?: boolean;
	submitButton?: string;
	resetButton?: string;
	successMessage?: string;
	errorMessage?: string;
	onFieldsChange?: (changedFields: FieldData[], allFields: FieldData[]) => void;
	onValuesChange?: (values: IIndexer, changedValues: IIndexer) => void;
	onSubmit?: (values: unknown /* IIndexer */) => Promise<ApiResult>;
	formRef?: React.RefObject<FormInstance>;
}

interface State {
	loading: boolean;
}

class WrappedDataForm extends React.Component<Props, State> {

	state = {
		loading: false,
	};

	private readonly operationService = new OperationService();
	private readonly notificationService = new NotificationService();

	isMounted = true;
	formRef = React.createRef<FormInstance>();

	componentWillUnmount = () => {
		this.isMounted = false;
	};

	getFormRef = (): React.RefObject<FormInstance> => {
		return (this.props.formRef ?? this.formRef);
	};

	handleFieldsChange = async (changedFields: FieldData[], allFields: FieldData[]): Promise<void> => {
		const { onFieldsChange } = this.props;

		if (onFieldsChange) {
			onFieldsChange(changedFields, allFields);
		}
	};

	handleValuesChange = async (changedValues: Store, values: Store): Promise<void> => {
		const { onValuesChange } = this.props;

		if (onValuesChange) {
			onValuesChange(changedValues, values);
		}
	};

	handleSubmit = async (values: unknown /* IIndexer */): Promise<void> => {
		const { t, onSubmit, successMessage, errorMessage } = this.props;

		if (onSubmit) {
			if (this.isMounted) this.setState({ loading: true });

			await this.operationService.execute(async () => {
				return await onSubmit(values);
			}, {
				successMessage: successMessage || t("dataForm.submit.success"),
				errorMessage: errorMessage || t("dataForm.submit.error"),
				showFieldErrors: async (result: ApiResult) => {
					await this.setFieldErrors(result, values);
				}
			});

			if (this.isMounted) this.setState({ loading: false });
		}
	};

	setFieldErrors = async (result: ApiResult, values: unknown /* IIndexer */): Promise<void> => {
		const { t, fields, errorMessage } = this.props;

		const form = this.getFormRef().current;

		if (form && result && (result.message || result.errors)) {

			const fieldErrors: FieldData[] = [], otherErrors: string[] = [];

			if (fields && result.errors) {
				result.errors.forEach(error => {
					// todo: check key exists in state.fields (ignore case + add tests)
					const field = fields.find(x => x.key && error.key && x.key.toLowerCase() == error.key.toLowerCase());

					if (field && field.key) {
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
			}

			form.setFields(fieldErrors);

			if (result.message || otherErrors.length > 0) {
				// todo: show as alert before form
				const message = result.message || errorMessage || t("dataForm.submit.error"),
					description = <ul>{otherErrors.map(x => <li>{x}</li>)}</ul>;
				this.notificationService.error(message, description);
			}
		}
	};

	render = (): React.ReactNode => {
		const { mode = "edit", layout = "horizontal", itemLayout = "default", hideButtons = false,
			data, fields, submitButton, resetButton, t } = this.props,
			{ loading } = this.state;

		const itemLayoutProps = layout == "horizontal" ? FormDefaults.getItemLayoutProps(itemLayout) : null;

		// submit button should be rendered as hidden to allow submit form by pressing enter in modals and side panes
		const buttonsDisplay = (hideButtons || mode == "view") ? "none" : "block";

		return (
			<Spin spinning={loading}>
				{fields && <Form
					ref={this.getFormRef()}
					autoComplete="off"
					colon={false}
					className={`data-form-mode-${mode}`}
					initialValues={data}
					scrollToFirstError
					labelWrap
					layout={layout}
					onFieldsChange={this.handleFieldsChange}
					onValuesChange={this.handleValuesChange}
					onFinish={this.handleSubmit}>

					{fields.map(field => {
						const factory = DataFieldFactory.get(field.type);

						return factory?.createFormItem(field, data, this.props);
					})}

					<Form.Item {...itemLayoutProps?.tail} style={{ display: buttonsDisplay, clear: "both" }}>
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
