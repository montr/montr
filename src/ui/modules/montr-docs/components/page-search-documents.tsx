import { DataTable, Page, PageHeader } from "@montr-core/components";
import { DataColumn } from "@montr-core/models";
import React from "react";
import { DocumentBreadcrumb } from ".";
import { Api, Views } from "../module";
import { DocumentService } from "../services";

interface State {
	loading: boolean;
	columns?: DataColumn[];
}

export default class PageSearchDocuments extends React.Component<unknown, State> {

	private readonly documentService = new DocumentService();

	constructor(props: unknown) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchMetadata();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.documentService.abort();
	};

	fetchMetadata = async (): Promise<void> => {
		const dataView = await this.documentService.metadata(Views.documentList);

		this.setState({ loading: false, columns: dataView.columns });
	};

	render = (): React.ReactNode => {
		const { columns } = this.state;

		return (
			<Page
				title={<>
					<DocumentBreadcrumb />
					<PageHeader>Documents</PageHeader>
				</>}>

				<DataTable
					rowKey="uid"
					columns={columns}
					loadUrl={Api.documentList}
				/>

			</Page>
		);
	};
}

