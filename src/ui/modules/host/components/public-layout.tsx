import * as React from "react";
import { Layout, Breadcrumb } from "antd";
import { DataMenu, ErrorBoundary, Footer } from "@montr-core/components";
import { UserWithCompanyMenu } from "@montr-kompany/components/.";

export class PublicLayout extends React.Component {
	render = () => {
		return (
			<Layout className="public-layout">
				<Layout.Header>
					<ErrorBoundary>
						<DataMenu
							menuId="TopMenu"
							theme="light"
							mode="horizontal"
							style={{ lineHeight: "64px" }}
							tail={
								<UserWithCompanyMenu style={{ float: "right" }} />
							}
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

							{this.props.children}

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
	}
}
