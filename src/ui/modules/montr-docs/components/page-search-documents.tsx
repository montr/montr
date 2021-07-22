import { DataTable, Page, PageHeader } from "@montr-core/components";
import React from "react";
import { DocumentBreadcrumb } from ".";
import { Api, Views } from "../module";

export default class PageSearchDocuments extends React.Component {
	render = (): React.ReactNode => {
		return (
			<Page
				title={<>
					<DocumentBreadcrumb />
					<PageHeader>Documents</PageHeader>
				</>}>

				<DataTable
					rowKey="uid"
					viewId={Views.documentList}
					loadUrl={Api.documentList}
				/>

			</Page>
		);
	};
}

