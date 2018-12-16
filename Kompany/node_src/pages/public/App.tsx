import * as React from "react";

import { LocaleProvider, Layout, Menu, Breadcrumb } from "antd";
import * as ru_RU from "antd/lib/locale-provider/ru_RU";

import { Registration } from "./Registration";

export class App extends React.Component {
	render() {
		return (
			<LocaleProvider locale={ru_RU as any}>
				<Layout className="public-layout">
					<Layout.Header>
						<Menu
							theme="light"
							mode="horizontal"
							style={{ lineHeight: "64px" }}>
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
					<Layout.Content style={{ padding: "0 50px" }}>
						<Breadcrumb style={{ margin: "16px 0" }}>
							<Breadcrumb.Item>Home</Breadcrumb.Item>
							<Breadcrumb.Item>List</Breadcrumb.Item>
							<Breadcrumb.Item>App</Breadcrumb.Item>
						</Breadcrumb>
						<div style={{ minHeight: 280 }}>

							<h1>Регистрация компании</h1>

							<Registration />

						</div>
					</Layout.Content>
					<Layout.Footer>© {new Date().getFullYear()}</Layout.Footer>
				</Layout>
			</LocaleProvider>
		);
	}
}