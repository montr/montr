import * as React from "react";
import * as ReactDOM from "react-dom";
import { ConfigProvider } from "antd";
import { Translation } from "react-i18next";
import { Locale } from "antd/lib/locale-provider";
import en_US from "antd/lib/locale-provider/en_US";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { Layout } from "@montr-core/constants";
import { AppLayoutRegistry, AppRouteRegistry } from "@montr-core/services";
import { ErrorBoundary, UserContextProvider, AuthCallbackHandler, AppRouteList, SuspenseFallback } from "@montr-core/components";
import { CompanyContextProvider } from "@montr-kompany/components";
import * as Layouts from "./components";

import "./modules";

AppLayoutRegistry.register(Layout.auth, Layouts.AuthLayout);
AppLayoutRegistry.register(Layout.public, Layouts.PublicLayout);
AppLayoutRegistry.register(Layout.private, Layouts.PrivateLayout);

function getLocale(lng: string): Locale {
	if (lng == "ru") return ru_RU;
	return en_US;
}

class App extends React.Component {
	render = () => {
		return (
			<React.Suspense fallback={<SuspenseFallback />}>
				<ErrorBoundary>
					<Translation>{(t, { i18n, lng }) =>
						<ConfigProvider locale={getLocale(lng)}>
							<UserContextProvider>
								<CompanyContextProvider>
									<AuthCallbackHandler>
										<AppRouteList
											routes={AppRouteRegistry.Routes}
											layoutRegistry={AppLayoutRegistry.get}
											defaultLayout={Layout.private}
											errorLayout={Layout.public}
										/>
									</AuthCallbackHandler>
								</CompanyContextProvider>
							</UserContextProvider>
						</ConfigProvider>}
					</Translation>
				</ErrorBoundary>
			</React.Suspense>
		);
	};
}

ReactDOM.render(<App />, document.getElementById("root"));
