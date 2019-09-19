import * as React from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { ConfigProvider, Layout, Breadcrumb } from "antd";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { AuthCallbackHandler, UserContextProvider, DataMenu } from "@montr-core/components";
import { CompanyContextProvider, UserWithCompanyMenu } from "@kompany/components/.";
import { Routes } from "./routes";
import { Footer } from "../../components";

export class App extends React.Component {
	render() {
		return (
			<Router>
				<ConfigProvider locale={ru_RU}>
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
									<Layout.Footer>

										<Footer />

									</Layout.Footer>
								</Layout>
							</AuthCallbackHandler>
						</CompanyContextProvider>
					</UserContextProvider>
				</ConfigProvider>
			</Router>
		);
	}
}
