import React from "react";
import { Translation } from "react-i18next";
import { DataMenu, Page, PageHeader, Toolbar } from ".";
import { DataBreadcrumb } from "./data-breadcrumb";

export default class PageSettings extends React.Component {

	private main: HTMLDivElement | undefined = undefined;

	render = (): React.ReactNode => {
		return (
			<Translation>
				{(t) => <Page
					title={<>
						<Toolbar float="right">
						</Toolbar>

						<DataBreadcrumb items={[{ name: "Settings" }]} />
						<PageHeader>Settings</PageHeader>
					</>}>

					{/* todo: good names & create components */}
					<div className="grid-content">
						<div className="page-with-menu" ref={ref => { if (ref) { this.main = ref; } }}>
							<div className="menu">
								<DataMenu menuId="SettingsMenu" />
							</div>
							<div className="content">
								{/* <RouteList routes={SettingsRoutes} /> */}
							</div>
						</div>
					</div>
				</Page>}
			</Translation>
		);
	};
}
