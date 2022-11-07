import { ErrorBoundary } from "@montr-core/components/error-boundary";
import { Footer } from "@montr-core/components/footer";
import { Breadcrumb, Layout } from "antd";
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
						<Breadcrumb style={{ margin: "16px 0" }}>
							<Breadcrumb.Item>Home</Breadcrumb.Item>
							<Breadcrumb.Item>List</Breadcrumb.Item>
							<Breadcrumb.Item>App</Breadcrumb.Item>
						</Breadcrumb>

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
