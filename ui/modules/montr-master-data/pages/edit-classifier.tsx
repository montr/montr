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
import { RouteBuilder } from "../";

interface IRouteProps {
	typeCode: string;
	uid?: string;
	tabKey?: string;
}

interface IProps extends CompanyContextProps, RouteComponentProps<IRouteProps> {
}

interface IState {
	loading: boolean;
	fields?: IFormField[];
	type?: IClassifierType;
	data: IClassifier;
	redirect?: string;
}

/* class _TabEditClassifier extends React.Component<IProps, IState> {
} */

class _EditClassifier extends React.Component<IProps, IState> {

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
		if (this.props.match.params.typeCode !== prevProps.match.params.typeCode ||
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
		const { typeCode, uid } = this.props.match.params;
		const { currentCompany } = this.props;

		if (currentCompany) {

			const type = await this._classifierTypeService.get(currentCompany.uid, typeCode);

			const dataView = await this._metadataService.load(`Classifier/${typeCode}`);

			const data = (uid)
				? await this._classifierService.get(currentCompany.uid, typeCode, uid)
				: this.state.data;

			this.setState({ loading: false, type, fields: dataView.fields, data });
		}
	}

	private save = async (values: IClassifier) => {

		const { typeCode, uid } = this.props.match.params;
		const { uid: companyUid } = this.props.currentCompany;

		if (uid) {
			const data = { uid, ...values };
			const rowsUpdated = await this._classifierService.update(companyUid, { uid, ...values });

			if (rowsUpdated != 1) throw new Error();

			this.setState({ data });
		}
		else {
			const uid: Guid = await this._classifierService.insert(companyUid, typeCode, values);

			const path = RouteBuilder.edit(typeCode, uid);

			this.setState({ redirect: path });
		}
	}

	private handleTabChange = (tabKey: string) => {
		const { typeCode, uid } = this.props.match.params;

		const path = RouteBuilder.edit(typeCode, uid, tabKey);

		this.props.history.replace(path)
	}

	render = () => {
		if (this.state.redirect) {
			return <Redirect to={this.state.redirect} />
		}

		const { tabKey } = this.props.match.params;
		const { type, fields, data } = this.state;

		const otherTabsDisabled = !data.uid;

		return (
			<Page title={<>
				<ClassifierBreadcrumb type={type} item={data} />
				<PageHeader>{data.name}</PageHeader>
			</>}>
				<Spin spinning={this.state.loading}>
					<Tabs defaultActiveKey={tabKey} onChange={this.handleTabChange}>
						<Tabs.TabPane key="info" tab="Информация">
							{fields && <DataForm fields={fields} data={data} onSave={this.save} />}
						</Tabs.TabPane>
						<Tabs.TabPane key="hierarchy" tab="Иерархия" disabled={otherTabsDisabled}></Tabs.TabPane>
						<Tabs.TabPane key="dependencies" tab="Зависимости" disabled={otherTabsDisabled}></Tabs.TabPane>
						<Tabs.TabPane key="history" tab="История изменений" disabled={otherTabsDisabled}></Tabs.TabPane>
					</Tabs>
				</Spin>
			</Page>
		);
	}
}

export const EditClassifier = withCompanyContext(_EditClassifier);
