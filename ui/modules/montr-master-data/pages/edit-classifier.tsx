import * as React from "react";
import { Page, FormDefaults } from "@montr-core/components";
import { RouteComponentProps, Redirect } from "react-router";
import { Form, Input, Button, Spin } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { MetadataService, NotificationService } from "@montr-core/services";
import { IFormField, Guid } from "@montr-core/models";
import { ClassifierService } from "../services";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { IClassifier } from "../models";

interface IRouteProps {
	typeCode: string;
	uid?: string;
}

interface IProps extends FormComponentProps, CompanyContextProps {
	typeCode: string;
	uid?: string;
}

interface IState {
	loading: boolean;
	fields: IFormField[];
	data: any;
	newUid?: Guid;
}

class _EditClassifierForm extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _notificationService = new NotificationService();
	private _classifierService = new ClassifierService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			fields: [],
			data: {}
		};
	}

	componentDidMount = async () => {

		const { typeCode } = this.props;

		const dataView = await this._metadataService.load(`Classifier/${typeCode}`);

		const data = await this.fetchData();

		this.setState({ loading: false, fields: dataView.fields, data });
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.typeCode !== prevProps.typeCode ||
			this.props.currentCompany !== prevProps.currentCompany) {

			const data = await this.fetchData();

			this.setState({ data });
		}
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._classifierService.abort();
	}

	private fetchData = async (): Promise<IClassifier> => {
		const { typeCode, currentCompany, uid } = this.props;

		let data = {};
		if (currentCompany && this.props.uid) {
			//try {
			data = await this._classifierService.get(currentCompany.uid, typeCode, new Guid(uid));
			//} catch (error) {
			//	console.log(error);
			//	throw error;
			//}
		}

		return data;
	}

	private handleSubmit = async (e: React.SyntheticEvent) => {
		e.preventDefault();

		await this.save();
	}

	private save = async () => {
		this.props.form.validateFieldsAndScroll(async (errors, values: any) => {
			if (!errors) {

				const { uid: companyUid } = this.props.currentCompany,
					item = {
						uid: this.props.uid,
						typeCode: this.props.typeCode,
						...values
					};

				if (this.props.uid) {
					const uid: Guid = await this._classifierService.update(companyUid, item);

					this._notificationService.success("Данные успешно сохранены.");
				}
				else {
					const uid: Guid = await this._classifierService.insert(companyUid, item);

					// todo: redirect to edit
					this._notificationService.success("Данные успешно добавлены.");

					this.setState({ newUid: uid });
				}
			}
			else {
				console.log(errors);
			}
		});
	}

	private create = (field: IFormField): React.ReactNode => {

		const { getFieldDecorator } = this.props.form;

		const initialValue = this.state.data[field.key];

		let node: React.ReactNode = null;

		if (field.type == "string") {
			node = getFieldDecorator(field.key, {
				rules: [{
					required: field.required,
					whitespace: field.required,
					message: `Поле «${field.name}» обязательно для заполнения`
				}],
				initialValue: initialValue
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
				initialValue: initialValue
			})(
				<Input.TextArea autosize={{ minRows: 4, maxRows: 24 }} />
			);
		}

		return (
			<Form.Item key={field.key} label={field.name} {...FormDefaults.formItemLayout}>
				{node}
			</Form.Item>
		);
	}

	render = () => {
		const { typeCode } = this.props;

		if (this.state.newUid) {
			return <Redirect to={`/classifiers/${typeCode}/edit/${this.state.newUid}`} />
		}

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

const EditClassifierForm = withCompanyContext(Form.create()(_EditClassifierForm));

export class EditClassifier extends React.Component<RouteComponentProps<IRouteProps>> {
	render = () => {

		const { typeCode, uid } = this.props.match.params;

		return (
			<Page title={typeCode}>
				<EditClassifierForm typeCode={typeCode} uid={uid} />
			</Page >
		)
	}
}
