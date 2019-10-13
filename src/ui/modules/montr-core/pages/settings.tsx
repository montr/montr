import React from "react";
import { Page, Toolbar, PageHeader, DataBreadcrumb } from "@montr-core/components";

interface IProps {
}

interface IState {
}

export class Settings extends React.Component<IProps, IState> {
	constructor(props: IProps) {
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
	}
}
