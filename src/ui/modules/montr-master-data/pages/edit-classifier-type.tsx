import * as React from "react";
import { RouteComponentProps } from "react-router";
import { Spin, Tabs } from "antd";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components";
import { Page, PageHeader, PaneSearchMetadata } from "@montr-core/components";
import { ClassifierTypeService } from "../services";
import { IClassifierType } from "../models";
import { ClassifierBreadcrumb, TabEditClassifierType, TabEditClassifierTypeHierarchy } from "../components";
import { RouteBuilder } from "../module";

interface IRouteProps {
	uid?: string;
	tabKey?: string;
}

interface IProps extends CompanyContextProps, RouteComponentProps<IRouteProps> {
}

interface IState {
	loading: boolean;
	types?: IClassifierType[];
	data?: IClassifierType;
}

class _EditClassifierType extends React.Component<IProps, IState> {

	private _classifierTypeService = new ClassifierTypeService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.match.params.uid !== prevProps.match.params.uid ||
			this.props.currentCompany !== prevProps.currentCompany) {
			await this.fetchData();
		}
	};

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
	};

	fetchData = async () => {
		const { uid } = this.props.match.params;

		const types = await this._classifierTypeService.list();

		const data: IClassifierType = (uid)
			? await this._classifierTypeService.get({ uid })
			// todo: load defaults with metadata
			: { name: "", hierarchyType: "None" };

		this.setState({ loading: false, data, types: types.rows });
	};

	handleDataChange = (data: IClassifierType) => {
		this.setState({ data });
	};

	handleTabChange = (tabKey: string) => {
		const { uid } = this.props.match.params;

		const path = RouteBuilder.editClassifierType(uid, tabKey);

		this.props.history.replace(path);
	};

	render = () => {
		const { uid, tabKey } = this.props.match.params,
			{ loading, data, types } = this.state;

		let title;
		// todo: remove this sh*t
		if (loading) {
			title = <>
				<ClassifierBreadcrumb /* types={types} */ />
				<PageHeader>&#xA0;</PageHeader>
			</>;
		}
		else {
			title = <>
				{(uid)
					? <ClassifierBreadcrumb /* types={types} */ type={data} item={{ name: "Настройка" }} />
					: <ClassifierBreadcrumb item={{ name: "Добавление" }} />
				}
				<PageHeader>{data.name}</PageHeader>
			</>;
		}

		const otherTabsDisabled = !data?.uid;

		return (
			<Page title={title}>
				<Spin spinning={loading}>
					{data && <Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
						<Tabs.TabPane key="info" tab="Информация">
							<TabEditClassifierType data={data} onDataChange={this.handleDataChange} />
						</Tabs.TabPane>
						<Tabs.TabPane key="hierarchy" tab="Иерархия" disabled={otherTabsDisabled}>
							<TabEditClassifierTypeHierarchy type={data} />
						</Tabs.TabPane>
						<Tabs.TabPane key="fields" tab="Поля" disabled={otherTabsDisabled}>
							{data?.code && <PaneSearchMetadata entityTypeCode={`Classifier.${data.code}`} />}
						</Tabs.TabPane>
						<Tabs.TabPane key="history" tab="История изменений" disabled={otherTabsDisabled}></Tabs.TabPane>
					</Tabs>}
				</Spin>
			</Page>
		);
	};
}

const EditClassifierType = withCompanyContext(_EditClassifierType);

export default EditClassifierType;
