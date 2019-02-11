import * as React from "react";
import { Page, FormDefaults } from "@montr-core/components";
import { RouteComponentProps } from "react-router";
import { Form, Input, Button, Spin } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { MetadataService, NotificationService } from "@montr-core/services";
import { IFormField } from "@montr-core/models";

interface IRouteProps {
	configCode: string;
}

interface IProps extends FormComponentProps {
	configCode: string;
}

interface IState {
	loading: boolean;
	fields: IFormField[];
}

class _EditClassifierForm extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			fields: []
		};
	}

	componentDidMount = async () => {
		const dataView = await this._metadataService.load(`Classifier/${this.props.configCode}`);

		this.setState({ loading: false, fields: dataView.fields });
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
	}

	private handleSubmit = async (e: React.SyntheticEvent) => {
		e.preventDefault();
		await this.save();
	}

	private save = async () => {
		this.props.form.validateFieldsAndScroll((errors, values: any) => {
			if (!errors) {
				/* this._eventService
					.update({ id: this.props.data.id, ...values })
					.then((result: IApiResult) => {
						message.success("Данные успешно сохранены");
					}); */
			}
			else {
				console.log(errors);
			}
		});
	}

	private create = (field: IFormField): React.ReactNode => {

		const { getFieldDecorator } = this.props.form;

		let node: React.ReactNode = null;

		if (field.type == "string") {
			node = getFieldDecorator(field.key, {
				rules: [{
					required: field.required,
					whitespace: field.required,
					message: `Поле «${field.name}» обязательно для заполнения`
				}],
				initialValue: field.key
			})(
				<Input />
			);
		}

		if (field.type == "textarea") {
			node = getFieldDecorator(field.key, {
				rules: [{
					required: field.required,
					whitespace: field.required,
					message: `Поле «${field.name}» обязательно для заполнения`
				}],
				initialValue: field.key
			})(
				<Input.TextArea autosize={{ minRows: 4, maxRows: 24 }} />
			);
		}

		return (
			<Form.Item key={field.key} label={field.name} {...FormDefaults.formItemLayout}>
				{node}
			</Form.Item>
		);;
	}

	render = () => {
		return (
			<Spin spinning={this.state.loading}>
				<Form onSubmit={this.handleSubmit}>
					{this.state.fields.map((field) => {
						return this.create(field);
					})}
					<Form.Item {...FormDefaults.tailFormItemLayout}>
						<Button type="primary" htmlType="submit" icon="check">Сохранить</Button>
					</Form.Item>
				</Form>
			</Spin>
		);
	}
}

const EditClassifierForm = Form.create()(_EditClassifierForm);

export class EditClassifier extends React.Component<RouteComponentProps<IRouteProps>> {
	render = () => {
		return (
			<Page title={this.props.match.params.configCode}>
				<EditClassifierForm configCode={this.props.match.params.configCode} />
			</Page >
		)
	}
}
