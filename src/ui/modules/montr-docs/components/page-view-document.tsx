import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin, Tabs, PageHeader, Button, Tag, Descriptions } from "antd";
import { Page } from "@montr-core/components";
import { IDocument } from "../models";
import { DocumentService } from "../services";
import { RouteBuilder } from "../module";

interface IRouteProps {
	uid?: string;
	tabKey?: string;
}

interface IProps extends RouteComponentProps<IRouteProps> {
}

interface IState {
	loading: boolean;
	data?: IDocument;
}

export default class PageViewDocument extends React.Component<IProps, IState> {

	private _documentService = new DocumentService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			data: {}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._documentService.abort();
	};

	fetchData = async () => {
		const { uid } = this.props.match.params;

		const data = (uid)
			? await this._documentService.get(uid)
			// todo: load defaults from server
			: {};

		this.setState({ loading: false, data });
	};

	handleTabChange = (tabKey: string) => {
		const { uid } = this.props.match.params;

		const path = RouteBuilder.viewDocument(uid, tabKey);

		this.props.history.replace(path);
	};

	render = () => {
		const { uid, tabKey } = this.props.match.params,
			{ loading, data } = this.state;

		const otherTabsDisabled = !uid;

		return (
			<Spin spinning={loading}>
				<PageHeader
					onBack={() => window.history.back()}
					title={data.documentNumber}
					subTitle={data.configCode}
					tags={<Tag color="green">{data.statusCode}</Tag>}
					extra={[
						<Button key="2">Отклонить</Button>,
						<Button key="1" type="primary">Допустить</Button>,
					]}
					footer={
						<Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
							<Tabs.TabPane key="common" tab="Общая информация">
							</Tabs.TabPane>
							<Tabs.TabPane key="fields" tab="Анкета">
							</Tabs.TabPane>
						</Tabs>
					}>
					<Descriptions size="small" column={1}>
						<Descriptions.Item label="Date">{data.documentDate}</Descriptions.Item>
						<Descriptions.Item label="Name">{data.name}</Descriptions.Item>
					</Descriptions>
				</PageHeader>
			</Spin>
		);
	};
}
