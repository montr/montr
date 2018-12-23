import * as React from "react";

import { Layout, Breadcrumb } from "antd";

import { TopMenu } from "../../components";

export class App extends React.Component {
	render() {
		return (
			<Layout className="public-layout">
				<Layout.Header>
					<TopMenu />
				</Layout.Header>
				<Layout.Content style={{ padding: '0 50px' }}>
					<Breadcrumb style={{ margin: '16px 0' }}>
						<Breadcrumb.Item>Home</Breadcrumb.Item>
						<Breadcrumb.Item>List</Breadcrumb.Item>
						<Breadcrumb.Item>App</Breadcrumb.Item>
					</Breadcrumb>
					<div style={{ minHeight: 280 }}>

						<h1>TENDR</h1>

					</div>
				</Layout.Content>
				<Layout.Footer>© {new Date().getFullYear()}</Layout.Footer>
			</Layout>
		);
	}
}