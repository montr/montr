import * as React from "react";
import { Page, PageHeader } from "@montr-core/components";
import { RouteComponentProps } from "react-router";
import { Spin, Tabs } from "antd";
import { ClassifierService, ClassifierTypeService } from "../services";
import { CompanyContextProps, withCompanyContext } from "@kompany/components";
import { IClassifier, IClassifierType } from "../models";
import { ClassifierBreadcrumb, TabEditClassifier } from "../components";
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
	type?: IClassifierType;
	data: IClassifier;
}

class _EditClassifier extends React.Component<IProps, IState> {
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
			this.props.match.params.uid !== prevProps.match.params.uid ||
			this.props.currentCompany !== prevProps.currentCompany) {
			await this.fetchData();
		}
	}

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
		await this._classifierService.abort();
	}

	private fetchData = async () => {
		const { typeCode, uid } = this.props.match.params;
		const { currentCompany } = this.props;

		if (currentCompany) {

			const type = await this._classifierTypeService.get(currentCompany.uid, { typeCode });

			const data = (uid)
				? await this._classifierService.get(currentCompany.uid, typeCode, uid)
				: this.state.data;

			this.setState({ loading: false, type, data });
		}
	}

	private handleDataChange = (data: IClassifier) => {
		this.setState({ data });
	}

	private handleTabChange = (tabKey: string) => {
		const { typeCode, uid } = this.props.match.params;

		const path = RouteBuilder.editClassifier(typeCode, uid, tabKey);

		this.props.history.replace(path)
	}

	render = () => {
		const { tabKey } = this.props.match.params;
		const { type, data } = this.state;

		const otherTabsDisabled = !data.uid;

		return (
			<Page title={<>
				<ClassifierBreadcrumb type={type} item={data} />
				<PageHeader>{data.name}</PageHeader>
			</>}>
				<Spin spinning={this.state.loading}>
					<Tabs defaultActiveKey={tabKey} onChange={this.handleTabChange}>
						<Tabs.TabPane key="info" tab="Информация">
							<TabEditClassifier type={type} data={data} onDataChange={this.handleDataChange} />
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
