import React from "react";
import { Page, PageHeader, DataTable } from "@montr-core/components";
import { Views, Api } from "../module";

interface IProps {
}

interface IState {
}

export default class PageSearchDocuments extends React.Component<IProps, IState> {
	render = () => {
		return (
			<Page
				title={<>
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
