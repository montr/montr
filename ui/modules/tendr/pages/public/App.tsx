import * as React from "react";
import { BrowserRouter as Router, Route } from "react-router-dom";
import { LocaleProvider, Layout, Breadcrumb } from "antd";
import * as ru_RU from "antd/lib/locale-provider/ru_RU";
import { AuthCallbackHandler, TopMenu, UserContextProvider } from "@montr-core/components";
import { Registration } from "./registration";

export class App extends React.Component {
	render() {
		return (
			<Router /* basename="/#" */ >
				<LocaleProvider locale={ru_RU as any}>
					<UserContextProvider>
						<AuthCallbackHandler>
							<Layout className="public-layout">
								<Layout.Header>
									<TopMenu />
								</Layout.Header>
								<Layout.Content style={{ padding: '0 50px' }}>

									<Breadcrumb style={{ margin: "16px 0" }}>
										<Breadcrumb.Item>Home</Breadcrumb.Item>
										<Breadcrumb.Item>List</Breadcrumb.Item>
										<Breadcrumb.Item>App</Breadcrumb.Item>
									</Breadcrumb>

									<div style={{ minHeight: 280 }}>

										<Route path="/registration" exact component={Registration} />

									</div>
								</Layout.Content>
								<Layout.Footer>© {new Date().getFullYear()}</Layout.Footer>
							</Layout>
						</AuthCallbackHandler>
					</UserContextProvider>
				</LocaleProvider>
			</Router>
		);
	}
}
