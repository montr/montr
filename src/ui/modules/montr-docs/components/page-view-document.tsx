import { PageHeader } from "@ant-design/pro-components";
import { StatusTag } from "@montr-core/components";
import { DataTabs } from "@montr-core/components/data-tabs";
import { DataToolbar } from "@montr-core/components/data-toolbar";
import { withNavigate, withParams } from "@montr-core/components/react-router-wrappers";
import { ConfigurationItemProps, DataPaneProps, DataView } from "@montr-core/models";
import { DateHelper } from "@montr-core/services/date-helper";
import { Classifier } from "@montr-master-data/models";
import { ClassifierService } from "@montr-master-data/services";
import { Spin } from "antd";
import React from "react";
import { NavigateFunction } from "react-router-dom";
import { IDocument } from "../models";
import { ClassifierTypeCode, EntityTypeCode, RouteBuilder, Views } from "../module";
import { DocumentService } from "../services";
import { DocumentBreadcrumb } from "./document-breadcrumb";

interface RouteProps {
	uid?: string;
	tabKey?: string;
}

interface Props {
	params: RouteProps;
	navigate: NavigateFunction;
}

interface State {
	loading: boolean;
	document?: IDocument;
	documentType?: Classifier;
	dataView?: DataView<Classifier>;
}

class PageViewDocument extends React.Component<Props, State> {

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

	getRouteProps = (): RouteProps => {
		return this.props.params;
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

		const path = RouteBuilder.viewDocument(uid, tabKey);

		this.props.navigate(path);
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

export default withNavigate(withParams(PageViewDocument));
