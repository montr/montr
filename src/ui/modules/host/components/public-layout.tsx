import { DataBreadcrumb } from "@montr-core/components/data-breadcrumb";
import { ErrorBoundary } from "@montr-core/components/error-boundary";
import { Footer } from "@montr-core/components/footer";
import { Layout } from "antd";
import * as React from "react";
import { Outlet } from "react-router-dom";
import { MainMenu } from "./main-menu";

export default class PublicLayout extends React.Component {

	render = () => {
		return (
			<Layout className="public-layout">
				<Layout.Header>
					<ErrorBoundary>
						<MainMenu
							menuId="TopMenu"
							mode="horizontal"
						// style={{ lineHeight: "64px" }}
						// tail={this.getUserWithCompanyMenu()}
						/* tail={
							<UserWithCompanyMenu style={{ float: "right" }} />
						} */
						/>
					</ErrorBoundary>
				</Layout.Header>
				<Layout.Content style={{ padding: "0 50px" }}>
					<ErrorBoundary>
						<DataBreadcrumb items={[]} />

						<div style={{ minHeight: 280 }}>

							<Outlet />

						</div>
					</ErrorBoundary>
				</Layout.Content>
				<Layout.Footer>
					<ErrorBoundary>
						<Footer />
					</ErrorBoundary>
				</Layout.Footer>
			</Layout>
		);
	};
}
