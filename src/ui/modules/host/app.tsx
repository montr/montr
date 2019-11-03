import * as React from "react";
import * as ReactDOM from "react-dom";
import { ConfigProvider } from "antd";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { Layout } from "@montr-core/constants";
import { AppLayoutRegistry, AppRouteRegistry } from "@montr-core/services";
import { ErrorBoundary, UserContextProvider, AuthCallbackHandler, AppRouteList, SuspenseFallback } from "@montr-core/components";
import { CompanyContextProvider } from "@montr-kompany/components";
import { PublicLayout, PrivateLayout } from "./components";

import "@montr-core/i18n";
import "@montr-core/index.less"
import "./modules"

AppLayoutRegistry.register(Layout.public, PublicLayout);
AppLayoutRegistry.register(Layout.private, PrivateLayout);

class App extends React.Component {
	render = () => {
		return (
			<React.Suspense fallback={<SuspenseFallback />}>
				<ErrorBoundary>
					<ConfigProvider locale={ru_RU}>
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
					</ConfigProvider>
				</ErrorBoundary>
			</React.Suspense>
		);
	}
}

ReactDOM.render(<App />, document.getElementById("root"));
