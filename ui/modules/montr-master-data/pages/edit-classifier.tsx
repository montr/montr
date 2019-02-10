import * as React from "react";
import { Page, FormDefaults } from "@montr-core/components";
import { RouteComponentProps } from "react-router";
import { Form, Input, Button, Spin } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { MetadataService, NotificationService } from "@montr-core/services";
import { IDataColumn } from "@montr-core/models";

interface IRouteProps {
	configCode: string;
}

interface IProps extends FormComponentProps {
	configCode: string;
}

interface IState {
	loading: boolean;
	columns: IDataColumn[];
}

class _EditClassifierForm extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _notificationService = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			columns: []
		};
	}

	componentDidMount = async () => {
		const dataView = await this._metadataService.load(`Classifier/${this.props.configCode}`);

		this.setState({ loading: false, columns: dataView.columns });
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

	render = () => {
		const { getFieldDecorator } = this.props.form;

		return (
			<Spin spinning={this.state.loading}>
				<Form onSubmit={this.handleSubmit}>
					{this.state.columns.map((column) => {
						return (
							<Form.Item key={column.key} label={column.name} {...FormDefaults.formItemLayout}>
								{getFieldDecorator(column.key, {
									rules: [{
										required: true,
										whitespace: true,
										message: `Поле «${column.name}» обязательно для заполнения`
									}],
									initialValue: column.key
								})(
									<Input.TextArea autosize={{ minRows: 4, maxRows: 24 }} />
								)}
							</Form.Item>
						);
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
