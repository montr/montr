import * as React from "react";
import { Page, PageHeader, DataForm } from "@montr-core/components";
import { RouteComponentProps, Redirect } from "react-router";
import { Spin, Tabs } from "antd";
import { MetadataService } from "@montr-core/services";
import { IFormField, Guid } from "@montr-core/models";
import { ClassifierService, ClassifierTypeService } from "../services";
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
	redirect?: string;
}

class _EditClassifierForm extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _classifierTypeService = new ClassifierTypeService();
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
		await this._classifierTypeService.abort();
		await this._classifierService.abort();
	}

	private fetchData = async () => {
		const { typeCode, currentCompany, uid } = this.props;

		if (currentCompany) {

			const types = await this._classifierTypeService.list(currentCompany.uid);
			const type = types.rows.find(x => x.code == typeCode);

			const dataView = await this._metadataService.load(`Classifier/${typeCode}`);

			const data = (uid)
				? await this._classifierService.get(currentCompany.uid, typeCode, uid)
				: {};

			this.setState({ loading: false, type, types: types.rows, fields: dataView.fields, data });
		}
	}

	private save = async (values: IClassifier) => {

		const { typeCode, uid } = this.props;
		const { uid: companyUid } = this.props.currentCompany;

		if (uid) {
			const data = { uid, ...values };
			const rowsUpdated = await this._classifierService.update(companyUid, { uid, ...values });

			if (rowsUpdated != 1) throw new Error();
			this.setState({ data });
		}
		else {
			const uid: Guid = await this._classifierService.insert(companyUid, this.props.typeCode, values);

			this.setState({ redirect: `/classifiers/${typeCode}/edit/${uid}` });
		}
	}

	render = () => {
		if (this.state.redirect) {
			return <Redirect to={this.state.redirect} />
		}

		const { type, types, fields, data } = this.state;

		const otherTabsDisabled = !data.uid;

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
						<Tabs.TabPane key="2" tab="Иерархия" disabled={otherTabsDisabled}></Tabs.TabPane>
						<Tabs.TabPane key="3" tab="Ссылки" disabled={otherTabsDisabled}>
							usages of classifier
						</Tabs.TabPane>
						<Tabs.TabPane key="4" tab="История изменений" disabled={otherTabsDisabled}></Tabs.TabPane>
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
