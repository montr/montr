import React from "react";
import { Page, Toolbar, PageHeader, DataBreadcrumb } from "@montr-core/components";

interface Props {
}

interface State {
}

export default class PageSettings extends React.Component<Props, State> {
	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	render = () => {

		return (
			<Page
				title={<>
					<Toolbar float="right">
					</Toolbar>

					<DataBreadcrumb items={[{ name: "Настройки" }]} />
					<PageHeader>Настройки</PageHeader>
				</>}>


			</Page>
		);
	};
}
