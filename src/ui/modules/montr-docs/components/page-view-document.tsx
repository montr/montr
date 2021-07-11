import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin, PageHeader, Button } from "antd";
import { DataView } from "@montr-core/models";
import { ClassifierService } from "@montr-master-data/services";
import { Classifier } from "@montr-master-data/models";
import { DataBreadcrumb, DataTabs, StatusTag } from "@montr-core/components";
import { IDocument } from "../models";
import { DocumentMetadataService, DocumentService } from "../services";
import { ClassifierTypeCode, EntityTypeCode, RouteBuilder, Views } from "../module";

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

		const dataView = await this.documentMetadataService.view(documentType.uid, Views.documentTabs);

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

		return (
			<Spin spinning={loading}>
				<PageHeader
					onBack={() => window.history.back()}
					title={documentType.name}
					subTitle={document.uid}
					tags={<StatusTag statusCode={document.statusCode} />}
					breadcrumb={<DataBreadcrumb items={[{ name: "Documents" }]} />}
					extra={[
						<Button key="2">Accept or Reject</Button>,
						<Button key="1" type="primary">Publish</Button>,
					]}>
					<DocumentSignificantInfo document={document} />
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
