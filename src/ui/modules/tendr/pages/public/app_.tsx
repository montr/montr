import * as React from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { ConfigProvider, Layout, Breadcrumb, Spin } from "antd";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { AuthCallbackHandler, UserContextProvider, DataMenu, ErrorBoundary, Footer } from "@montr-core/components";
import { CompanyContextProvider, UserWithCompanyMenu } from "@kompany/components/.";
import { Routes } from "./routes";

export class App extends React.Component {
	render() {
		return (
			<React.Suspense fallback={<Spin style={{ position: "fixed", top: "33%", left: "49%" }} />}>
				<ErrorBoundary>
					<Router>
						<ConfigProvider locale={ru_RU}>
							<UserContextProvider>
								<CompanyContextProvider>
									<AuthCallbackHandler>
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

														<Routes />

													</div>
												</ErrorBoundary>
											</Layout.Content>
											<Layout.Footer>
												<ErrorBoundary>
													<Footer />
												</ErrorBoundary>
											</Layout.Footer>
										</Layout>
									</AuthCallbackHandler>
								</CompanyContextProvider>
							</UserContextProvider>
						</ConfigProvider>
					</Router>
				</ErrorBoundary>
			</React.Suspense >
		);
	}
}
