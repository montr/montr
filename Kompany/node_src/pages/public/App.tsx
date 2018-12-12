import * as React from "react";

import { Layout, Menu, Breadcrumb } from 'antd';

import { Registration } from "./Registration";

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
						<Menu.Item key="1">
							<a href="http://idx.montr.io:5050/">idx</a>
						</Menu.Item>
						<Menu.Item key="2">
							<a href="http://tendr.montr.io:5000/">tendr</a>
						</Menu.Item>
						<Menu.Item key="3">
							<a href="http://app.tendr.montr.io:5000/">app.tendr</a>
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

						<h2>Регистрация компании</h2>

						<Registration />

					</div>
				</Layout.Content>
				<Layout.Footer>© {new Date().getFullYear()}</Layout.Footer>
			</Layout>
		);
	}
}