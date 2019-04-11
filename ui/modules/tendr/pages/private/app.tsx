import * as React from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { LocaleProvider, Layout } from "antd";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { Routes } from ".";
import { Routes as MasterDataRoutes } from "@montr-master-data/.";
import { AuthCallbackHandler, UserContextProvider, DataMenu } from "@montr-core/components";
import { CompanyContextProvider, UserWithCompanyMenu } from "@kompany/components";
import { NotificationService } from "@montr-core/services";

interface IProps {
}

interface IState {
}

export class App extends React.Component<IProps, IState> {

	private _notification = new NotificationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
		};
	}

	componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
		this._notification.error("App.componentDidCatch ", error.message);
	}

	render() {

		const siderWidth = 220;

		return (
			<Router>
				<LocaleProvider locale={ru_RU}>
					<UserContextProvider>
						<CompanyContextProvider>
							<AuthCallbackHandler>
								<Layout hasSider className="private-layout bg-white">

									<Layout.Sider theme="light" collapsible={false} width={siderWidth}
										style={{ overflow: 'auto', height: "100vh", position: 'fixed', left: 0 }}>
										{/* <div className="logo" /> */}

										<DataMenu
											menuId="SideMenu"
											theme="light"
											mode="inline"
											tail={
												<UserWithCompanyMenu />
											}
										/>

									</Layout.Sider>

									<Layout style={{ marginLeft: siderWidth }} className="bg-white">
										<Layout.Content className="bg-white">

											<Routes />
											<MasterDataRoutes />

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
