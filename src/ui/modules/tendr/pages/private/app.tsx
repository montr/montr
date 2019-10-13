import * as React from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { ConfigProvider, Layout, Spin } from "antd";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { Routes } from ".";
import { Routes as MasterDataRoutes } from "@montr-master-data/.";
import { AuthCallbackHandler, UserContextProvider, DataMenu, ErrorBoundary, Footer } from "@montr-core/components";
import { CompanyContextProvider, UserWithCompanyMenu } from "@kompany/components";

export class App extends React.Component {

	render() {

		const siderWidth = 220;

		return (
			<React.Suspense fallback={<Spin style={{ position: "fixed", top: "33%", left: "49%" }} />}>
				<ErrorBoundary>
					<Router>
						<ConfigProvider locale={ru_RU}>
							<UserContextProvider>
								<CompanyContextProvider>
									<AuthCallbackHandler>
										<Layout hasSider className="private-layout bg-white">
											<Layout.Sider theme="light" collapsible={false} width={siderWidth}
												style={{ overflow: 'auto', height: "100vh", position: 'fixed', left: 0 }}>
												<ErrorBoundary>
													{/* <div className="logo" /> */}

													<DataMenu
														menuId="SideMenu"
														theme="light"
														mode="inline"
														tail={
															<UserWithCompanyMenu />
														}
													/>
												</ErrorBoundary>
											</Layout.Sider>
											<Layout style={{ marginLeft: siderWidth }} className="bg-white">
												<Layout.Content className="bg-white">
													<ErrorBoundary>
														<Routes />
														<MasterDataRoutes />
													</ErrorBoundary>
												</Layout.Content>
												<Layout.Footer className="bg-white">

													<ErrorBoundary>
														<Footer />
													</ErrorBoundary>

												</Layout.Footer>
											</Layout>
										</Layout>
									</AuthCallbackHandler>
								</CompanyContextProvider>
							</UserContextProvider>
						</ConfigProvider>
					</Router>
				</ErrorBoundary>
			</React.Suspense>
		);
	}
}
