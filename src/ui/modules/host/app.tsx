import * as React from "react";
import * as ReactDOM from "react-dom";
import { BrowserRouter, Switch } from "react-router-dom";
import { Spin, ConfigProvider } from "antd";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { AppLayoutRegistry } from "@montr-core/services";
import { ErrorBoundary, UserContextProvider, AuthCallbackHandler, AppRoute } from "@montr-core/components";
import { CompanyContextProvider } from "@montr-kompany/components";
import { PublicLayout, PrivateLayout } from "./";
import { AppRouteRegistry } from "@montr-core/services/";

import "@montr-core/routes";
import "@montr-master-data/routes";
import "@montr-tendr/routes";

import "@montr-core/i18n";
import "@montr-core/index.less"
import { Error404 } from "@montr-core/pages";

AppLayoutRegistry.register("public", PublicLayout);
AppLayoutRegistry.register("private", PrivateLayout);

class App extends React.Component {
	render = () => {
		return (
			<React.Suspense fallback={<Spin style={{ position: "fixed", top: "33%", left: "49%" }} />}>
				<ErrorBoundary>
					<ConfigProvider locale={ru_RU}>
						<UserContextProvider>
							<CompanyContextProvider>
								<AuthCallbackHandler>
									<BrowserRouter>
										<Switch>
											{AppRouteRegistry.Routes.map(({ layout, ...props }, _do_not_use_) => {
												return <AppRoute key={0} {...props} layoutComponent={AppLayoutRegistry.get(layout || "private")} />
											})}
											<AppRoute component={Error404} layoutComponent={AppLayoutRegistry.get("public")} />
										</Switch>
									</BrowserRouter>
								</AuthCallbackHandler>
							</CompanyContextProvider>
						</UserContextProvider>
					</ConfigProvider>
				</ErrorBoundary>
			</React.Suspense>
		);
	}
}

ReactDOM.render(<App />, document.getElementById("root"));
