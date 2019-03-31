import * as React from "react";
import { Page, FormDefaults, PageHeader } from "@montr-core/components";
import { RouteComponentProps, Redirect } from "react-router";
import { Form, Input, Button, Spin } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { MetadataService, NotificationService } from "@montr-core/services";
import { IFormField, Guid } from "@montr-core/models";
import { ClassifierService } from "../services";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { IClassifier, IClassifierType } from "../models";
import { ClassifierBreadcrumb } from "../components";

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
	type?: IClassifierType;
	data: IClassifier;
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

		const { currentCompany } = this.props;

		if (currentCompany) {
			await this.fetchData();
		}
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.typeCode !== prevProps.typeCode ||
			this.props.currentCompany !== prevProps.currentCompany) {

			await this.fetchData();
		}
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._classifierService.abort();
	}

	private fetchClassifierTypes = async (): Promise<IClassifierType> => {
		const { typeCode, currentCompany } = this.props;

		const data = await this._classifierService.types(currentCompany.uid);
		const types = data.rows;

		const type = types.find(x => x.code == typeCode);

		return type;
	}

	private fetchData = async () => {
		const { typeCode, currentCompany, uid } = this.props;

		if (currentCompany) {

			var type = await this.fetchClassifierTypes();

			const dataView = await this._metadataService.load(`Classifier/${typeCode}`);

			let data = {};
			if (this.props.uid) {
				//try {
				data = await this._classifierService.get(currentCompany.uid, typeCode, new Guid(uid));
				//} catch (error) {
				//	console.log(error);
				//	throw error;
				//}
			}

			this.setState({ loading: false, type, fields: dataView.fields, data });
		}
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
						...values
					};

				if (this.props.uid) {
					const uid: Guid = await this._classifierService.update(companyUid, item);

					this._notificationService.success("Данные успешно сохранены.");
				}
				else {
					const uid: Guid = await this._classifierService.insert(companyUid, this.props.typeCode, item);

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
		const { typeCode } = this.props,
			{ type, data } = this.state;

		if (this.state.newUid) {
			return <Redirect to={`/classifiers/${typeCode}/edit/${this.state.newUid}`} />
		}

		return (
			<Page title={<>
				<ClassifierBreadcrumb type={type} />
				<PageHeader>{data.name}</PageHeader>
			</>}>
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
			</Page>
		);
	}
}

const EditClassifierForm = withCompanyContext(Form.create()(_EditClassifierForm));

export class EditClassifier extends React.Component<RouteComponentProps<IRouteProps>> {
	render = () => {

		const { typeCode, uid } = this.props.match.params;

		return <EditClassifierForm typeCode={typeCode} uid={uid} />
	}
}
