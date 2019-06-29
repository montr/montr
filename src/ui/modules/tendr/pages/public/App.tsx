import * as React from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { LocaleProvider, Layout, Breadcrumb } from "antd";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { AuthCallbackHandler, UserContextProvider, DataMenu } from "@montr-core/components";
import { CompanyContextProvider, UserWithCompanyMenu } from "@kompany/components/.";
import { Routes } from "./routes";

export class App extends React.Component {
	render() {
		return (
			<Router>
				<LocaleProvider locale={ru_RU}>
					<UserContextProvider>
						<CompanyContextProvider>
							<AuthCallbackHandler>
								<Layout className="public-layout">
									<Layout.Header>

										<DataMenu
											menuId="TopMenu"
											theme="light"
											mode="horizontal"
											style={{ lineHeight: "64px" }}
											tail={
												<UserWithCompanyMenu style={{ float: "right" }} />
											}
										/>

									</Layout.Header>
									<Layout.Content style={{ padding: "0 50px" }}>

										<Breadcrumb style={{ margin: "16px 0" }}>
											<Breadcrumb.Item>Home</Breadcrumb.Item>
											<Breadcrumb.Item>List</Breadcrumb.Item>
											<Breadcrumb.Item>App</Breadcrumb.Item>
										</Breadcrumb>

										<div style={{ minHeight: 280 }}>

											<Routes />

										</div>
									</Layout.Content>
									<Layout.Footer>© {new Date().getFullYear()}</Layout.Footer>
								</Layout>
							</AuthCallbackHandler>
						</CompanyContextProvider>
					</UserContextProvider>
				</LocaleProvider>
			</Router>
		);
	}
}
