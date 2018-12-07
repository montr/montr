import * as React from "react";

import { Layout, Menu, Breadcrumb } from 'antd';

export class App extends React.Component {
	render() {
		return (
			<Layout className="public-layout">
				<Layout.Header>
					<div className="logo" />
					<Menu
						theme="light"
						mode="horizontal"
						defaultSelectedKeys={['3']}
						style={{ lineHeight: '64px' }}
					>
						<Menu.Item key="1">nav 1</Menu.Item>
						<Menu.Item key="2">nav 2</Menu.Item>
						<Menu.Item key="3">
							<a href="http://app.tendr.montr.io:5000/">App</a>
						</Menu.Item>
					</Menu>
				</Layout.Header>
				<Layout.Content style={{ padding: '0 50px' }}>
					<Breadcrumb style={{ margin: '16px 0' }}>
						<Breadcrumb.Item>Home</Breadcrumb.Item>
						<Breadcrumb.Item>List</Breadcrumb.Item>
						<Breadcrumb.Item>App</Breadcrumb.Item>
					</Breadcrumb>
					<div style={{ minHeight: 280 }}>

						<h1>PUBLIC</h1>


					</div>
				</Layout.Content>
				<Layout.Footer>© {new Date().getFullYear()}</Layout.Footer>
			</Layout>
		);
	}
}