import * as React from "react";
import { BrowserRouter as Router, Route } from "react-router-dom";
import { LocaleProvider, Layout, message } from "antd";
import * as ru_RU from "antd/lib/locale-provider/ru_RU";
import { SideMenu } from "../../components";
import { Dashboard, SearchEvents, EditEvent, SelectEventTemplate } from ".";
import { AuthCallbackHandler, UserContextProvider } from "@montr-core/components";
import { CompanyContextProvider } from "@kompany/components";

export class App extends React.Component {

	componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
		message.error("App.componentDidCatch " + error.message);
	}

	render() {
		return (
			<Router /* basename="/#" */ >
				<LocaleProvider locale={ru_RU as any}>
					<UserContextProvider>
						<CompanyContextProvider>
							<AuthCallbackHandler>
								<Layout hasSider className="private-layout">

									<Layout.Sider theme="dark" breakpoint="lg" collapsedWidth="0" width="230"
										style={{ height: "100vh" }}>
										{/* <div className="logo" /> */}

										<SideMenu />
									</Layout.Sider>

									<Layout className="bg-white">
										<Layout.Content>

											<Route path="/" exact component={() => <Dashboard />} />
											<Route path="/events" exact component={() => <SearchEvents />} />
											<Route path="/events/new" component={() => <SelectEventTemplate />} />
											<Route path="/events/edit/:id"
												component={({ match }: any) => <EditEvent {...match} />} />

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
