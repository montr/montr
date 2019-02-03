import * as React from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { LocaleProvider, Layout } from "antd";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { Routes } from ".";
import { AuthCallbackHandler, UserContextProvider, DataMenu } from "@montr-core/components";
import { CompanyContextProvider, UserWithCompanyMenu } from "@kompany/components";
import { NotificationService } from "@montr-core/services";

export class App extends React.Component {

	private _notification = new NotificationService();

	componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
		this._notification.error({ message: "App.componentDidCatch " + error.message });
	}

	render() {
		return (
			<Router>
				<LocaleProvider locale={ru_RU}>
					<UserContextProvider>
						<CompanyContextProvider>
							<AuthCallbackHandler>
								<Layout hasSider className="private-layout">

									<Layout.Sider theme="light" breakpoint="lg" collapsedWidth="0" width="220"
										style={{ height: "100vh" }}>
										{/* <div className="logo" /> */}

										<DataMenu
											menuId="SideMenu"
											theme="light"
											mode="vertical"
											tail={
												<UserWithCompanyMenu />
											}
										/>

									</Layout.Sider>

									<Layout className="bg-white">
										<Layout.Content>

											<Routes />

										</Layout.Content>
										<Layout.Footer className="bg-white">© {new Date().getFullYear()}</Layout.Footer>
									</Layout>

								</Layout>
							</AuthCallbackHandler>
						</CompanyContextProvider>
					</UserContextProvider>
				</LocaleProvider>
			</Router >
		);
	}
}
