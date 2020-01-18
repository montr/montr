import React from "react";
import { Page, PageHeader, DataTable } from "@montr-core/components";
import { Constants } from "@montr-core/.";

interface IProps {
}

interface IState {
}

export default class PageSearchProcess extends React.Component<IProps, IState> {
	render = () => {
		return (
			<Page
				title={<>
					<PageHeader>Processes</PageHeader>
				</>}>

				<DataTable
					rowKey="uid"
					viewId={`Process/List`}
					loadUrl={`${Constants.apiURL}/process/list/`}
				/>

			</Page>
		);
	};
}
