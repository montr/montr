import * as React from "react";
import { DataTabs, Page, PageHeader } from "@montr-core/components";
import { DataView } from "@montr-core/models";
import { RouteComponentProps } from "react-router";
import { Spin } from "antd";
import { ClassifierService, ClassifierTypeService, ClassifierLinkService } from "../services";
import { IClassifier, IClassifierType } from "../models";
import { ClassifierBreadcrumb } from ".";
import { RouteBuilder } from "../module";
import { MetadataService } from "@montr-core/services";

interface RouteProps {
	typeCode: string;
	uid?: string;
	parentUid?: string;
	tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
	loading: boolean;
	dataView?: DataView<IClassifier>;
	type?: IClassifierType;
	data?: IClassifier;
}

export default class PageEditClassifier extends React.Component<Props, State> {

	private _metadataService = new MetadataService();
	private _classifierTypeService = new ClassifierTypeService();
	private _classifierService = new ClassifierService();
	private _classifierLinkService = new ClassifierLinkService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentDidUpdate = async (prevProps: Props) => {
		if (this.props.match.params.typeCode !== prevProps.match.params.typeCode ||
			this.props.match.params.uid !== prevProps.match.params.uid) {
			await this.fetchData();
		}
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._classifierTypeService.abort();
		await this._classifierService.abort();
		await this._classifierLinkService.abort();
	};

	fetchData = async () => {
		const { typeCode, uid, parentUid } = this.props.match.params;

		const dataView = await this._metadataService.load("Classifier/Edit");

		const type = await this._classifierTypeService.get({ typeCode });

		const data = (uid)
			? await this._classifierService.get(typeCode, uid)
			: await this._classifierService.create(typeCode, parentUid);

		if (uid && type.hierarchyType == "Groups") {
			const links = await this._classifierLinkService.list({ typeCode: type.code, itemUid: uid });

			const defaultLink = links.rows.find(x => x.tree.code == "default");

			if (defaultLink) data.parentUid = defaultLink.group.uid;
		}

		this.setState({ loading: false, dataView, type, data });
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
			{ loading, dataView, type, data } = this.state;

		const otherTabsDisabled = !data?.uid;

		return (
			<Page title={<>
				<ClassifierBreadcrumb type={type} item={data} />
				<PageHeader>{data?.name}</PageHeader>
			</>}>
				<Spin spinning={loading}>

					<DataTabs
						tabKey={tabKey}
						panes={dataView?.panes}
						onTabChange={this.handleTabChange}
						tabProps={{ type, data, onDataChange: this.handleDataChange }}
					/>

					{/* <Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
						<Tabs.TabPane key="info" tab="Информация">
							<TabEditClassifier type={type} data={data} onDataChange={this.handleDataChange} />
						</Tabs.TabPane>
						<Tabs.TabPane key="hierarchy" tab="Иерархия" disabled={otherTabsDisabled}>
							<TabEditClassifierHierarchy type={type} data={data} onDataChange={this.handleDataChange} />
						</Tabs.TabPane>
						<Tabs.TabPane key="dependencies" tab="Зависимости" disabled={otherTabsDisabled}></Tabs.TabPane>
						<Tabs.TabPane key="history" tab="История изменений" disabled={otherTabsDisabled}></Tabs.TabPane>
					</Tabs> */}

				</Spin>
			</Page>
		);
	};
}
