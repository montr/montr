import * as React from "react";
import { Form, Input, message } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { IApiResult, IPaneProps } from "@montr-core/models";
import { EventService } from "../../services";
import { IPaneComponent, FormDefaults } from "@montr-core/components";
import { IEvent } from "modules/tendr/models";

interface IEventFormProps extends FormComponentProps {
	data: IEvent;
}

interface IEventFormState {
}

class EventForm extends React.Component<IEventFormProps, IEventFormState> {

	private _eventService = new EventService();

	constructor(props: IEventFormProps) {
		super(props);

		this.state = {};
	}

	componentWillUnmount = async () => {
		await this._eventService.abort();
	}

	private handleSubmit = async (e: React.SyntheticEvent) => {
		e.preventDefault();

		await this.save();
	}

	private save = async () => {
		this.props.form.validateFieldsAndScroll((errors, values: IEvent) => {
			if (!errors) {
				this._eventService
					.update({ id: this.props.data.id, ...values })
					.then((result: IApiResult) => {
						message.success("Данные успешно сохранены");
					});
			}
			else {
				message.error("Received errors: " + JSON.stringify(errors));
			}
		});
	}

	render = () => {

		const { getFieldDecorator } = this.props.form;

		return (
			<Form onSubmit={this.handleSubmit}>
				<Form.Item {...FormDefaults.formItemLayout} label="Наименование">
					{getFieldDecorator("name", {
						rules: [
							{ required: true, whitespace: true, message: "Поле «Наименование» обязательно для заполнения" }
						],
						initialValue: this.props.data.name
					})(
						<Input.TextArea autosize={{ minRows: 4, maxRows: 24 }} />
					)}
				</Form.Item>
				<Form.Item {...FormDefaults.formItemLayout} label="Описание" extra="Как можно подробнее опишите что вы хотите купить.">
					{getFieldDecorator("description", {
						initialValue: this.props.data.description
					})(
						<Input.TextArea autosize={{ minRows: 4, maxRows: 24 }} />
					)}
				</Form.Item>
				{/*  <Form.Item {...tailFormItemLayout}>
                    <Button type="primary" htmlType="submit" icon="check">Сохранить</Button>
                </Form.Item> */}
			</Form >
		);
	}
}

const WrappedForm = Form.create<IEventFormProps>()(EventForm);

interface IEditEventPaneProps extends IPaneProps<IEvent> {
	data: IEvent;
}

interface IEditEventTabState {
}

export class EditEventPane extends React.Component<IEditEventPaneProps, IEditEventTabState> {

	private _formRef: IPaneComponent;

	save() {
		this._formRef.save();
	}

	render() {
		return (
			<WrappedForm data={this.props.data} wrappedComponentRef={(form: any) => this._formRef = form} />
		);
	}
}
