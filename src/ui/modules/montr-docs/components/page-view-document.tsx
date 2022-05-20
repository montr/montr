import { DataTabs, DataToolbar, StatusTag } from "@montr-core/components";
import { ConfigurationItemProps, DataPaneProps, DataView } from "@montr-core/models";
import { DateHelper } from "@montr-core/services";
import { Classifier } from "@montr-master-data/models";
import { ClassifierService } from "@montr-master-data/services";
import { PageHeader, Spin } from "antd";
import React from "react";
import { useNavigate, useParams } from "react-router-dom";
import { DocumentBreadcrumb } from ".";
import { IDocument } from "../models";
import { ClassifierTypeCode, EntityTypeCode, RouteBuilder, Views } from "../module";
import { DocumentService } from "../services";

interface RouteProps {
	uid?: string;
	tabKey?: string;
}

interface State {
	loading: boolean;
	document?: IDocument;
	documentType?: Classifier;
	dataView?: DataView<Classifier>;
}

export default class PageViewDocument extends React.Component<unknown, State> {

	private readonly documentService = new DocumentService();
	private readonly classifierService = new ClassifierService();

	constructor(props: unknown) {
		super(props);

		this.state = {
			loading: true,
			document: {},
			documentType: {}
		};
	}

	getRouteProps = (): RouteProps => {
		return useParams();
	};

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.documentService.abort();
		await this.classifierService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { uid } = this.getRouteProps();

		const document = await this.documentService.get(uid);

		const documentType = await this.classifierService
			.get(ClassifierTypeCode.documentType, document.documentTypeUid);

		const dataView = await this.documentService.metadata(Views.documentPage, document.uid);

		this.setState({ loading: false, document, documentType, dataView });
	};

	handleDataChange = async (): Promise<void> => {
		await this.fetchData();
	};

	handleTabChange = (tabKey: string): void => {
		const { uid } = this.getRouteProps();

		const navigate = useNavigate();

		const path = RouteBuilder.viewDocument(uid, tabKey);

		navigate(path);
	};

	render = (): React.ReactNode => {
		const { tabKey } = this.getRouteProps(),
			{ loading, document, documentType, dataView } = this.state;

		if (!document || !document.documentTypeUid) return null;

		const pageTitle = `${documentType.name} — ${document.documentNumber} — ${DateHelper.toLocaleDateString(document.documentDate)}`;

		const buttonProps: ConfigurationItemProps = {
			// document,
			// documentType,
			onDataChange: this.handleDataChange,
			// entityTypeCode: EntityTypeCode.document,
			// entityUid: document.uid
		};

		const paneProps: DataPaneProps<IDocument> = {
			document,
			documentType,
			entityTypeCode: EntityTypeCode.document,
			entityUid: document.uid
		};

		return (
			<Spin spinning={loading}>
				<PageHeader
					onBack={() => window.history.back()}
					title={pageTitle}
					subTitle={<>document.uid</>}
					tags={<StatusTag statusCode={document.statusCode} />}
					breadcrumb={<DocumentBreadcrumb />}
					extra={<DataToolbar buttons={dataView.toolbar} buttonProps={buttonProps} />}>
					{/* <DocumentSignificantInfo document={document} /> */}
				</PageHeader>

				<DataTabs
					tabKey={tabKey}
					panes={dataView?.panes}
					onTabChange={this.handleTabChange}
					disabled={(_, index) => index > 0 && !document?.uid}
					paneProps={paneProps}
				/>

			</Spin>
		);
	};
}
