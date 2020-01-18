import * as React from "react";
import { Page, PageHeader } from "@montr-core/components";
import { RouteComponentProps } from "react-router";
import { Spin, Tabs } from "antd";
import { ClassifierService, ClassifierTypeService, ClassifierLinkService } from "../services";
import { IClassifier, IClassifierType } from "../models";
import { ClassifierBreadcrumb, TabEditClassifier, TabEditClassifierHierarchy } from ".";
import { RouteBuilder } from "../module";

interface IRouteProps {
	typeCode: string;
	uid?: string;
	parentUid?: string;
	tabKey?: string;
}

interface IProps extends RouteComponentProps<IRouteProps> {
}

interface IState {
	loading: boolean;
	type?: IClassifierType;
	data?: IClassifier;
}

export default class EditClassifier extends React.Component<IProps, IState> {
	private _classifierTypeService = new ClassifierTypeService();
	private _classifierService = new ClassifierService();
	private _classifierLinkService = new ClassifierLinkService();

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
		if (this.props.match.params.typeCode !== prevProps.match.params.typeCode ||
			this.props.match.params.uid !== prevProps.match.params.uid) {
			await this.fetchData();
		}
	};

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
		await this._classifierService.abort();
		await this._classifierLinkService.abort();
	};

	fetchData = async () => {
		const { typeCode, uid, parentUid } = this.props.match.params;

		const type = await this._classifierTypeService.get({ typeCode });

		const data = (uid)
			? await this._classifierService.get(typeCode, uid)
			// todo: load defaults from server
			: { parentUid };

		if (uid && type.hierarchyType == "Groups") {
			const links = await this._classifierLinkService.list({ typeCode: type.code, itemUid: uid });

			const defaultLink = links.rows.find(x => x.tree.code == "default");

			if (defaultLink) data.parentUid = defaultLink.group.uid;
		}

		this.setState({ loading: false, type, data });
	};

	handleDataChange = (data: IClassifier) => {
		const { typeCode, uid } = this.props.match.params;

		if (uid) {
			this.setState({ data });
		}
		else {
			const path = RouteBuilder.editClassifier(typeCode, data.uid);

			this.props.history.push(path);
		}
	};

	handleTabChange = (tabKey: string) => {
		const { typeCode, uid } = this.props.match.params;

		const path = RouteBuilder.editClassifier(typeCode, uid, tabKey);

		this.props.history.replace(path);
	};

	render = () => {
		const { tabKey } = this.props.match.params,
			{ loading, type, data } = this.state;

		const otherTabsDisabled = !data?.uid;

		return (
			<Page title={<>
				<ClassifierBreadcrumb type={type} item={data} />
				<PageHeader>{data?.name}</PageHeader>
			</>}>
				<Spin spinning={loading}>
					<Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
						<Tabs.TabPane key="info" tab="Информация">
							<TabEditClassifier type={type} data={data} onDataChange={this.handleDataChange} />
						</Tabs.TabPane>
						<Tabs.TabPane key="hierarchy" tab="Иерархия" disabled={otherTabsDisabled}>
							<TabEditClassifierHierarchy type={type} data={data} onDataChange={this.handleDataChange} />
						</Tabs.TabPane>
						<Tabs.TabPane key="dependencies" tab="Зависимости" disabled={otherTabsDisabled}></Tabs.TabPane>
						<Tabs.TabPane key="history" tab="История изменений" disabled={otherTabsDisabled}></Tabs.TabPane>
					</Tabs>
				</Spin>
			</Page>
		);
	};
}
