import * as React from "react";
import { Form, Button } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { IFormField, IIndexer } from "../models";
import { NotificationService } from "../services/notification-service";
import { FormDefaults, FormFieldFactory } from ".";

declare const FormLayouts: ["horizontal", "inline", "vertical"];

interface IProps extends FormComponentProps {
	layout?: (typeof FormLayouts)[number];
	fields: IFormField[];
	data: IIndexer;
	showControls?: boolean;
	onSave: (values: IIndexer) => void
}

interface IState {
}

export class WrappedDataForm extends React.Component<IProps, IState> {
	private _notificationService = new NotificationService();

	public handleSubmit = async (e: React.SyntheticEvent) => {
		e.preventDefault();

		this.props.form.validateFieldsAndScroll(async (errors, values: any) => {
			if (errors) {
				// console.log(errors);
			}
			else {
				try {
					await this.props.onSave(values);
					this._notificationService.success("Данные успешно сохранены");
				} catch (error) {
					this._notificationService.error("Ошибка при сохранении данных", error.message);
				}
			}
		});
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

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.formItemLayout : null;

		return (
			<Form.Item key={field.key} label={field.name} extra={field.description} {...itemLayout}>
				{getFieldDecorator(field.key, fieldOptions)(fieldNode)}
			</Form.Item>
		);
	}

	render = () => {
		const { layout, fields, showControls } = this.props;

		const itemLayout = (layout == null || layout == "horizontal") ? FormDefaults.tailFormItemLayout : null;

		return (
			<Form layout={layout || "horizontal"} onSubmit={this.handleSubmit}>
				{fields && fields.map((field) => {
					return this.createItem(field);
				})}
				{fields && showControls !== false &&
					<Form.Item {...itemLayout}>
						<Button type="primary" htmlType="submit" icon="check">Сохранить</Button>&#xA0;
					{/* <Button htmlType="reset">Отменить</Button> */}
					</Form.Item>
				}
			</Form>
		);
	}
}

export const DataForm = Form.create()(WrappedDataForm);
