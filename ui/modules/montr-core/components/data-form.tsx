import * as React from "react";
import { Form, Button } from "antd";
import { IFormField, IIndexer } from "../models";
import { FormDefaults, FormFieldFactory } from ".";
import { FormComponentProps } from "antd/lib/form";

interface IProps extends FormComponentProps {
	fields: IFormField[];
	data: IIndexer;
	onSave: (values: IIndexer) => void
}

interface IState {
}

class _DataForm extends React.Component<IProps, IState> {

	private handleSubmit = async (e: React.SyntheticEvent) => {
		e.preventDefault();

		this.props.form.validateFieldsAndScroll(async (errors, values: any) => {
			if (errors) {
				// console.log(errors);
			}
			else {
				this.props.onSave(values);
			}
		});
	}

	private createItem = (field: IFormField): React.ReactNode => {
		const { data } = this.props;
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

		return (
			<Form.Item key={field.key} label={field.name} extra={field.description}
				{...FormDefaults.formItemLayout}>
				{getFieldDecorator(field.key, fieldOptions)(fieldNode)}
			</Form.Item>
		);
	}

	render = () => {
		const { fields } = this.props;

		return (
			<Form onSubmit={this.handleSubmit}>
				{fields.map((field) => {
					return this.createItem(field);
				})}
				<Form.Item {...FormDefaults.tailFormItemLayout}>
					<Button type="primary" htmlType="submit" icon="check">Сохранить</Button>&#xA0;
					{/* <Button htmlType="reset">Отменить</Button> */}
				</Form.Item>
			</Form>
		);
	}
}

export const DataForm = Form.create()(_DataForm);
