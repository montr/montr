import * as React from "react";
import { Form, Button, Spin } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { IFormField, IIndexer, IApiResult, IApiResultError } from "../models";
import { NotificationService } from "../services/notification-service";
import { FormDefaults, FormFieldFactory } from ".";

declare const FormLayouts: ["horizontal", "inline", "vertical"];

interface IProps extends FormComponentProps {
	layout?: (typeof FormLayouts)[number];
	fields: IFormField[];
	data: IIndexer;
	showControls?: boolean;
	onSave: (values: IIndexer) => Promise<IApiResult>
}

interface IState {
	loading: boolean;
}

export class WrappedDataForm extends React.Component<IProps, IState> {
	private _notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: false
		};
	}

	handleSubmit = async (e: React.SyntheticEvent) => {
		e.preventDefault();

		const { form, onSave } = this.props;

		form.validateFieldsAndScroll(async (errors, values: any) => {
			if (errors) {
				// console.log(errors);
			}
			else {
				try {
					this.setState({ loading: true });

					var result = await onSave(values);

					if (result && result.success) {
						this._notificationService.success("Данные успешно сохранены");
					}

					await this.setFieldErrors(result, values);

				} catch (error) {
					this._notificationService.error("Ошибка при сохранении данных", error.message);
				}
				finally {
					this.setState({ loading: false });
				}
			}
		});
	}

	setFieldErrors = async (result: IApiResult, values: any) => {
		const { form, fields } = this.props,
			fieldErrors: any = {},
			otherErrors: string[] = [];

		if (result && result.errors) {
			result.errors.forEach(error => {
				// todo: check key exists in state.fields (ignore case + add tests)
				if (fields.find(x => x.key == error.key)) {
					fieldErrors[error.key] = {
						value: values.code,
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
				this._notificationService.error(otherErrors);
			}
		}
	}

	createItem = (field: IFormField): React.ReactNode => {
		const { layout, data } = this.props;
		const { getFieldDecorator } = this.props.form;

		const fieldOptions = {
			rules: [{
				required: field.required,
				whitespace: field.required,
				message: `Поле «${field.name}» обязательно для заполнения`
			}],
			initialValue: data[field.key]
		};

		const fieldFactory = FormFieldFactory.get(field.type);

		const fieldNode = fieldFactory.createNode(field, data);

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.formItemLayout : {};

		return (
			<Form.Item key={field.key} label={field.name} extra={field.description} {...itemLayout}>
				{getFieldDecorator(field.key, fieldOptions)(fieldNode)}
			</Form.Item>
		);
	}

	render = () => {
		const { layout, fields, showControls } = this.props,
			{ loading } = this.state;

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.tailFormItemLayout : null;

		return (
			<Spin spinning={loading}>
				<Form layout={layout || "horizontal"} onSubmit={this.handleSubmit}>
					{fields && fields.map(x => this.createItem(x))}
					{fields && showControls !== false &&
						<Form.Item {...itemLayout}>
							<Button type="primary" htmlType="submit" icon="check">Сохранить</Button>&#xA0;
					{/* <Button htmlType="reset">Отменить</Button> */}
						</Form.Item>
					}
				</Form>
			</Spin>
		);
	}
}

export const DataForm = Form.create<IProps>()(WrappedDataForm);
