import React from "react";
import { Page, PageHeader, DataTable } from "@montr-core/components";
import { Views, Api } from "../module";

interface IProps {
}

interface IState {
}

export default class PageSearchProcesses extends React.Component<IProps, IState> {
	render = () => {
		return (
			<Page
				title={<>
					<PageHeader>Processes</PageHeader>
				</>}>

				<DataTable
					rowKey="uid"
					viewId={Views.processList}
					loadUrl={Api.processList}
				/>

			</Page>
		);
	};
}
