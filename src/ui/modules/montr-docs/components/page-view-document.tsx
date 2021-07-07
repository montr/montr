import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin, Tabs, PageHeader, Button, Descriptions } from "antd";
import { IDocument } from "../models";
import { DocumentService } from "../services";
import { ClassifierTypeCode, RouteBuilder } from "../module";
import { TabViewDocumentFields } from "./tab-view-document-fields";
import { DateHelper } from "@montr-core/services";
import { ClassifierService } from "@montr-master-data/services";
import { Classifier } from "@montr-master-data/models";
import { StatusTag } from "@montr-core/components";

interface RouteProps {
	uid?: string;
	tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
	loading: boolean;
	document?: IDocument;
	documentType?: Classifier;
}

export default class PageViewDocument extends React.Component<Props, State> {

	private readonly documentService = new DocumentService();
	private readonly classifierService = new ClassifierService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true,
			document: {},
			documentType: {}
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.documentService.abort();
		await this.classifierService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { uid } = this.props.match.params;

		const document = await this.documentService.get(uid);

		const documentType = await this.classifierService
			.get(ClassifierTypeCode.documentType, document.documentTypeUid);

		this.setState({ loading: false, document, documentType });
	};

	handleTabChange = (tabKey: string): void => {
		const { uid } = this.props.match.params;

		const path = RouteBuilder.viewDocument(uid, tabKey);

		this.props.history.replace(path);
	};

	render = (): React.ReactNode => {
		const { tabKey } = this.props.match.params,
			{ loading, document, documentType } = this.state;

		if (!document || !document.documentTypeUid) return null;

		const documentDate = DateHelper.toLocaleDateTimeString(document.documentDate);

		return (
			<Spin spinning={loading}>
				<PageHeader
					onBack={() => window.history.back()}
					title={`${document.documentNumber || ''} — ${documentDate}`}
					subTitle={documentType.name}
					tags={<StatusTag statusCode={document.statusCode} />}
					extra={[
						<Button key="1" type="primary">Publish</Button>,
						<Button key="2">Accept</Button>,
						<Button key="3">Reject</Button>
					]}>
					<Descriptions size="small" column={1}>
						<Descriptions.Item label="Uid">{document.uid}</Descriptions.Item>
						<Descriptions.Item label="Name">{document.name}</Descriptions.Item>
						<Descriptions.Item label="Number">{document.documentNumber}</Descriptions.Item>
						<Descriptions.Item label="Date">{documentDate}</Descriptions.Item>
					</Descriptions>
				</PageHeader>

				<Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
					<Tabs.TabPane key="common" tab="Информация">
					</Tabs.TabPane>
					<Tabs.TabPane key="fields" tab="Анкета">
						<TabViewDocumentFields data={document} />
					</Tabs.TabPane>
				</Tabs>
			</Spin>
		);
	};
}
