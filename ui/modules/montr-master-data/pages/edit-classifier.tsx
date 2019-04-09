import * as React from "react";
import { Page, PageHeader, DataForm } from "@montr-core/components";
import { RouteComponentProps, Redirect } from "react-router";
import { Spin, Tabs } from "antd";
import { MetadataService, NotificationService } from "@montr-core/services";
import { IFormField, Guid } from "@montr-core/models";
import { ClassifierService } from "../services";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { IClassifier, IClassifierType } from "../models";
import { ClassifierBreadcrumb } from "../components";

interface IProps extends CompanyContextProps {
	typeCode: string;
	uid?: Guid;
}

interface IState {
	loading: boolean;
	fields?: IFormField[];
	type?: IClassifierType;
	types?: IClassifierType[];
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
			data: {}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
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

	private fetchData = async () => {
		const { typeCode, currentCompany, uid } = this.props;

		if (currentCompany) {

			const types = await this._classifierService.types(currentCompany.uid);
			const type = types.rows.find(x => x.code == typeCode);

			const dataView = await this._metadataService.load(`Classifier/${typeCode}`);

			const data = (uid)
				? await this._classifierService.get(currentCompany.uid, typeCode, uid)
				: {};

			this.setState({ loading: false, type, types: types.rows, fields: dataView.fields, data });
		}
	}

	private save = async (values: IClassifier) => {

		const { uid } = this.props;
		const { uid: companyUid } = this.props.currentCompany;

		if (uid) {
			await this._classifierService.update(companyUid, { uid, ...values });

			this._notificationService.success("Данные успешно сохранены.");
		}
		else {
			const uid: Guid = await this._classifierService.insert(companyUid, this.props.typeCode, values);

			this._notificationService.success("Данные успешно добавлены.");

			this.setState({ newUid: uid });
		}
	}

	render = () => {
		const { typeCode } = this.props,
			{ type, types, fields, data } = this.state;

		if (this.state.newUid) {
			return <Redirect to={`/classifiers/${typeCode}/edit/${this.state.newUid}`} />
		}

		return (
			<Page title={<>
				<ClassifierBreadcrumb type={type} types={types} item={data} />
				<PageHeader>{data.name}</PageHeader>
			</>}>
				<Spin spinning={this.state.loading}>
					<Tabs>
						<Tabs.TabPane key="1" tab="Информация">
							{fields && <DataForm fields={fields} data={data} onSave={this.save} />}
						</Tabs.TabPane>
						<Tabs.TabPane key="2" tab="Иерархия"></Tabs.TabPane>
						<Tabs.TabPane key="3" tab="Ссылки"></Tabs.TabPane>
						<Tabs.TabPane key="4" tab="История изменений"></Tabs.TabPane>
					</Tabs>
				</Spin>
			</Page>
		);
	}
}

const EditClassifierForm = withCompanyContext(_EditClassifierForm);

interface IRouteProps {
	typeCode: string;
	uid?: string;
}

export class EditClassifier extends React.Component<RouteComponentProps<IRouteProps>> {
	render = () => {

		const { typeCode, uid } = this.props.match.params;

		return <EditClassifierForm typeCode={typeCode} uid={uid ? new Guid(uid) : null} />
	}
}
