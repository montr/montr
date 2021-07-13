import { DataTabs, StatusTag } from "@montr-core/components";
import { DataView } from "@montr-core/models";
import { DateHelper } from "@montr-core/services";
import { Classifier } from "@montr-master-data/models";
import { ClassifierService } from "@montr-master-data/services";
import { Button, PageHeader, Spin } from "antd";
import React from "react";
import { RouteComponentProps } from "react-router";
import { DocumentBreadcrumb } from ".";
import { IDocument } from "../models";
import { ClassifierTypeCode, EntityTypeCode, RouteBuilder, Views } from "../module";
import { DocumentMetadataService, DocumentService } from "../services";

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
	dataView?: DataView<Classifier>;
}

export default class PageViewDocument extends React.Component<Props, State> {

	private readonly documentMetadataService = new DocumentMetadataService();
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
		await this.documentMetadataService.abort();
		await this.documentService.abort();
		await this.classifierService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { uid } = this.props.match.params;

		const document = await this.documentService.get(uid);

		const documentType = await this.classifierService
			.get(ClassifierTypeCode.documentType, document.documentTypeUid);

		const dataView = await this.documentMetadataService.view(document.uid, Views.documentTabs);

		this.setState({ loading: false, document, documentType, dataView });
	};

	handleDataChange = (document: IDocument): void => {
		const { uid } = this.props.match.params;

		if (uid) {
			this.setState({ document });
		}
	};

	handleTabChange = (tabKey: string): void => {
		const { uid } = this.props.match.params;

		const path = RouteBuilder.viewDocument(uid, tabKey);

		this.props.history.replace(path);
	};

	render = (): React.ReactNode => {
		const { tabKey } = this.props.match.params,
			{ loading, document, documentType, dataView } = this.state;

		if (!document || !document.documentTypeUid) return null;

		const pageTitle = `${documentType.name} — ${document.documentNumber} — ${DateHelper.toLocaleDateString(document.documentDate)}`;

		return (
			<Spin spinning={loading}>
				<PageHeader
					onBack={() => window.history.back()}
					title={pageTitle}
					subTitle={document.uid}
					tags={<StatusTag statusCode={document.statusCode} />}
					breadcrumb={<DocumentBreadcrumb />}
					extra={[
						<Button key="2">Accept or Reject</Button>,
						<Button key="1" type="primary">Publish</Button>,
					]}>
					{/* <DocumentSignificantInfo document={document} /> */}
				</PageHeader>

				<DataTabs
					tabKey={tabKey}
					panes={dataView?.panes}
					onTabChange={this.handleTabChange}
					disabled={(_, index) => index > 0 && !document?.uid}
					tabProps={{
						document,
						documentType,
						onDataChange: this.handleDataChange,
						entityTypeCode: EntityTypeCode.document,
						entityUid: document?.uid
					}}
				/>

			</Spin>
		);
	};
}
