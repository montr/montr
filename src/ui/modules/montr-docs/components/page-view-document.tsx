import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin, Tabs, PageHeader, Button } from "antd";
import { IDocument } from "../models";
import { DocumentService } from "../services";
import { ClassifierTypeCode, RouteBuilder } from "../module";
import { TabViewDocumentFields } from "./tab-view-document-fields";
import { DateHelper } from "@montr-core/services";
import { ClassifierService } from "@montr-master-data/services";
import { Classifier } from "@montr-master-data/models";
import { DataBreadcrumb, StatusTag } from "@montr-core/components";
import { DocumentSignificantInfo } from ".";

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
					breadcrumb={<DataBreadcrumb items={[{ name: "Documents" }]} />}
					extra={[
						<Button key="2">Accept or Reject</Button>,
						<Button key="1" type="primary">Publish</Button>,
					]}>
					<DocumentSignificantInfo document={document} />
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
