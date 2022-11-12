import { Page, PageHeader, Toolbar } from "@montr-core/components";
import { DataBreadcrumb } from "@montr-core/components/data-breadcrumb";
import { DataMenu } from "@montr-core/components/data-menu";
import React from "react";
import { Translation } from "react-i18next";
import { Outlet } from "react-router-dom";

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
								<Outlet />
							</div>
						</div>
					</div>
				</Page>}
			</Translation>
		);
	};
}
