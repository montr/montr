import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin, Tabs, PageHeader, Button, Tag, Descriptions } from "antd";
import { IDocument } from "../models";
import { DocumentService } from "../services";
import { RouteBuilder } from "../module";
import { TabViewDocumentFields } from "./tab-view-document-fields";
import { DateHelper } from "@montr-core/services";

interface RouteProps {
	uid?: string;
	tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
	loading: boolean;
	data?: IDocument;
}

export default class PageViewDocument extends React.Component<Props, State> {

	private _documentService = new DocumentService();

	constructor(props: Props) {
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

		if (!data || !data.documentTypeUid) return null;

		const documentDate = DateHelper.toLocaleDateTimeString(data.documentDate);

		return (
			/* todo: load subtitle */
			<Spin spinning={loading}>
				<PageHeader
					onBack={() => window.history.back()}
					title={`${data.documentNumber} от ${documentDate}`}
					subTitle="Заявка на регистрацию"
					tags={<Tag color="green">{data.statusCode}</Tag>}
					extra={[
						<Button key="1" type="primary">Допустить</Button>,
						<Button key="2">Отклонить</Button>
					]}>
					<Descriptions size="small" column={1}>
						<Descriptions.Item label="Name">{data.name}</Descriptions.Item>
						<Descriptions.Item label="Number">{data.documentNumber}</Descriptions.Item>
						<Descriptions.Item label="Date">{DateHelper.toLocaleDateTimeString(data.documentDate)}</Descriptions.Item>
					</Descriptions>
				</PageHeader>

				<Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
					<Tabs.TabPane key="common" tab="Информация">
					</Tabs.TabPane>
					<Tabs.TabPane key="fields" tab="Анкета">
						<TabViewDocumentFields data={data} />
					</Tabs.TabPane>
				</Tabs>
			</Spin>
		);
	};
}
